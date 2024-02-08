using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmGameEngine;

namespace mmGameEngineTest
{
    public class DragComponent : Component
    {
        /*
         * Attached to an entity that is being dragged across scene
         */
        public Entity EntityOrig;                   //Stack Entity cards came from

        public DragComponent()
        {

        }
    }
}
