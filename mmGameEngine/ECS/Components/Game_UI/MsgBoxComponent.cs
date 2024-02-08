using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Microsoft.Graphics.Canvas;
using Windows.UI.Composition.Scenes;
using Entitas;

namespace mmGameEngine
{
    /*
     * Contains a LabelComponent & ButtonComponent
     * Allows for user to display an informative message with an action button
     * 
     */
    public class MsgBoxComponent : RenderComponent
    {
        private List<Entity> PanelEntities = new List<Entity>();
        private List<RenderComponent> PanelComponents = new List<RenderComponent>();

        public bool RecalcPanelComponents = true;

        public Color TextColor = Colors.Black;
        public Color BackgroundColor = Colors.Transparent;
        public Color BorderColor = Colors.White;
        public LabelComponent MsgLabel;
        public ButtonComponent MsgButton;
        public Color Color;

        private Entity lblEnt;
        private Entity btnEnt;

        public MsgBoxComponent(string _content, int _width, int _height)
        {
            Enabled = true;
            Width = _width + 30;
            Height = _height + 30;
            BorderColor = Colors.Black;

            lblEnt = Global.CreateSceneEntity(new Vector2(0, -10));
            lblEnt.Tag = -10000;
            MsgLabel = new LabelComponent(_content, _width, _height);
            lblEnt.Add(MsgLabel);
            PanelEntities.Add(lblEnt);

            //
            // Find middle of box
            // Width subtracts half of button width
            // Heigth add height of button
            //
            Vector2 btnLocation = new Vector2(Width / 2 - 15, Height / 2 + 30);
            btnEnt = Global.CreateSceneEntity(btnLocation);
            btnEnt.Tag = -10000;
            MsgButton = new ButtonComponent("OK", 40, 30);
            btnEnt.Add(MsgButton);
            PanelEntities.Add(btnEnt);
            RenderLayer = Global.UI_LAYER;
        }
        public override void Update()
        {
            base.Update();
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
            {
                Hide();
                return;
            }
            else
                Display();

            if (!Enabled)
                return;


            //
            // label positon may have changed
            //
            RecalculateLayout();
            //
            // Update all PanelEntities within this 
            //
            //-----------------------------------------------------------------------
            // Update game components (holding data) attached to entities
            //-----------------------------------------------------------------------
            Entity ent;
            PanelComponents = new List<RenderComponent>();
            int gMax = PanelEntities.Count;
            for (int i = 0; i < gMax; i++)
            {
                ent = PanelEntities[i];
                if (!ent.IsEnabled)
                    continue;

                Entitas.IComponent[] allComp = ent.GetComponents();         //all entity components
                foreach (Entitas.IComponent comp in allComp)
                {
                    if (comp is RenderComponent)
                    {
                        RenderComponent myComp = (RenderComponent)comp;
                        myComp.OwnerEntity = ent;                           //attach entity to component
                        myComp.Update();
                        if (RecalcPanelComponents)
                        {
                            myComp.Transform.Position = new Vector2(myComp.Transform.Position.X + Transform.Position.X, myComp.Transform.Position.Y + Transform.Position.Y);
                        }

                        PanelComponents.Add(myComp);
                    }
                }
            }
            RecalcPanelComponents = false;
        }
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;
            //
            // Scene will call this Render method 
            //

            Global.CanvasDraw.FillRectangle(Rectangle, BackgroundColor);
            Global.CanvasDraw.DrawRoundedRectangle(Rectangle, 5, 5, BorderColor);

            foreach (RenderComponent comp in PanelComponents)
            {
                comp.Render();
            }
        }
        public void Hide()
        {
            //
            // if MsgBox entity is not visible, then all others must be same
            //
            OwnerEntity.IsVisible = false;
            btnEnt.IsVisible = false;
            lblEnt.IsVisible = false;   
        }
        public void Display()
        {
            OwnerEntity.IsVisible = true;
            btnEnt.IsVisible = true;
            lblEnt.IsVisible = true;
        }
        public void Remove()
        {
            Global.DestroyGameEntity(OwnerEntity);
            Global.DestroyGameEntity(btnEnt);
            Global.DestroyGameEntity(lblEnt);

        }
        public void RecalculateLayout()
        {
            Rectangle = new Rect(Transform.Position.X, Transform.Position.Y, Width, Height);
        }

    }
}
