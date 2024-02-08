using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmGameEngine;

namespace mmGameEngineTest
{
    //
    // This is the block currently moving on screen
    //
    public class CurrentTetrisBlock : Component 
    {
        public TetrisBlock CurrentBlock;
        public CurrentTetrisBlock() 
        { }

        public override void Update()
        {
            base.Update();
        }
    }
}
