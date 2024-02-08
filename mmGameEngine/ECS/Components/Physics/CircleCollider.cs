using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;


namespace mmGameEngine
{

    public class CircleCollider : RenderComponent
    {
		public List<Vector2> BoxPoints;
		public CircleAABB CollisionBox;
		public float Radius;
		public Vector2 Center;
		public int RadiusMultiplier;			//in case of explosions (normal is ONE)

		RectangleF boxContainer;
		Vector2 theCenter;				//origin of the texture size
		bool setScenColliders;

		/// <summary>
		/// Box to check for collisions. Box goes around the image (using scales)
		/// </summary>
		/// <param name="boxToCollide"></param>
		public CircleCollider(int _x, int _y, float _radius, int _radiusMultplier = 1)
        {
			//Rectangle _boxToCollide = new Rectangle(_x, _y, _width, _height);
			theCenter = new Vector2(_x, _y);
			Radius = _radius;
			Center = new Vector2(_x, _y);
			RadiusMultiplier = _radiusMultplier;
			//boxContainer = new Rectangle(_boxToCollide.x - theCenter.X,
			//							 _boxToCollide.y - theCenter.Y,
			//							 _boxToCollide.width,
			//							 _boxToCollide.height);


			CollisionBox = new CircleAABB();
			RenderLayer = Global.BOXCOLLIDER_LAYER;             //make sure this is drawn first
		}
        public override void Update()
        {
            base.Update();
			//
			// Has Entity been assigned yet?
			//
			if (OwnerEntity == null)
				return;
			//
			// update location of box containing the collider
			//
			boxContainer.X = Transform.Position.X;
			boxContainer.Y = Transform.Position.Y;
			BoxPoints = new List<Vector2>();
			Vector2 topL = new Vector2(boxContainer.X, boxContainer.Y - Radius);		//north
			Vector2 topR = new Vector2(boxContainer.X - Radius,  boxContainer.Y);		//west
			Vector2 botL = new Vector2(boxContainer.X + Radius, boxContainer.Y);		//east
			Vector2 botR = new Vector2(boxContainer.X, boxContainer.Y + Radius);		//south
			BoxPoints.Add(topL);
			BoxPoints.Add(botL);
			BoxPoints.Add(botR);
			BoxPoints.Add(topR);
			//
			// Find the min & max vectors for collision
			//
            CollisionBox.Fit(BoxPoints);				//updates the position of this collider

			if (!setScenColliders)
			{
				//
				// update the database of colliders in this scene (happens only once)
				//
				SceneColliderManager.SetCollider(OwnerEntity, CollidreShape.Circle);
				setScenColliders = true;
			}
		}
        public override void Render()
		{
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            if (Global.DebugRenderEnabled)
                RenderDebug();
        }
		public void RenderDebug()
		{
			//draw a rectangle

		//Raylib.DrawCircle((int)boxContainer.x, (int)boxContainer.y, Radius * RadiusMultiplier, Color.GRAY);
		}
	}
}
