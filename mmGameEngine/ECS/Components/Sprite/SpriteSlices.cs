using System;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;
using Windows.Foundation;
using System.Drawing;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Graphics.DirectX;					//used for CreateFromByes
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

//using mmGameEngine.ECS.Components;
//using Microsoft.Xna.Framework.Graphics;

namespace mmGameEngine
{
	/// <summary>
	/// A Renderable Component to attach to an Entity
	/// 
	/// Each entity is a representation of a level (map) to display
	/// </summary>
	public class SpriteSlices : RenderComponent
    {
		/// <summary>
		/// sprite image
		/// </summary>
        public CanvasBitmap[] Texture2D;
		/// <summary>
		/// number of slices to track
		/// </summary>
		public int ScreenWidth;
		public int ScreenHeight;
		/// <summary>
		/// Walls in the Texture2D (pefect sqr)
		/// </summary>
		public Rect[] SliceView;                            //each rectangle is one pixel max of screen width  (width = 1) (height = texture height)
		public Rect[] SliceCurrentTexture;					//each rectangle is one pixel max of texture width (width = 1) (height = texture height)
		public System.Drawing.Color[] SliceTint;							//brightness of the texture
		public int[] CurrentTextureNum;                     //number of textures in this level (map of the level determines how many textures)
		//
		// Floor & Ceiling
		//
		public Rect[] FloorSliceView;                            //each rectangle is one pixel max of screen width  (width = 1) (height = texture height)
		public Rect[] FloorSliceCurrentTexture;                  //each rectangle is one pixel max of texture width (width = 1) (height = texture height)
		public System.Drawing.Color[] FloorSliceTint;                           //brightness of the texture

		public CanvasBitmap Floor;
		public CanvasBitmap Ceiling;
		public int LevelNum = 0;
		/// <summary>
		/// UVs for the texture region
		/// </summary>
		public readonly RectangleF Uvs;

		/// <summary>
		/// center of the sourceRect if it had a 0,0 origin. This is basically the center in sourceRect-space.
		/// </summary>
		/// <value>The center.</value>
		public readonly Vector2 Center;

		/// <summary>
		/// the origin that a RenderableComponent should use when using this Sprite. Defaults to the center.
		/// </summary>
		public Vector2 Origin;
		//--store our "slice" rects--//
		public Rect[] slices;
		public SpriteSlices()
		{ }
		public SpriteSlices(Vector2 scrnSize)
		{ 			
			//Center = new Vector2((float)texture.Size.Width * 0.5f, (float)texture.Size.Height * 0.5f);
			//Origin = new Vector2((float)texture.Size.Width / 2, (float)texture.Size.Height / 2);

			ScreenWidth = (int)scrnSize.X;
			ScreenHeight = (int)scrnSize.Y;

			//
			// Entire view sliced into one pixel rectangles with length of screen
			// each Rect gives us the x,y location on the screen 
			//
			SliceView = new Rect[ScreenWidth];
			for (int x = 0; x < ScreenWidth; x++)
			{
				SliceView[x] = new Rect(x, 0, 1, ScreenHeight);
			}
			//
			// Textures
			//
			SliceCurrentTexture = new Rect[ScreenWidth];
			CurrentTextureNum = new int[ScreenWidth];

			for (int x = 0; x < ScreenWidth; x++)
            {
				CurrentTextureNum[x] = 1;							//all slices get same texture for now
            }
            //
            // Tint for the slices
            //
            SliceTint = new System.Drawing.Color[ScreenWidth];
			for (int x = 0; x < ScreenWidth; x++)
			{
				SliceTint[x] = System.Drawing.Color.White;						//all slices get same color tint
			}

			//
			// floors \\
			//
			//FloorSliceView = new Rect[ScreenWidth];
			//for (int x = 0; x < ScreenWidth; x++)
			//{
			//	FloorSliceView[x] = new Rect(x, 0, 1, ScreenHeight);
			//}
			FloorSliceCurrentTexture = new Rect[ScreenWidth];
			for (int x = ScreenHeight / 2; x < ScreenWidth; x++)
			{
				FloorSliceCurrentTexture[x] = new Rect(x, 0, 1, ScreenHeight);
			}
			//
			// Tint for the slices
			//
			FloorSliceTint = new System.Drawing.Color[ScreenWidth];
			for (int x = 0; x < ScreenWidth; x++)
			{
				FloorSliceTint[x] = System.Drawing.Color.White;
			}
		}
		public Rect[] VerticalSlices(double spriteHeight)
		{
			slices = new Rect[(int)spriteHeight];

			//--loop through creating a "slice" for each texture x--//
			for (int x = 0; x < spriteHeight; x++)
			{
				slices[x] = new Rect(x, 0, 1, spriteHeight);
			}

			return slices;
		}

		/// <summary>
		/// Render the Sprite Component
		/// </summary>
		/// <param name="sb"></param>
		public void Render(CanvasSpriteBatch sb)
        {
            //
            // display for one Map level
            //
            //       for (int x = ScreenHeight / 2 ; x < ScreenWidth; x++)
            //       {
            //           sb.DrawFromSpriteSheet(
            //               Floor,                //texture to use
            //SliceView[x],                                   //dest rect                                    //dest rect
            //FloorSliceCurrentTexture[x],                        //source rect

            //ToVector4(FloorSliceTint[x]));                       //tint

            //       }

            for (int x = 0; x < ScreenWidth; x++)
            {
                sb.DrawFromSpriteSheet(
                    Texture2D[CurrentTextureNum[x]],                //texture to use
                    SliceView[x],                                   //dest rect
                    SliceCurrentTexture[x],                         //source rect
                    ToVector4(SliceTint[x]));                       //tint

            }

        }
		private static Vector4 ToVector4(System.Drawing.Color color)
		{
			return new Vector4(
			(float)color.R / 255.0f,
			(float)color.G / 255.0f,
			(float)color.B / 255.0f,
			(float)color.A / 255.0f);
		}
	}
}
