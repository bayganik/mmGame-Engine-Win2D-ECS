using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Entitas;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace mmGameEngine
{
    /*
	 * Sprite animation using a sprite sheet with eaqually spaced frames
	 */
    public class SpriteAnimation : RenderComponent
    {

        public CanvasBitmap Texture2D;				// sprite image
        public Vector2 TextureCenter;
        /// <summary>
        /// rectangle in the Texture for this element
        /// </summary>
        public Rect DestRect;
        /// <summary>
        /// Each rectangle represent an image/sprite
        /// </summary>
        //public Rectangle DestRect;
        public Rect[] SourceFrames;

        /// <summary>
        /// the current state of the animation
        /// </summary>
        public AnimationState CurrentState { get; private set; } = AnimationState.None;
        public SpriteAnimationSet CurrentAnimation;
        public bool OriginReCalc = true;
        //
        // Animation sets 
        //
        Dictionary<string, SpriteAnimationSet> Animations = new Dictionary<string, SpriteAnimationSet>();
        //
        // play back
        //
        //int MAX_FRAME_SPEED = 15;
        //int MIN_FRAME_SPEED = 3;
        int currentFrame = 0;
        double timer = 0;

        //int framesCounter = 0;
        //int framesSpeed = 8;
        int FrameWidth = 0;
        int FrameHeight = 0;
        public SpriteAnimation(CanvasBitmap sheetTexture, int cellWidth, int cellHeight, int cellOffset = 0)
        {
            FrameHeight = cellHeight;
            FrameWidth = cellWidth;

            Texture2D = sheetTexture;
            var cols = (int)sheetTexture.Size.Width / cellWidth;
            var rows = (int)sheetTexture.Size.Height / cellHeight;
            SourceFrames = new Rect[cols * rows];
            //
            // Find source rectangles for each frame on this spritesheet
            // I assume they are uniformly done
            //
            int i = 0;
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    SourceFrames[i] = new Rect(x * cellWidth + cellOffset, y * cellHeight + cellOffset, cellWidth, cellHeight);
                    i++;
                }
            }
        }
        public void AddAnimation(string name = "", string frameNumbers = "", float fps = 3)
        {
            //
            // default is 3 fps or frames/second (3/60 = 0.05 seconds)
            //
            int numOfAnimFrames;

            //
            // frameNumbers  = "all" or "1,2,3,4"
            //
            if (string.IsNullOrEmpty(name))             //name assumed to be "all"
                name = "all";

            if (string.IsNullOrEmpty(frameNumbers))     //no frame numbers, then "all"
                frameNumbers = "all";

            string[] nums;
            if (frameNumbers.ToLower() == "all")
            {
                // if "all" then fine all frames
                numOfAnimFrames = SourceFrames.Count();
                nums = new string[numOfAnimFrames];
                for (int i = 0; i < numOfAnimFrames; i++)
                    nums[i] = i.ToString();
            }
            else
            {
                // if numbers supplied for frames "11,12,13,11"
                nums = Regex.Split(frameNumbers, ",");
                numOfAnimFrames = nums.Count();
            }

            SpriteAnimationSet saSet = new SpriteAnimationSet();
            saSet.FrameRate = fps;
            saSet.SpriteFrames = new Rect[numOfAnimFrames];
            for (int i = 0; i < nums.Count(); i++)
            {
                int framNum = Convert.ToInt32(nums[i]);
                saSet.SpriteFrames[i] = SourceFrames[framNum];
            }
            CurrentAnimation.SpriteFrames = saSet.SpriteFrames;
            Animations.Add(name, saSet);

        }
        public override void Update()
        {
            base.Update();
            //
            // This component is not attached to Entity yet, cycle out
            //
            if (OwnerEntity == null)
                return;

            if (!Enabled)
                return;

            if (CurrentState != AnimationState.Running)
                return;
            //
            // add delta time (timer = .20 is quarter of a second but 0.05 is slower)
            //
            timer += Global.DeltaTime;         //time it takes to render ONE frame 60/1000

            //
            // Change the current frame if number of frames > CurrentAnimation.FrameRate
            //
            //framesCounter++;
            //if (framesCounter > CurrentAnimation.FrameRate)
            //{
            //	currentFrame++;
            //	framesCounter = 0;
            //}
            if (timer >= (CurrentAnimation.FrameRate / 30))
            {
                timer = 0;
                currentFrame++;
            }
            //
            // Find out if current frame is more than the animation has
            //
            if (currentFrame >= CurrentAnimation.SpriteFrames.Count())
            {
                currentFrame = 0;
                //
                // If its ONCE only, then stop
                //
                if (!CurrentAnimation.Loop)
                {
                    CurrentState = AnimationState.None;
                }

            }
            //
            // Set the origin of the frame one time only
            //
            if (OriginReCalc)
            {
                TextureCenter = new Vector2(FrameWidth * 0.5f * Transform.Scale.X, FrameHeight * 0.5f * Transform.Scale.Y);
                Origin = TextureCenter;
                OriginReCalc = false;
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

            if (CurrentState != AnimationState.Running)
            {
                Transform.Enabled = false;
                return;
            }

            //DestRect = new Rect(Transform.Position.X, Transform.Position.Y,
            //                         FrameWidth * Transform.Scale.X,
            //                         FrameHeight * Transform.Scale.Y);
            Vector2 scale = Transform.Scale;

            Global.SpriteBatchDraw.DrawFromSpriteSheet(Texture2D,
                Transform.Position,
                CurrentAnimation.SpriteFrames[currentFrame],
                Vector4.One,
                Origin,
                Transform.Rotation,
                scale,
                Transform.Flip);
        }
        /// <summary>
        /// Play animation set once (loop = false) or continusly (loop = true)
        /// </summary>
        /// <param name="animName"></param>
        /// <param name="loop"></param>
        public void Play(string animName, bool loop = false)
        {

            if (!Animations.TryGetValue(animName, out CurrentAnimation))
                return;

            CurrentAnimation.Loop = loop;
            CurrentState = AnimationState.Running;
            //Transform.Enabled = true;
        }
        public void Stop()
        {
            CurrentState = AnimationState.None;
            currentFrame = 0;
        }
    }

    public struct SpriteAnimationSet
    {
        public Rect[] SpriteFrames;
        public float FrameRate;
        public bool Loop;
    }
    public enum AnimationState
    {
        None,
        Running,
        Paused,
        Completed
    }
}
