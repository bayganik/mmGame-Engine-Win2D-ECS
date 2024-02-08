using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class GameStateSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "CardScene")
                return;

            var entities = Context<Default>.AllOf<GameStateComponent>().GetEntities();

            foreach (var entity in entities)
            {
                GameStateComponent sc = entity.Get<GameStateComponent>();
                TextComponent txt = entity.Get<TextComponent>();
                txt.Content = sc.ScoreWords + sc.Score.ToString();


                if (Global.StateOfGame == GameState.GameEnd)
                {
                    MsgBoxComponent msb = entity.Get<MsgBoxComponent>();
                    msb.RenderLayer = 10000;

                    LabelComponent lbl = new LabelComponent("Press Play to continue the game.", 200, 40);

                    msb.MsgLabel = lbl;
                    //lbl = new Label("Score : " + sc.Score.ToString("###"));
                    //msb.AddMsg(lbl, new Vector2(15, 30));

                    entity.IsVisible = true;
                }
            }
        }
    }
}
