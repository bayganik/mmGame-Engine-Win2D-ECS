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
    public class TankMovementSystem : IExecuteSystem
    {
        GameScene ActiveScene;
        public void Execute()
        {
            ActiveScene = (GameScene)Global.CurrentScene;
            //
            // Get group with all matched entities from context
            //
            var entities = Context<Default>.AllOf<EntityCapturedComponent>().GetEntities();
            if (entities.Length == 0) return;

            foreach (var entity in entities)
            {
                ActiveScene.LabelEnt.Get<LabelComponent>().Text = Input.KeyValue;
                //
                // Move the tank and all its children
                //
                Vector2 direction = Vector2.Zero;
                if (Input.IsKeyPressed("W"))
                    direction = new Vector2(0, -1);
                else if (Input.IsKeyPressed("S"))
                    direction = new Vector2(0, 1);
                else if (Input.IsKeyPressed("D"))
                    direction = new Vector2(1,0);
                else if (Input.IsKeyPressed("A"))
                    direction = new Vector2(-1, 0);

                entity.Get<TransformComponent>().Position += direction;


                if (Input.RightMousePressed)
                {
                    var mousePos = Global.ScreenToWorld(Input.MousePosition);
                    entity.Get<TransformComponent>().Position = mousePos;
                }
            }
        }
    }
}
