using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Entitas;
using Microsoft.Graphics.Canvas;

namespace mmGameEngine
{
	public class TransformComponent : Component
	{
		TransformComponent _parent;
		DirtyType hierarchyDirty;

		bool _localDirty;
		bool _localPositionDirty;
		bool _localScaleDirty;
		bool _localRotationDirty;
		bool _positionDirty;
		bool _worldToLocalDirty;
		bool _worldInverseDirty;

		// value is automatically recomputed from the position, rotation and scale
		Matrix3x2 _localTransformComponent;

		// value is automatically recomputed from the local and the parent matrices.
		Matrix3x2 _worldTransform = Matrix3x2.Identity;
		Matrix3x2 _worldToLocalTransform = Matrix3x2.Identity;
		Matrix3x2 _worldInverseTransform = Matrix3x2.Identity;

		Matrix3x2 _rotationMatrix;
		Matrix3x2 _translationMatrix;
		Matrix3x2 _scaleMatrix;

		System.Numerics.Vector2 _position;
		System.Numerics.Vector2 _scale;
		float _rotation;

		System.Numerics.Vector2 _localPosition;
		System.Numerics.Vector2 _localScale;
		float _localRotation;

		public List<TransformComponent> Children = new List<TransformComponent>();
		[Flags]
		enum DirtyType
		{
			Clean,
			PositionDirty,
			ScaleDirty,
			RotationDirty
		}

		public enum Component
		{
			Position,
			Scale,
			Rotation
		}


		#region properties and fields


		public CanvasSpriteFlip Flip { get; set; }

		/// <summary>
		/// the parent TransformComponent of this TransformComponent
		/// </summary>
		/// <value>The parent.</value>
		public TransformComponent Parent
		{
			get => _parent;
			set => SetParent(value);
		}


		/// <summary>
		/// total children of this TransformComponent
		/// </summary>
		/// <value>The child count.</value>
		public int ChildCount => Children.Count;


		/// <summary>
		/// position of the TransformComponent in world space
		/// </summary>
		/// <value>The position.</value>
		public System.Numerics.Vector2 Position
		{
			get
			{
				UpdateTransformComponent();
				if (_positionDirty)
				{
					if (Parent == null)
					{
						_position = _localPosition;
					}
					else
					{
						Parent.UpdateTransformComponent();
						Vector2Ext.Transform(ref _localPosition, ref Parent._worldTransform, out _position);
					}

					_positionDirty = false;
				}

				return _position;
			}
			set => SetPosition(value);
		}


		/// <summary>
		/// position of the TransformComponent relative to the parent TransformComponent. If the TransformComponent has no parent, it is the same as TransformComponent.position
		/// </summary>
		/// <value>The local position.</value>
		public System.Numerics.Vector2 LocalPosition
		{
			get
			{
				UpdateTransformComponent();
				return _localPosition;
			}
			set => SetLocalPosition(value);
		}


		/// <summary>
		/// rotation of the TransformComponent in world space in radians
		/// </summary>
		/// <value>The rotation.</value>
		public float Rotation
		{
			get
			{
				UpdateTransformComponent();
				return _rotation;
			}
			set => SetRotation(value);
		}


		/// <summary>
		/// rotation of the TransformComponent in world space in degrees
		/// </summary>
		/// <value>The rotation degrees.</value>
		public float RotationDegrees
		{
			get => MathHelper.ToDegrees(_rotation);
			set => SetRotation(MathHelper.ToRadians(value));
		}


		/// <summary>
		/// the rotation of the TransformComponent relative to the parent TransformComponent's rotation. If the TransformComponent has no parent, it is the same as TransformComponent.rotation
		/// </summary>
		/// <value>The local rotation.</value>
		public float LocalRotation
		{
			get
			{
				UpdateTransformComponent();
				return _localRotation;
			}
			set => SetLocalRotation(value);
		}


		/// <summary>
		/// rotation of the TransformComponent relative to the parent TransformComponent's rotation in degrees
		/// </summary>
		/// <value>The rotation degrees.</value>
		public float LocalRotationDegrees
		{
			get => MathHelper.ToDegrees(_localRotation);
			set => LocalRotation = MathHelper.ToRadians(value);
		}


		/// <summary>
		/// global scale of the TransformComponent
		/// </summary>
		/// <value>The scale.</value>
		public System.Numerics.Vector2 Scale
		{
			get
			{
				UpdateTransformComponent();
				return _scale;
			}
			set => SetScale(value);
		}

		public TransformComponent()
		{
			_scale = _localScale = System.Numerics.Vector2.One;
		}
		/// <summary>
		/// the scale of the TransformComponent relative to the parent. If the TransformComponent has no parent, it is the same as TransformComponent.scale
		/// </summary>
		/// <value>The local scale.</value>
		public System.Numerics.Vector2 LocalScale
		{
			get
			{
				UpdateTransformComponent();
				return _localScale;
			}
			set => SetLocalScale(value);
		}


		public Matrix3x2 WorldInverseTransformComponent
		{
			get
			{
				UpdateTransformComponent();
				if (_worldInverseDirty)
				{
					Matrix3x2.Invert(_worldTransform, out _worldInverseTransform);
					_worldInverseDirty = false;
				}

				return _worldInverseTransform;
			}
		}


		public Matrix3x2 LocalToWorldTransformComponent
		{
			get
			{
				UpdateTransformComponent();
				return _worldTransform;
			}
		}


		public Matrix3x2 WorldToLocalTransformComponent
		{
			get
			{
				if (_worldToLocalDirty)
				{
					if (Parent == null)
					{
						_worldToLocalTransform = Matrix3x2.Identity;
					}
					else
					{
						Parent.UpdateTransformComponent();
						Matrix3x2.Invert(Parent._worldTransform, out _worldToLocalTransform);
					}

					_worldToLocalDirty = false;
				}

				return _worldToLocalTransform;
			}
		}




		#endregion

		/// <summary>
		/// returns the TransformComponent child at index
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="index">Index.</param>
		public TransformComponent GetChild(int index)
		{
			return Children[index];
		}


		#region Fluent setters

		/// <summary>
		/// sets the parent TransformComponent of this TransformComponent
		/// </summary>
		/// <returns>The parent.</returns>
		/// <param name="parent">Parent.</param>
		public TransformComponent SetParent(TransformComponent parent)
		{
			if (_parent == parent)
				return this;

			if (_parent != null)
				_parent.Children.Remove(this);

			if (parent != null)
				parent.Children.Add(this);

			_parent = parent;

			SetDirty(DirtyType.PositionDirty);

			return this;
		}


		/// <summary>
		/// sets the position of the TransformComponent in world space
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="position">Position.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetPosition(System.Numerics.Vector2 position)
		{
			if (position == _position)
				return this;

			_position = position;
			if (Parent != null)
			{
				
				LocalPosition = System.Numerics.Vector2.Transform(_position,WorldToLocalTransformComponent);
			}
			else
				LocalPosition = position;

			_positionDirty = false;

			return this;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetPosition(float x, float y)
		{
			return SetPosition(new System.Numerics.Vector2(x, y));
		}


		/// <summary>
		/// sets the position of the TransformComponent relative to the parent TransformComponent. If the TransformComponent has no parent, it is the same
		/// as TransformComponent.position
		/// </summary>
		/// <returns>The local position.</returns>
		/// <param name="localPosition">Local position.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetLocalPosition(System.Numerics.Vector2 localPosition)
		{
			if (localPosition == _localPosition)
				return this;

			_localPosition = localPosition;
			_localDirty = _positionDirty = _localPositionDirty = _localRotationDirty = _localScaleDirty = true;
			SetDirty(DirtyType.PositionDirty);

			return this;
		}


		/// <summary>
		/// sets the rotation of the TransformComponent in world space in radians
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetRotation(float radians)
		{
			_rotation = radians;
			if (Parent != null)
				LocalRotation = Parent.Rotation + radians;
			else
				LocalRotation = radians;

			return this;
		}


		/// <summary>
		/// sets the rotation of the TransformComponent in world space in degrees
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetRotationDegrees(float degrees)
		{
			return SetRotation(MathHelper.ToRadians(degrees));
		}


		/// <summary>
		/// sets the the rotation of the TransformComponent relative to the parent TransformComponent's rotation. If the TransformComponent has no parent, it is the
		/// same as TransformComponent.rotation
		/// </summary>
		/// <returns>The local rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetLocalRotation(float radians)
		{
			_localRotation = radians;
			_localDirty = _positionDirty = _localPositionDirty = _localRotationDirty = _localScaleDirty = true;
			SetDirty(DirtyType.RotationDirty);

			return this;
		}


		/// <summary>
		/// sets the the rotation of the TransformComponent relative to the parent TransformComponent's rotation. If the TransformComponent has no parent, it is the
		/// same as TransformComponent.rotation
		/// </summary>
		/// <returns>The local rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetLocalRotationDegrees(float degrees)
		{
			return SetLocalRotation(MathHelper.ToRadians(degrees));
		}

		/// <summary>
		/// Rotate so the top of the sprite is facing <see cref="pos"/>
		/// </summary>
		/// <param name="pos">The position to look at</param>
		public void LookAt(System.Numerics.Vector2 pos)
		{
			int sign = _position.X > pos.X ? -1 : 1;
			System.Numerics.Vector2 vectorToAlignTo = System.Numerics.Vector2.Normalize(_position - pos);
			Rotation = sign * Mathf.Acos(System.Numerics.Vector2.Dot(vectorToAlignTo, System.Numerics.Vector2.UnitY));

		}

		/// <summary>
		/// sets the global scale of the TransformComponent
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetScale(System.Numerics.Vector2 scale)
		{
			_scale = scale;
			if (Parent != null)
				LocalScale = scale / Parent._scale;
			else
				LocalScale = scale;

			return this;
		}


		/// <summary>
		/// sets the global scale of the TransformComponent
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetScale(float scale)
		{
			return SetScale(new System.Numerics.Vector2(scale));
		}


		/// <summary>
		/// sets the scale of the TransformComponent relative to the parent. If the TransformComponent has no parent, it is the same as TransformComponent.scale
		/// </summary>
		/// <returns>The local scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetLocalScale(System.Numerics.Vector2 scale)
		{
			_localScale = scale;
			_localDirty = _positionDirty = _localScaleDirty = true;
			SetDirty(DirtyType.ScaleDirty);

			return this;
		}


		/// <summary>
		/// sets the scale of the TransformComponent relative to the parent. If the TransformComponent has no parent, it is the same as TransformComponent.scale
		/// </summary>
		/// <returns>The local scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformComponent SetLocalScale(float scale)
		{
			return SetLocalScale(new System.Numerics.Vector2(scale));
		}

		#endregion


		/// <summary>
		/// rounds the position of the TransformComponent
		/// </summary>

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void UpdateTransformComponent()
		{
			if (hierarchyDirty != DirtyType.Clean)
			{
				if (Parent != null)
					Parent.UpdateTransformComponent();

				if (_localDirty)
				{
					if (_localPositionDirty)
					{
                        _translationMatrix = Matrix3x2.CreateTranslation(_localPosition.X, _localPosition.Y);
						_localPositionDirty = false;
					}

					if (_localRotationDirty)
					{
                        _rotationMatrix = Matrix3x2.CreateRotation(_localRotation);
						_localRotationDirty = false;
					}

					if (_localScaleDirty)
					{
						_scaleMatrix = Matrix3x2.CreateScale(_localScale.X, _localScale.Y);
						_localScaleDirty = false;
					}

                    _localTransformComponent = Matrix3x2.Multiply( _scaleMatrix, _rotationMatrix);
                    _localTransformComponent = Matrix3x2.Multiply(_localTransformComponent, _translationMatrix);

					if (Parent == null)
					{
						_worldTransform = _localTransformComponent;
						_rotation = _localRotation;
						_scale = _localScale;
						_worldInverseDirty = true;
					}

					_localDirty = false;
				}

				if (Parent != null)
				{
                    _worldTransform = Matrix3x2.Multiply( _localTransformComponent, Parent._worldTransform);

                    //if (Entity.Tag == "Missile")
                    //    _rotation = Parent._rotation;
                    //else
                    //	
                    //
                    // When we call LookAt method, we lose our child rotation
                    //
                    //_rotation = Parent._rotation;

                    _rotation = _localRotation + Parent._rotation;
					_scale = Parent._scale * _localScale;
					_worldInverseDirty = true;
				}

				_worldToLocalDirty = true;
				_positionDirty = true;
				hierarchyDirty = DirtyType.Clean;
			}
		}


		/// <summary>
		/// sets the dirty flag on the enum and passes it down to our children
		/// </summary>
		/// <param name="dirtyFlagType">Dirty flag type.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void SetDirty(DirtyType dirtyFlagType)
		{
			if ((hierarchyDirty & dirtyFlagType) == 0)
			{
				hierarchyDirty |= dirtyFlagType;

				//switch (dirtyFlagType)
				//{
				//	case DirtyType.PositionDirty:
				//		Entity.OnTransformComponentChanged(Component.Position);
				//		break;
				//	case DirtyType.RotationDirty:
				//		Entity.OnTransformComponentChanged(Component.Rotation);
				//		break;
				//	case DirtyType.ScaleDirty:
				//		Entity.OnTransformComponentChanged(Component.Scale);
				//		break;
				//}

				// dirty our children as well so they know of the changes
				for (var i = 0; i < Children.Count; i++)
					Children[i].SetDirty(dirtyFlagType);
			}
		}


		public void CopyFrom(TransformComponent TransformComponent)
		{
			_position = TransformComponent.Position;
			_localPosition = TransformComponent._localPosition;
			_rotation = TransformComponent._rotation;
			_localRotation = TransformComponent._localRotation;
			_scale = TransformComponent._scale;
			_localScale = TransformComponent._localScale;

			SetDirty(DirtyType.PositionDirty);
			SetDirty(DirtyType.RotationDirty);
			SetDirty(DirtyType.ScaleDirty);
		}


		public override string ToString()
		{
			return string.Format(
				"[TransformComponent: parent: {0}, position: {1}, rotation: {2}, scale: {3}, localPosition: {4}, localRotation: {5}, localScale: {6}]",
				Parent != null, Position, Rotation, Scale, LocalPosition, LocalRotation, LocalScale);
		}
	}
}