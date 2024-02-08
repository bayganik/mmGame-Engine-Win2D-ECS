using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;

namespace mmGameEngineTest
{
    public abstract class GridBlock
    {
        protected abstract int[,] Tiles { get;  }
        protected abstract Vector2 StartOffset { get; }
        public abstract int Id { get; }
        public int BlockState = 0;

        private int rotationState;
        private Vector2 offset;

        public GridBlock()
        {
            offset = new Vector2(StartOffset.X, StartOffset.Y);
            BlockState = 0;
        }
        public IEnumerable<int[,]> TileValues()
        {
            int[,] tileValues = new int[4,4];
            int index = rotationState * 4;

            tileValues[0, 0] = Tiles[index, 0];
            tileValues[0, 1] = Tiles[index, 1];
            tileValues[0, 2] = Tiles[index, 2];
            tileValues[0, 3] = Tiles[index, 3];
            index++;
            tileValues[1, 0] = Tiles[index, 0];
            tileValues[1, 1] = Tiles[index, 1];
            tileValues[1, 2] = Tiles[index, 2];
            tileValues[1, 3] = Tiles[index, 3];
            index++;
            tileValues[2, 0] = Tiles[index, 0];
            tileValues[2, 1] = Tiles[index, 1];
            tileValues[2, 2] = Tiles[index, 2];
            tileValues[2, 3] = Tiles[index, 3];
            index++;
            tileValues[3, 0] = Tiles[index, 0];
            tileValues[3, 1] = Tiles[index, 1];
            tileValues[3, 2] = Tiles[index, 2];
            tileValues[3, 3] = Tiles[index, 3];
            yield return tileValues;
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
