using Entitas;
using mmGameEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngineTest
{
    public class ExplosionComponent : Component
    {
        /*
         * Attached to an entity that is being dragged across scene
         */
        public Entity EntityOrig;                   //Stack Entity cards came from

        public ExplosionComponent()
        {

        }
    }
}
