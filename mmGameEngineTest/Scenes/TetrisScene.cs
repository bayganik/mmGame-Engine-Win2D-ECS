using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using mmGameEngine;
using Entitas;
using Microsoft.Graphics.Canvas;
using System.Reflection;
using System.Net.Sockets;
using Windows.UI.Xaml.Media;
using Microsoft.UI;

namespace mmGameEngineTest
{
    public class TetrisScene : Scene
    {
        CanvasBitmap[] tileImages;
        CanvasBitmap[,] imageControls;
        GridComponent Grid;
        int blockSize = 32;

        public Entity LabelEnt;
        public Entity ExplosionEnt;
        public Entity MissleEnt;
        public Entity ButtonEnt;
        private Entity turretEnt;


        Entity MsgEnt;
        public TetrisScene() 
        {
            Global.ScreenSize = new Windows.Foundation.Size(600, 800);
            Global.DebugRenderEnabled = true;
        }
        public override async Task Process()
        {
            //
            // TetrisBlock is 32x32
            //
            tileImages = new CanvasBitmap[8];
            tileImages[0] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Empty);   //block empty
            tileImages[1] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Cyan);
            tileImages[2] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Blue);
            tileImages[3] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Orange);
            tileImages[4] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Yellow);
            tileImages[5] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Green);
            tileImages[6] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Purple);
            tileImages[7] = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Tetris.Red);     //block red

            Entity boardEnt = Global.CreateGameEntity("board", new Vector2(100, 70));
            Grid = new GridComponent(22, 10);
;

            //PanelComponent pnl = new PanelComponent(0,0);
            //pnl.HasBorder = false;

            int entNumber = 0;
            Vector2 pos = Vector2.Zero; 
            for (int c = 0; c < 10; c++)                    //10 columns (width)
            {
                for (int r = 0; r < 22; r++)                //22 rows (height)
                {
                    pos = new Vector2((blockSize * c) + 100, (blockSize * r) + 70);
                    //
                    // Entity Id will determine which color block will disp
                    //
                    Entity tileEnt = Global.CreateGameEntity("tile", pos);
                    tileEnt.Tag = 0;

                    TileComponent tc = new TileComponent(blockSize,blockSize, tileImages[0]);
                    tc.Click += TileClicked;
                    tc.HasBorder = true;
                    tc.BackgroundColor = Windows.UI.Colors.Transparent;
                    Grid.SetTile(r, c, tc);
                    tileEnt.Add(tc);
                    entNumber++;
                }
            }
            boardEnt.Add(Grid);
            AddSystem(new TetrisKeySystem());
        }
        public void TileClicked(object sender)
        {
            TileComponent tc = (TileComponent)sender;
            tc.Image = tileImages[1];
            //tc.Opacity = 0.5f;
        }
    }
}
