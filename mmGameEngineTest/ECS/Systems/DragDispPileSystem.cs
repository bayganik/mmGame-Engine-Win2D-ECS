using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class DragDispPileSystem : IExecuteSystem
    {
        CardScene MyScene;
        Vector2 fanOutDistannce;
        Vector2 PrevMouse;
        Vector2 CurrentMouse = Vector2.Zero;
        public void Execute()
        {
            var tmpScene = (Scene)Global.CurrentScene;
            if (tmpScene.GetType().Name != "CardScene")
                return;

            MyScene = (CardScene)tmpScene;
            var entities = Context<Default>.AllOf<DragComponent>().GetEntities();
            if (entities.Length == 0) return;

            foreach (var entity in entities)
            {
                //
                // We have a DragDisp entity (holds all cards entities)
                //
                CardPileComponent sc = entity.GetComponent<CardPileComponent>();
                if (sc != null)
                {
                    if (sc.CardsInPile.Count <= 0)
                        return;                         //no cards to drag
                }

                var _mouseCollider = entity.GetComponent<BoxCollider>();
                PrevMouse = CurrentMouse;
                CurrentMouse = Input.MousePosition;
                //
                // Current location of the mouse used for the hand icon
                //
                entity.Get<TransformComponent>().Position = CurrentMouse;

                Entity lastCardonStack = sc.CardsInPile.LastOrDefault();
                //
                // Display of stack by fan out direction
                //
                switch (sc.FannedDirection)
                {
                    case 0:
                        fanOutDistannce = Vector2.Zero;
                        break;
                    case 1:
                        fanOutDistannce = new Vector2(30f, 0);
                        break;
                    case 2:
                        fanOutDistannce = new Vector2(-30f, 0);
                        break;
                    case 3:
                        fanOutDistannce = new Vector2(0, -30f);
                        break;
                    case 4:
                        fanOutDistannce = new Vector2(0, 30f);
                        break;

                }
                //
                // All cards are Entities in this stack
                //
                int ind = 0;                            //cars number in stack
                for (int i = 0; i < sc.CardsInPile.Count; i++)
                {
                    Entity cardEntity = sc.CardsInPile[i];
                    cardEntity.Get<TransformComponent>().Enabled = true;
                    cardEntity.Get<TransformComponent>().Position = entity.Get<TransformComponent>().Position + fanOutDistannce * ind;
                    //
                    // Get the sprite (face/back)
                    //
                    //cardEntity.GetComponent<CardComponent>().RenderLayer = (ind * 1000);
                    //cardEntity.GetComponent<CardComponent>().RenderLayer = Global.CURSOR_LAYER - ind;
                    cardEntity.GetComponent<CardComponent>().RenderLayer = Global.CURSOR_LAYER - ind;
                    ind += 1;
                }
            }
        }


    }
}
