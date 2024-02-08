using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using Microsoft.Graphics.Canvas;

namespace mmGameEngine
{
    public class ImageCursor
    {
        public ImageCursor(CanvasBitmap _image)
        {
            Entity CursorEnt = Global.CreateGameEntity(Vector2.Zero);
            CursorEnt.Name = "cursor";
            CursorEnt.Tag = 1000;
            //
            // Image to move with mouse
            //
            Sprite Spr = new Sprite(_image);
            Spr.RenderLayer = Global.CURSOR_LAYER;      //on top of everything
            CursorEnt.Add(Spr);
            //
            // Add small box collider if we click on anything
            //
            BoxCollider bxxx = new BoxCollider(8, 8);
            CursorEnt.Add(bxxx);
            CursorEnt.Add<CursorImageComponent>();

        }
    }
    public class CursorImageComponent : Component
    {
        public Entity CurrentEntityChosen;
        public CursorImageComponent()
        {

        }
    }
    public class EntityCapturedComponent : Component
    {
        //
        // Component indicates an entity that Left Mouse button was clicked on
        //
        public Entity CurrentEntityChosen;
        public EntityCapturedComponent()
        {

        }
    }
}
