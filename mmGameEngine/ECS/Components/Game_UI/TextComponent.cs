using Entitas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace mmGameEngine
{
    public class TextComponent : RenderComponent
    {

        // Font names:
        //
        //      "Old English Text MT"
        //      "Arial"
        //      "Arial Black"
        //      "Calibri"
        //      "Algerian"
        //
        public Windows.UI.Color TextColor;
        public CanvasTextFormat TextFormat = new CanvasTextFormat();
        public CanvasTextLayout TextLayout;
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                SetTextLayout();
            }
        }

        string _text;
        public string Content
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                SetTextLayout();
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
                SetTextLayout();
            }
        }
        public TextComponent()
        {
            _text = "";
            _fontSize = 14;
            Init();
        }
        public TextComponent(string _content)
        {
            _text = _content;
            _fontSize = 14;
            Init();
            RenderLayer = Global.UI_LAYER;
        }
        int _fontSize;
        
        void Init()
        {
            //Text = string.Empty;
            //Position = new Vector2(0, 0);
            TextColor = Colors.Black;
            _fontName = "Arial";
            TextFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                FontFamily = _fontName,
                FontSize = _fontSize,
                WordWrapping = CanvasWordWrapping.NoWrap,
                VerticalAlignment = CanvasVerticalAlignment.Center          //best choice

            };
            SetTextLayout();
        }
        public void SetNewFont(string _newFontName, int _newFontSize)
        {
            _fontName = _newFontName;
            _fontSize = _newFontSize;

            SetTextLayout();
        }
        public void SetTextLayout()
        {
            TextFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                FontFamily = _fontName,
                FontSize = _fontSize,
                WordWrapping = CanvasWordWrapping.NoWrap,
                VerticalAlignment = CanvasVerticalAlignment.Center          //best choice

            };
            TextLayout = new CanvasTextLayout(Global.CanvasDeviceInUse, Content, TextFormat, 0, 0);
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;
            if (!Enabled)
                return;

            Global.CanvasDraw.DrawTextLayout(TextLayout, Transform.Position, TextColor);
        }
    }
}
