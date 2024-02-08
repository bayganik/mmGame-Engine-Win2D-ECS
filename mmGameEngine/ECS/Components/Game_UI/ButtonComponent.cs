using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition.Scenes;
using Windows.UI;

namespace mmGameEngine
{
    public class ButtonComponent: RenderComponent
    {
        private Color SavedColor;
        private Color SavedTextColor;

        public CanvasBitmap Image;
        public Color TextColor = Colors.WhiteSmoke;
        public Color BackgroundColor;
        public Color BorderColor = Colors.White;
        public GameUIType UIComponentType = GameUIType.Button;
        public Color MouseOverColor = Colors.LightGray;

        private CanvasTextLayout TextLayout;
        private Vector2 TextLayoutPosition;
        private bool MouseCurrentlyOverControl { get; set; }
        private string _text;
        public string Text
        {
            get => _text;
            set => SetTextLayOut(value);
        }
        public ButtonComponent(string _content, int _width, int _height)
        {
            Enabled = true;
            Width = _width;
            Height = _height;
            BackgroundColor = Colors.Gray;
            BorderColor = Colors.White;
            SavedColor = BackgroundColor;
            SavedTextColor = TextColor;
            Text = _content;
            RenderLayer = Global.UI_LAYER;
            //TextLayout = new CanvasTextLayout(device, Content, Statics.DefaultFontNoWrap, 0, 0);
            //RecalculateLayout();
        }
        private void SetTextLayOut(string _textIn)
        {
            _text = _textIn;
            TextLayout = new CanvasTextLayout(Global.CanvasDeviceInUse, _text, Global.DefaultFontNoWrap, 0, 0);
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

            RecalculateLayout();

            MouseMoveOver();
        }
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            DrawRectangle();
            DrawText();
        }

        private void DrawRectangle()
        {
            //
            // Draw Button area
            //
            Global.CanvasDraw.FillRectangle(Rectangle, SavedColor);

            if (Image != null)
                Global.CanvasDraw.DrawImage(Image, Rectangle);
            //
            // Draw a border 
            //
            Global.CanvasDraw.DrawRoundedRectangle(Rectangle, 5,5, BorderColor);
        }

        private void DrawText()
        {
            Global.CanvasDraw.DrawTextLayout(TextLayout, TextLayoutPosition, SavedTextColor);
        }


        public void MouseMoveOver()
        {
            if (!Enabled)
                return;

            Vector2 MousePos = Global.ScreenToWorld(Input.MousePosition);
            if (HitTest(new Point(MousePos.X, MousePos.Y)))
            {
                //HasFocus = true;
                SavedColor = MouseOverColor;
                SavedTextColor = Colors.Black;
                //
                // Test the last key for Left Mouse button
                //
                if (Input.LeftMousePressed)
                {
                    if (this.Enabled)
                    {
                        OnClick(this);
                        Input.LeftMousePressed = false;
                    }
                        
                }
            }
            else
            {
                SavedColor = BackgroundColor;
                SavedTextColor = TextColor;
            }


        }

        public void RecalculateLayout()
        {
            //base.RecalculateLayout();
            TextLayoutPosition = new Vector2(Transform.Position.X + (Width - (float)TextLayout.LayoutBounds.Width) / 2, Transform.Position.Y + (Height - (float)TextLayout.LayoutBounds.Height) / 2);
            Rectangle = new Rect(Transform.Position.X, Transform.Position.Y, Width, Height);
        }
    }
}
