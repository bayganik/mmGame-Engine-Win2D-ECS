using Entitas;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine
{
    public class MouseEntity
    {
        public MouseEntity()
        {
            Entity CursorEnt = Global.CreateGameEntity(Vector2.Zero);
            CursorEnt.Name = "cursor";
            CursorEnt.Tag = 1000;
            //
            // Add small box collider if we click on anything
            //
            BoxCollider bxxx = new BoxCollider(8, 8);
            CursorEnt.Add(bxxx);
            CursorEnt.Add<MouseComponent>();

        }
    }
}
