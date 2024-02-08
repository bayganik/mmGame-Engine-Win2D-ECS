using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace mmGameEngine
{
    public class TileComponent : RenderComponent
    {
        public CanvasBitmap Image { get; set; }
        public int SizeOfBlock;
        public bool ImageSet2BackgroundColor = false;
        public float ImageScale { get; set; } = 1.0f;
        public string Name { get; set; }
        public float Opacity { get; set; } = 1.0f;

        public float OffsetX { get; set; } = 0;
        public float OffsetY { get; set; } = 0;
        public Vector2 Offset => new Vector2(OffsetX, OffsetY);
        public bool HasBorder { get; set; } = false;
        public Color BorderColor = Colors.White;

        private Color SavedColor;
        private Color SavedTextColor;


        public Color TextColor = Colors.WhiteSmoke;             //color of text
        public Color BackgroundColor;                           //color of tile



        private CanvasTextLayout TextLayout;
        private Vector2 TextLayoutPosition;

        public Color MouseOverColor = Colors.LightGray;
        //
        //public event Action<SceneTile> OnClicked;
        //public event Action<bool> OnChanged;
        private bool MouseCurrentlyOverControl { get; set; }
        private string _text;
        public string Text
        {
            get => _text;
            set => SetTextLayOut(value);
        }

        Rect sourceRect;
        public TileComponent()
        {
            Enabled = true;

            Width = 1;
            Height = 1;
            BackgroundColor = Colors.Transparent;
            BorderColor = Colors.White;
            SavedColor = BackgroundColor;
            SavedTextColor = TextColor;
            Image = null;

            if (Image == null)
                sourceRect = new Rect(0, 0, Width, Height);
            else
                sourceRect = Image.Bounds;
        }
        public TileComponent(CanvasBitmap _image)
        {
            if (_image == null)
                return;

            Enabled = true;

            Width = (int)_image.Size.Width;
            Height = (int)_image.Size.Height;
            BackgroundColor = Colors.Transparent;
            BorderColor = Colors.White;
            SavedColor = BackgroundColor;
            SavedTextColor = TextColor;
            Image = _image;

            sourceRect = Image.Bounds;
        }
        public TileComponent(int _width, int _height, CanvasBitmap _image = null)
        {
            Enabled = true;

            Width = _width;
            Height = _height;
            BackgroundColor = Colors.Transparent;
            BorderColor = Colors.White;
            SavedColor = BackgroundColor;
            SavedTextColor = TextColor;
            Image = _image;

            if (Image == null)
                sourceRect = new Rect(0, 0, Width, Height);
            else
                sourceRect = Image.Bounds;
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

            if (Image != null)
            {
                if (ImageSet2BackgroundColor)
                {
                    //
                    // Entire block is background color
                    //
                    int size = (int)Image.SizeInPixels.Width * (int)Image.SizeInPixels.Height;
                    //int size = SizeOfBlock;
                    Color[] clr = new Color[size];
                    for (int i = 0; i < size; i++)
                        clr[i] = BackgroundColor;
                    Image.SetPixelColors(clr);
                }
                Global.CanvasDraw.DrawImage(Image, Rectangle, sourceRect, Opacity);
            }
            else
            {
                //
                // no image, fill rectangle
                //
                Global.CanvasDraw.FillRectangle(Rectangle, BackgroundColor);
            }


            if (HasBorder)
                DrawRectangle();

            if (!string.IsNullOrEmpty(_text))
                DrawText();
        }

        private void DrawRectangle()
        {
            Rect rect = new Rect(Rectangle.X, Rectangle.Y, Rectangle.Width - 1, Rectangle.Height - 1);
            //
            // Draw a border 
            //
            Global.CanvasDraw.DrawRectangle(rect, BorderColor);
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
            if (!string.IsNullOrEmpty(_text))
                TextLayoutPosition = new Vector2(Transform.Position.X + (Width - (float)TextLayout.LayoutBounds.Width) / 2, Transform.Position.Y + (Height - (float)TextLayout.LayoutBounds.Height) / 2);
            Rectangle = new Rect(Transform.Position.X, Transform.Position.Y, Width, Height);
        }


    }
}
