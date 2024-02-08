using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class TetrisManager
    {
        private readonly GridBlock[] blocks = new GridBlock[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()

        };
        private readonly Random rand = new Random();    
        public GridBlock NextBlock { get; set; }
        public TetrisManager() 
        {
            RandomBlock();

        }
        private GridBlock RandomBlock()
        { 
            return blocks[rand.Next(blocks.Length)]; 
        }
        public GridBlock GetAndUpdate()
        {
            GridBlock block = NextBlock;
            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);
            return block;
        }
    }
}
