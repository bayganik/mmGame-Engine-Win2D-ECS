using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class IBlock : TetrisBlock
    {
        private readonly Vector2[][] tiles = new Vector2[][]
        {
            new Vector2[]{new Vector2(1,0), new Vector2(1,1),new Vector2(1,2), new Vector2(1,3)},
            new Vector2[]{new Vector2(0,2), new Vector2(1,2),new Vector2(2,2), new Vector2(3,2)},
            new Vector2[]{new Vector2(2,0), new Vector2(2,1),new Vector2(2,2), new Vector2(2,3)},
            new Vector2[]{new Vector2(0,1), new Vector2(1,1),new Vector2(2,1), new Vector2(3,1)}
        };
        public override int Id => 1;
        protected override Vector2 StartOffset => new Vector2(-1,3);
        protected override Vector2[][] Tiles => tiles;

    }
}
