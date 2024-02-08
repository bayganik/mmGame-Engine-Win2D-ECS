using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class BlockPosition
    {
        public int Row {  get; set; }
        public int Column { get; set; }
        public BlockPosition(int _row, int _column) 
        {
            Row = _row;
            Column = _column;
        }
    }
}
