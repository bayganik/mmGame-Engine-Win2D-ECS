using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class OBlock : TetrisBlock
    {
        private readonly Vector2[][] tiles = new Vector2[][]
        {
            new Vector2[] { new Vector2(0,0), new Vector2(0,1) , new Vector2(1,0), new Vector2(1,1)}
        };
        public override int Id => 4;
        protected override Vector2 StartOffset => new Vector2(0, 4);
        protected override Vector2[][] Tiles => tiles;
    }
}
