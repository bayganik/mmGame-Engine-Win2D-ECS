using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;

namespace mmGameEngine
{
    public class LabelComponent : RenderComponent
    {
        // Font names:
        //
        //      "Old English Text MT"
        //      "Arial"
        //      "Arial Black"
        //      "Calibri"
        //      "Algerian"
        //
        public Color TextColor = Colors.Black;
        public Color BackgroundColor;
        public Color BorderColor = Colors.White;
        public GameUIType UIComponentType = GameUIType.Button;
        public CanvasTextFormat TextFormat = new CanvasTextFormat();
        public double StringPadding = 10;


        private CanvasTextLayout TextLayout;
        private Vector2 TextLayoutPosition;
        private Rect StringRect;

        private string _text;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                SetTextLayOut();
            }
        }
        int _fontSize;
        /// <summary>
        /// Text to display using default font
        /// </summary>
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                SetTextLayOut();
            }
        }
        string _fontName;
        /// <summary>
        /// Text to display using default font
        /// </summary>
        public string FontName
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;
                SetTextLayOut();
            }
        }
        public LabelComponent(string _content, int _width, int _height)
        {
            Enabled = true;
            Width = _width;
            Height = _height;
            _text = _content;
            _fontName = "Arial";
            _fontSize = 14;
            RenderLayer = Global.UI_LAYER;
            SetTextLayOut();
            //RecalculateLayout();
        }
        private void SetTextLayOut()
        {
            TextFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                FontFamily = _fontName,
                FontSize = _fontSize,
                WordWrapping = CanvasWordWrapping.Wrap,
                VerticalAlignment = CanvasVerticalAlignment.Center          //best choice

            };
            if (string.IsNullOrEmpty(_text)) _text = string.Empty;
            TextLayout = new CanvasTextLayout(Global.CanvasDeviceInUse, _text, TextFormat, 0, 0);
            
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
        }
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            Global.CanvasDraw.DrawText(_text, StringRect, TextColor, TextFormat);
        }


        public void RecalculateLayout()
        {
            StringRect = new Rect(Transform.Position.X + StringPadding, 
                                  Transform.Position.Y + StringPadding, 
                                  Width + StringPadding, 
                                  Height - StringPadding * 2);
            TextLayoutPosition = new Vector2(Transform.Position.X + (Width - (float)TextLayout.LayoutBounds.Width) / 2, Transform.Position.Y + (Height - (float)TextLayout.LayoutBounds.Height) / 2);
        }
    }
}
