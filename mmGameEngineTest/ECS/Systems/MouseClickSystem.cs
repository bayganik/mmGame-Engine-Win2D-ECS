using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class MouseClickSystem : IExecuteSystem
    {
        bool Dragging = false;
        float timerDelay = 500;
        float clickTimer;
        CardScene MyScene;
        public void Execute()
        {
            var tmpScene = (Scene)Global.CurrentScene;
            if (tmpScene.GetType().Name != "CardScene")
                return;

            MyScene = (CardScene)tmpScene;
            var entities = Context<Default>.AllOf<MouseComponent>().GetEntities();

            foreach (var mouseEntity in entities)
            {
                mouseEntity.Get<TransformComponent>().Position = Input.MousePosition;
                clickTimer += (float)Global.DeltaTime * 1000;

                if (Input.IsLeftMouseReleased())
                {
                    if (clickTimer < timerDelay)
                    {
                        clickTimer = 0;             //double click detected
                    }
                    clickTimer = 0;
                    if (Dragging)
                    {
                        Dragging = false;

                        CollisionResult cr;
                        if (!SceneColliderManager.CollidedWithBox(mouseEntity, out cr))
                            return;

                        Entity collidedEntity = cr.OwnerEntity;
                        //
                        // Dealt Card is released but was not put on a stack
                        //
                        if (collidedEntity.Tag == 80)
                        {
                            //
                            // Ace pile drop
                            //
                            MyScene.DropCardFromDrag2AceStack(collidedEntity);
                            return;                     //ace pile stack
                        }
                        if ((collidedEntity.Tag >= 1) && (collidedEntity.Tag <= 7))
                        {
                            //
                            // Play pile drop
                            //
                            MyScene.DropCardFromDrag2PlayStack(collidedEntity);
                            return;
                        }
                        //
                        // mouse released but not on Ace or Play area, return card to its place
                        //
                        MyScene.ReturnCardFromDrag2Stack();
                        return;                     //drap disp stack (release of mouse outside of play area)
                    }
                    //Input.LeftMouseReleased = false;

                }
                if (Input.IsLeftMousePressed())
                {
                    //
                    // if game over, don't allow movement
                    //
                    if (Global.StateOfGame == GameState.GameEnd)
                        return;

                    CheckCollisionResults(mouseEntity);
                }
            }
        }
        /*
         *
         * Pile with tag = 90 dealt pile displayed
         * Pile with tag = 80 are Ace piles
         * Pile with tag = 70 face down cards to deal
         * Pile with tags 1 - 7 are play stacks
         *       
         */
        private void CheckCollisionResults(Entity entity)
        {
            CollisionResult cr;
            if (!SceneColliderManager.CollidedWithBox(entity, out cr))
                return;
            //-----------------------------------------
            // we have clicked on a box collider
            //-----------------------------------------
            entity.Get<MouseComponent>().CurrentEntityClicked = cr.OwnerEntity;

            Dragging = false;
            switch (cr.OwnerEntity.Tag)
            {
                case 80:
                    MyScene.AcePileCard2Drag(cr.OwnerEntity);
                    Dragging = true;
                    break;
                case 70:
                    MyScene.DealtCard2Drag(cr.OwnerEntity);
                    Dragging = true;
                    break;
                case 90:
                    MyScene.DealCard2Disp(cr.OwnerEntity);           //from face down deal to face up
                    break;
                case int n when (n >= 1 && n <= 7):
                    Entity cardEntity = MyScene.FindCardInPlayPile(cr.OwnerEntity, Input.MousePosition);
                    if (cardEntity == null)
                        return;
                    //
                    // We have card, drag it + all others under it
                    //
                    Dragging = true;
                    MyScene.TakeCards2Drag(cardEntity);
                    break;
            }
        }
    }
}
