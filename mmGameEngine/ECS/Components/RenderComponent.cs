using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace mmGameEngine
{
    /*
	 * Same as component, with additional fields, a single delegate (used for buttons)
	 * and ability to invoke Render() method in our Scene class
	 * All components must first be added to an Entity.
	 */
    public delegate void ClickEventHandler(object obj);
    public class RenderComponent : Component
    {
        public event ClickEventHandler Click;

        public int RenderLayer;                     //render from low to high
        public Vector2 Origin;                      //override origin used for disp Sprite. Typically the center of it
        //
        // Following is typically used for UI components
        //
        public int Width = 0;
        public int Height = 0;
        public Rect Rectangle = new Rect();

        public RenderComponent()
        {
            Tag = 0;
            Enabled = true;
        }
        public virtual void Render()
        { }
        //
        // Test for UI to see if mouse is clicked on them
        //
        public bool HitTest(Windows.Foundation.Point point)
        {
            if (point.X < Transform.Position.X) { return false; }
            if (point.X >= Transform.Position.X + Width) { return false; }
            if (point.Y < Transform.Position.Y) { return false; }
            if (point.Y >= Transform.Position.Y + Height) { return false; }

            return true;
        }
        public void OnClick(object obj) { Click?.Invoke(obj); }
    }
}
