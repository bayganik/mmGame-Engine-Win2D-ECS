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
    public class TurretMovementSystem : IExecuteSystem
    {
        GameScene ActiveScene;
        public void Execute()
        {

            //
            // Make sure you are in the correct scene!
            //
            //var MyScene = (Scene)Global.CurrentScene;
            //if (MyScene.GetType().Name != "PlayScene")
            //    return;

            ActiveScene = (GameScene)Global.CurrentScene;
            //
            // Get group with all matched entities from context
            //
            var entities = Context<Default>.AllOf<TurretComponent>().GetEntities();


            foreach (var entity in entities)
            {
                TransformComponent tr = entity.Get<TransformComponent>();

                var mousePos = ActiveScene.ScreenToWorld(Input.MousePosition);
                entity.Get<TransformComponent>().LookAt(mousePos);
                //
                // Fire a shot
                //
                if (Input.IsKeyPressed("space"))
                {
                    ActiveScene.FireShot(mousePos);
                    //Vector2 from = ActiveScene.MissleEnt.Get<TransformComponent>().Position;

                }
            }
        }
    }
}
