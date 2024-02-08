using Entitas;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;

namespace mmGameEngine
{
    /*
     * Main class for every game screen display.  This class :
     *		Creates & Holds all entities
     *			Game entities - all objects to play with
     *			Scene entities - all objects drawn on top of all other objects (mostly UI entities)
     *		Displays entities
     *			sorts entities by game then render
     *						   by scene then render
     */
    public class Scene : mmGameApp
    {
        //
        // ECS used for Entities (Game & Scene entities)
        //
        Entitas.Systems EntitySystems;
        List<Entity> GameEntities;
        List<Entity> SceneEntities;

        Vector2 pos = Vector2.Zero;
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        //                Scene holds all entities on screen
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        public Scene() 
        {
            Global.DebugRenderEnabled = false;

            //
            // List of all entities, List of entities destroyed
            //
            GameEntities = new List<Entity>();
            Global.GameEntityToDestroy = new Dictionary<Entity, bool>();
            SceneEntities = new List<Entity>();
            Global.SceneEntityToDestroy = new Dictionary<Entity, bool>();
        }
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        //               Attaching systems to act in this Scene
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        public void AddSystem(Entitas.ISystem _system)
        {
            EntitySystems.Add(_system);
        }
        internal void Begin() 
        {
            //
            // mmGame calls Begin() before a new scene window is created
            //
            SceneColliderManager.Initialize();
            //
            // start ECS (Game entities & components & systems)
            //
            //EntityContext = Entitas.Contexts.Default;
            Global.EntityContext = Entitas.Contexts.Default;
            //
            // Allow Entitas to init systems, auto collect matched systems, no manual Systems.Add(ISystem) required
            //
            //EntitySystems = new Entitas.Feature(null);               //allows all systems to be registered automatically
            EntitySystems = new Entitas.Systems();						//must add each system to the scene manually
            EntitySystems.Initialize();

            Global.Camera = Matrix3x2.Identity;
        }
        internal void End() 
        {
            //
            // mmGame calls End() when scene changes, before new scene is created
            //
            EntitySystems.TearDown();
            EntitySystems.ClearReactiveSystems();
            Global.EntityContext.DestroyAllEntities();
        }
        public virtual async Task Process()
        {
            //
            // You must override this method as in your scene
            //
        }
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        //               Update components in Scene
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        internal void Update() 
        {
            //
            // Find all Entities (Game & UI)
            //
            GameEntities = Global.EntityContext.GetEntities().ToList();

            //-----------------------------------------------------------------------
            // Update game components (holding data) attached to entities
            //-----------------------------------------------------------------------
            Entity ent;
            int gMax = GameEntities.Count;
            for (int i = 0; i < gMax; i++)
            {
                ent = GameEntities[i];
                if (!ent.IsEnabled)
                    continue;

                Entitas.IComponent[] allComp = ent.GetComponents();         //all entity components
                foreach (Entitas.IComponent comp in allComp)
                {
                    {
                        Component myComp = (Component)comp;
                        myComp.OwnerEntity = ent;                           //attach entity to component
                        myComp.Update();
                    }
                }
            }
            //-----------------------------------------------------------------------
            // Execute systems. All logic using components data
            //-----------------------------------------------------------------------
            EntitySystems.Execute();					//run IExecuteSystems
        }
        internal void Render()
        {
            //-------------------------------------------------------------------------------
            // Get all RenderComponent, sort them, low -> high
            //-------------------------------------------------------------------------------
            List<RenderComponent> ComponentsToRender = new List<RenderComponent>();
            //
            // Find all Entities (in case some were removed/added)
            //
            GameEntities = Global.EntityContext.GetEntities().Where(e => e.EntityType == 0).ToList();
            SceneEntities = Global.EntityContext.GetEntities().Where(e => e.EntityType == 1).ToList();
            //
            // Add all RenderComponent in a list to be displayed
            //
            int maxEnt = GameEntities.Count;
            for (int i = 0; i < maxEnt; i++)
            {
                Entity entity = GameEntities[i];
                if (!(entity.Get<TransformComponent>().Enabled && entity.IsVisible))
                    continue;

                //
                // Ask Entitas for all components attached to "ent" entity
                //
                Entitas.IComponent[] allComp = entity.GetComponents();       //this entity's component
                foreach (Entitas.IComponent comp in allComp)              //get the renderable ones
                {
                    if (comp is RenderComponent)
                    {
                        RenderComponent myComp = (RenderComponent)comp;
                        ComponentsToRender.Add(myComp);
                    }
                }
            }
            using (var ds = Global.SwapChain.CreateDrawingSession(Colors.CornflowerBlue))
            {
                Global.CanvasDraw = ds;

                using (var sb = ds.CreateSpriteBatch())
                {
                    Global.SpriteBatchDraw = sb;

                    //-------------------------------------------------------------------------------
                    //   RENDER ORDER  sorting (low to high) then render
                    //-------------------------------------------------------------------------------
                    List<RenderComponent> tmpRenderList = ComponentsToRender.OrderBy(e => e.RenderLayer).ToList();
                    foreach (RenderComponent myComp in tmpRenderList)
                    {
                        myComp.Render();                                    //call draw method
                    }
                }
                //-------------------------------------------------------------------------------
                //   UI  ENTITIES , they are drawn on top of all other game entities
                //-------------------------------------------------------------------------------
                foreach (Entity ent in SceneEntities)
                {
                    if (!ent.Get<TransformComponent>().Enabled)
                        continue;

                    if (!ent.IsVisible)
                        continue;

                    Entitas.IComponent[] allComp = ent.GetComponents();         //this entity's component
                    foreach (Entitas.IComponent comp in allComp)
                    {
                        if (comp is RenderComponent)
                        {
                            RenderComponent myComp = (RenderComponent)comp;
                            myComp.Render();                                    //call draw method
                        }
                    }
                }
            }
            Global.SwapChain.Present();
        }
        public void RemoveDeletedEntities()
        {
            //
            // Remove game entities, all children are included
            //      
            if (Global.GameEntityToDestroy.Count > 0)
            {
                foreach (KeyValuePair<Entity, bool> ent in Global.GameEntityToDestroy)
                {
                    ent.Key.RemoveAllComponents();
                    ent.Key.Destroy();                  //release entity automatically

                    //GameEntities.Remove(ent.Key);
                }
                Global.GameEntityToDestroy = new Dictionary<Entity, bool>();

            }
            //
            // Remove Scene specific entities (UI?)
            //
            if (Global.SceneEntityToDestroy.Count > 0)
            {
                foreach (KeyValuePair<Entity, bool> ent in Global.SceneEntityToDestroy)
                {
                    ent.Key.RemoveAllComponents();
                    ent.Key.Destroy();

                    //GameEntities.Remove(ent.Key);
                }
                Global.SceneEntityToDestroy = new Dictionary<Entity, bool>();

            }
            //
            // Do ECS update/cleanup
            //
            EntitySystems.Cleanup();
        }
        Matrix3x2 _inverseTransformMatrix = Matrix3x2.Identity;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 _pos)
        {
            //
            // following matrix when moving to 0,0 location
            //
            Matrix3x2 tempMat = Matrix3x2.CreateTranslation(Vector2.Zero);

            Global.Camera = Matrix3x2.Multiply(Global.Camera, tempMat);
            //
            // Get current Camera matrix and fine inverse tranform matrix
            //
            bool transform = Matrix3x2.Invert(Global.Camera, out _inverseTransformMatrix);
            //
            // determine the current position in this world
            //
            Vector2 tempVector = Vector2.Transform(_pos, _inverseTransformMatrix);
            return tempVector;
        }

    }
}
