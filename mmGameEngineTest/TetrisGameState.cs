using mmGameEngine;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class TetrisGameState
    {
        private GridBlock currentBlock;
        public GridBlock CurrentBlock
        {
            get => currentBlock;
            set
            {
                currentBlock = value;
                currentBlock.Reset();
            }
        }
        public GridComponent GameGrid { get; }
        public TetrisManager BlockQueue { get; }
        public bool GameOver { get; set; }
        public TetrisGameState() 
        {
            GameGrid = new GridComponent(22, 10);
            BlockQueue = new TetrisManager();
            CurrentBlock = BlockQueue.GetAndUpdate();
            GameOver = false;
        }
        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }
        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits()) 
            {
                CurrentBlock.Move(0, 1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
        private bool IsGameOver()
        {
            //
            // If 2 top rows are full, then game is lost
            //
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }
        private void PlaceBlock()
        {
            //foreach (int[,] p in CurrentBlock.TileValues())
            //{
            //    GameGrid[(int)p.X, (int)p.Y] = CurrentBlock.Id;
            //}
            GameGrid.ClearFullRows();
            if (IsGameOver())
                GameOver = true;
            else
                CurrentBlock = BlockQueue.GetAndUpdate();
        }
        private bool BlockFits()
        {
            foreach (int[,] p in CurrentBlock.TileValues())
            {
                for (int i = 0; i < 4; i++) 
                {
                    if (!GameGrid.IsEmpty(p[3, i], p[3, i]))
                    {
                        return false;
                    }
                }

            }
            return true;
        }
    }
}
