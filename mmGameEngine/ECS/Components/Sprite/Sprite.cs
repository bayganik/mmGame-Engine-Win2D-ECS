using System;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using System.Drawing;
using Windows.Foundation;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Graphics.DirectX;					//used for CreateFromByes
using System.Linq;
//using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

//using mmGameEngine.ECS.Components;
//using Microsoft.Xna.Framework.Graphics;

namespace mmGameEngine
{
	/// <summary>
	/// A Renderable Component to attach to an Entity
	/// </summary>
	public class Sprite : RenderComponent
    {
        public CanvasBitmap Texture2D;				// sprite image
		public Rect SourceRect;                     // rectangle in the Texture2D for this element
        public readonly Vector2 Center;
		public Vector2 Origin;                      // Sprite. Defaults to the center.
		public Vector2 ScaleOverRide;				// scale to override
		
        public Sprite()
		{ }
		public Sprite(CanvasBitmap texture, Rect sourceRect, Vector2 origin)
        {
			Texture2D = texture;
			SourceRect = sourceRect;
			Center = new Vector2((float)sourceRect.Width * 0.5f, (float)sourceRect.Height * 0.5f);
			Origin = origin;
            ScaleOverRide = Vector2.Zero;
		}
		public Sprite(CanvasBitmap texture, Rect sourceRect) : this(texture, sourceRect, new Vector2((float)texture.Size.Width/2, (float)texture.Size.Height/2))
		{ }
        public Sprite(CanvasBitmap texture) : this(texture, new Rect(0, 0, (int)texture.Size.Width, (int)texture.Size.Height))
        { }
        public Sprite(CanvasBitmap texture, int x, int y, int width, int height) : this(texture,new Rect(x, y, width, height))
		{ }
		public static implicit operator CanvasBitmap(Sprite sprite) => sprite.Texture2D;
		public override void Update()
		{
			base.Update();

			if (OwnerEntity == null)
				return;
			if (!OwnerEntity.IsVisible)
				return;
			if (!Enabled)
				return;

		}
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            //
            // Entity.Transform.Position + LocalOffset (Vector2) consider for minor adjustments
            //
            Rect sRect = new Rect(SourceRect.X, SourceRect.Y, SourceRect.Width, SourceRect.Height);
			Vector2 scale = Transform.Scale;
			if (ScaleOverRide != Vector2.Zero)
				scale = ScaleOverRide;

            Global.SpriteBatchDraw.DrawFromSpriteSheet(Texture2D, 
				Transform.Position, 
				sRect,
				Vector4.One, 
				Origin, 
				Transform.Rotation, 
				scale, 
				Transform.Flip);
		}
	}
}
