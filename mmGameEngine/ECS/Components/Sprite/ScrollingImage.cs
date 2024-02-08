using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace mmGameEngine
{
	/// <summary>
	/// Scrolling sprite. Note that ScrollingSprite overrides the Material so that it can wrap the UVs. This class requires the texture
	/// to not be part of an atlas so that wrapping can work.
	/// </summary>
	public class ScrollingImage : RenderComponent
	{
        protected Vector2 _localOffset = Vector2.Zero;
        //protected float _layerDepth;
        //protected int _renderLayer;
        //protected RectangleF _bounds;
        //protected bool _isVisible = true;
        //protected bool _areBoundsDirty = true;
        /// <summary>
        /// x speed of automatic scrolling in pixels/s
        /// </summary>
        public float ScrollSpeedX = 15;

		/// <summary>
		/// y speed of automatic scrolling in pixels/s
		/// </summary>
		public float ScrollSpeedY = 0;
		/// <summary>
		/// x value of the texture scroll
		/// </summary>
		/// <value>The scroll x.</value>
		public int ScrollX
		{
			get => (int)_sourceRect.X;
			set => _sourceRect.X = value;
		}

		/// <summary>
		/// y value of the texture scroll
		/// </summary>
		/// <value>The scroll y.</value>
		public int ScrollY
		{
			get => (int)_sourceRect.Y;
			set => _sourceRect.Y = value;
		}
		/// <summary>
		/// scale of the texture
		/// </summary>
		/// <value>The texture scale.</value>
		public Vector2 TextureScale
		{
			get => _textureScale;
			set
			{
				_textureScale = value;

				// recalulcate our inverseTextureScale and the source rect size
				_inverseTexScale = new Vector2(1f / _textureScale.X, 1f / _textureScale.Y);
			}
		}
		/// <summary>
		/// overridden width value so that the TiledSprite can have an independent width than its texture
		/// </summary>
		/// <value>The width.</value>
		public new int Width
		{
			get => (int)_sourceRect.Width;
			set
			{
				//_areBoundsDirty = true;
				_sourceRect.Width = value;
			}
		}
		/// <summary>
		/// overridden height value so that the TiledSprite can have an independent height than its texture
		/// </summary>
		/// <value>The height.</value>
		public new int Height
		{
			get => (int)_sourceRect.Height;
			set
			{
				//_areBoundsDirty = true;
				_sourceRect.Height = value;
			}
		}
		/// <summary>
		/// we keep a copy of the sourceRect so that we dont change the Sprite in case it is used elsewhere
		/// </summary>
		protected Rect _sourceRect;

		protected Vector2 _textureScale = Vector2.One;
		protected Vector2 _inverseTexScale = Vector2.One;
		CanvasBitmap Texture2D;

		// accumulate scroll in a separate float so that we can round it without losing precision for small scroll speeds
		float _scrollX, _scrollY;

		public ScrollingImage(CanvasBitmap texture)
		{
			_sourceRect = texture.Bounds;
			Texture2D = texture;
			RenderLayer = Global.SCROLLINGBACK_LAYER;

        }
		public override void Update()
		{
			base.Update();

            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            _scrollX += ScrollSpeedX * (float)Global.DeltaTime;
			_scrollY += ScrollSpeedY * (float)Global.DeltaTime;
			_sourceRect.X = (int)_scrollX;
			_sourceRect.Y = (int)_scrollY;
		}
		public override void Render()
		{
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            Vector2 Origin = new Vector2((float)Texture2D.Size.Width / 2, (float)Texture2D.Size.Height / 2);
			var topLeft = Transform.Position + _localOffset;
			Rect _destRect = new Rect(topLeft.X, topLeft.Y,
				_sourceRect.Width * Transform.Scale.X * TextureScale.X,
				_sourceRect.Height * Transform.Scale.Y * TextureScale.Y);
            Vector2 scale = Transform.Scale;
            Global.SpriteBatchDraw.DrawFromSpriteSheet(Texture2D, _destRect, _sourceRect);

	    }
	}
}
