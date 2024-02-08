
namespace mmGameEngine
{
    public class GridComponent : Component
    {
        private int[,] grid;
        public int Rows;
        public int Cols;
        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }
        public TileComponent[,] Tiles { get; set; }
        public GridComponent() 
        {
            Rows = 1;
            Cols = 1;
            grid = new int[Rows, Cols];
            Tiles = new TileComponent[Rows, Cols];
        }
        public GridComponent(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Tiles = new TileComponent[Rows, Cols];
            grid = new int[Rows, Cols];
        }
        public void SetTile(int r, int c, TileComponent tile)
        {
            Tiles[r,c] = tile;
        }
        public override void Update()
        {
            base.Update();
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;
        }
        public bool IsInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Cols;
        }
        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }
        public bool IsRowFull(int r)
        {
            //
            // if any col is empty, then its not full
            //
            for (int c = 0; c < Cols; c++) 
            {
                if (grid[r, c] == 0) return false;
            }
            return true;
        }
        public bool IsRowEmpty(int r)
        {
            //
            // if any col is full, then its not empty
            //
            for (int c = 0; c < Cols; c++)
            {
                if (grid[r, c] != 0) return false;
            }
            return true;
        }
        private void ClearRow(int r)
        {
            for (int c = 0; r < Cols; c++)
            {
                grid[r,c] = 0;
            }
        }
        private void MoveRowDown(int r, int numRows)
        {
            for (int c =0; c< Cols; c++)
            {
                grid[r + numRows, c] = grid[r,c];
                grid[r,c] = 0;
            }
        }
        public int ClearFullRows()
        {
            int cleared = 0;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }
            return cleared;
        }
    }
}
