using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace mmGameEngine
{
    public class PanelComponent : RenderComponent
    {
      

        public Color Color;
        public Color BackgroundColor;
        public Color BorderColor;
        public bool HasBorder = true;

        private List<Entity> PanelEntities = new List<Entity>();
        private List<RenderComponent> PanelComponents = new List<RenderComponent>();   
        public bool RecalcPanelComponents = true;
        public PanelComponent(int _width, int _height, Color backgroundColor) 
        {
            BackgroundColor = backgroundColor;
            Width = _width;
            Height = _height;
            BorderColor = Colors.Black;
            RenderLayer = Global.UI_LAYER;
        }
        public PanelComponent(int _width, int _height)
        {
            BackgroundColor = Colors.CornflowerBlue;
            Width = _width;
            Height = _height;
            BorderColor = Colors.Black;
            RenderLayer = Global.UI_LAYER;
        }
        public override void Update()
        {
            base.Update();
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
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
            if (HasBorder)
                Global.CanvasDraw.DrawRoundedRectangle(Rectangle, 5, 5, BorderColor);

            foreach (RenderComponent comp in PanelComponents)
            {
                comp.Render();
            }


        }
        public void AddEntity(Entity comp)
        {
            // Transform = panel's TransformComponent
            // controlTransform = added component's TransformComponent
            // convert relative position to absolute for component within the panel
            // 

            PanelEntities.Add(comp);
        }
        public void RecalculateLayout()
        {
            Rectangle = new Rect(Transform.Position.X, Transform.Position.Y, Width, Height);
        }
    }
}
