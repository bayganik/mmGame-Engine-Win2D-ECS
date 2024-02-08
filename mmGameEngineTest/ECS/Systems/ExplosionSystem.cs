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
    public class ExplosionSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (GameScene)Global.CurrentScene;

            var entities = Context<Default>.AllOf<ExplosionComponent>().GetEntities();
            //
            // If the entity holding the animation is done, then remove it
            // from the scene
            //
            foreach (var ent in entities)
            {
                //
                // Get animation state and ask if done
                //
                AnimationState junk = MyScene.ExplosionEnt.Get<SpriteAnimation>().CurrentState;
                if (junk != AnimationState.None)
                    return;
                //
                // explosion is done playing, destroy the entity
                //
                ent.Get<TransformComponent>().Enabled = false;
                ent.IsVisible = false;
                Global.DestroyGameEntity(ent);

            }
        }
    }
}
