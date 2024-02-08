using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class TBlock : GridBlock
    {
        private readonly int[,] tiles = new int[,]
        {
            { 0, 0, 1, 0 },//-----
            { 0, 1, 1, 1 },//    |
            { 0, 0, 0, 0 },//    | state 0 of block
            { 0, 0, 0, 0 },//-----

            { 0, 0, 1, 0 },//-----
            { 0, 0, 1, 1 },//    |
            { 0, 0, 1, 0 },//    | state 1 of block
            { 0, 0, 0, 0 },//-----

            { 0, 0, 0, 0 },//-----
            { 0, 1, 1, 1 },//    |
            { 0, 0, 1, 0 },//    | state 2 of block
            { 0, 0, 0, 0 },//-----

            { 0, 0, 1, 0 },//-----
            { 0, 1, 1, 0 },//    |
            { 0, 0, 1, 0 },//    | state 3 of block
            { 0, 0, 0, 0 } //-----
        };
        public override int Id => 6;
        protected override Vector2 StartOffset => new Vector2(0,3);
        protected override int[,] Tiles => tiles;

    }
}
