using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace mmGameEngineTest
{
    public class TetrisKeySystem : IExecuteSystem
    {
        GameScene ActiveScene;
        public void Execute()
        {
            //ActiveScene = (GameScene)Global.CurrentScene;
            ////
            //// Get group with all matched entities from context
            ////
            //var entities = Context<Default>.AllOf<EntityCapturedComponent>().GetEntities();
            //if (entities.Length == 0) return;

            //foreach (var entity in entities)
            //{
                //ActiveScene.LabelEnt.Get<LabelComponent>().Text = Input.KeyValue;
                //
                // Move the tank and all its children
                //
                Vector2 direction = Vector2.Zero;
                if (Input.IsKeyPressed("Right"))
                    direction = new Vector2(0, -1);
                else if (Input.IsKeyPressed("Left"))
                    direction = new Vector2(0, 1);
                else if (Input.IsKeyPressed("Down"))
                    direction = new Vector2(1,0);
                else if (Input.IsKeyPressed("Up"))              //rotate clockwise
                    direction = new Vector2(-1, 0);
                else if (Input.IsKeyPressed("Z"))               //counter clockwise
                    direction = new Vector2(0, 1);

            //}
        }
    }
}
