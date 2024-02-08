using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace mmGameEngine
{
    public class CursorMoveSystem : IExecuteSystem
    {
        /*
         * All active scenes use the Cross Hair cursor
         */
        public void Execute()
        {
            var ActiveScene = (Scene)Global.CurrentScene;

            var entities = Context<Default>.AllOf<CursorImageComponent>().GetEntities();
            if (entities.Length == 0)
                return;

            foreach (var e in entities)
            {

                e.Get<TransformComponent>().Position = Global.ScreenToWorld(Input.MousePosition);

                if (Input.LeftMousePressed)
                {
                    CollisionResult cr;
                    if (SceneColliderManager.CollidedWithBox(e, out cr))
                    {
                        if (e.Get<CursorImageComponent>().CurrentEntityChosen != null)
                        {
                            Entity prvEntity = e.Get<CursorImageComponent>().CurrentEntityChosen;
                            prvEntity.Remove<EntityCapturedComponent>();
                        }
                        Entity currentEntity = cr.OwnerEntity;
                        currentEntity.Add<EntityCapturedComponent>();      //allows entity to move
                        e.Get<CursorImageComponent>().CurrentEntityChosen = currentEntity;  //current entity to act on
                    }
                }
            }
        }
    }
}
