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
    public class BulletMoveSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (GameScene)Global.CurrentScene;

            var entities = Context<Default>.AllOf<EntityMover>().GetEntities();

            foreach (var ent in entities)
            {
                if (!ent.Get<EntityMover>().Enabled)
                    continue;
                if (ent.Get<EntityMover>().IsMoving)
                    continue;
                //
                // We are Enabled & Not moving
                //
                CollisionResult cres = ent.Get<EntityMover>().MoveCollisionResult;
                //
                // stopped, Did we hit something?
                //
                if (cres.Collided)
                {
                    
                    //
                    // Tag 1000 is the cursor
                    //
                    if (cres.OwnerEntity.Tag == 1000)
                    {
                        Global.DestroyGameEntity(cres.OwnerEntity);
                    }
                    else
                    {
                        MyScene.ExpoldeMissile(cres.OwnerEntity.Get<TransformComponent>().Position);
                        Global.DestroyGameEntity(cres.OwnerEntity) ;
                    }

                      
                }
                //else
                //{
                //    //
                //    //stopped but didn't hit anything
                //    //
                //    ent.Get<EntityMover>().Enabled = false;
                //    ent.IsVisible = false;
                //    Global.DestroyGameEntity(ent);


                //}
                //
                // If it hit the cursor, then disable it
                //
                ent.Get<EntityMover>().Enabled = false;
                ent.Get<TransformComponent>().Enabled = false;
                ent.IsVisible = false;
                Global.DestroyGameEntity(ent);
            }
        }
    }
}
