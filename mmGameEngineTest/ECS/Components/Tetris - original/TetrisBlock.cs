using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Media.Animation;

namespace mmGameEngineTest
{
    public abstract class TetrisBlock
    {
        protected abstract Vector2[][] Tiles{get;}
        protected abstract Vector2 StartOffset { get;}
        public abstract int Id {  get;}
        private int rotationState;
        private Vector2 offset;
        
        public TetrisBlock()
        {
            offset = new Vector2(StartOffset.X, StartOffset.Y);
        }
        public IEnumerable<Vector2> TilePositions()
        {
            foreach (Vector2 block in Tiles[rotationState]) 
            {
                yield return new Vector2(block.X + offset.X, block.Y + offset.Y);
            }
        }
        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;    
        }
        public void RotateCCW()
        {
            if (rotationState == 0)
                rotationState = Tiles.Length - 1;
            else
                rotationState--;
        }
        public void Move(int rows, int columns)
        {
            offset.X += rows;
            offset.Y += columns;
        }
        public void Reset()
        {
            rotationState = 0;
            offset.X = StartOffset.X;
            offset.Y = StartOffset.Y;
        }
    }
}
