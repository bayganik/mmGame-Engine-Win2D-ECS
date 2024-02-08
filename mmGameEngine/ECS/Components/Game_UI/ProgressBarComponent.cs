using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;

using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Windows.Foundation;

namespace mmGameEngine
{
    public class ProgressBarComponent : RenderComponent
    {
        private int _currentValue;
        public int CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = Math.Min(Math.Max(value, _minValue), _maxValue); }
        }
        public Color Color100 = Colors.Green;
        public Color Color50 = Colors.Yellow;
        public Color Color30 = Colors.Red;

        public string Text;
        public CanvasTextFormat TextFormat = new CanvasTextFormat();
        public double StringPadding = 10;

        private int _minValue;
        private int _maxValue;
        private Vector2 TextLayoutPosition;
        private CanvasTextLayout TextLayout;

        private Rect StringRect;

        private CanvasTextLayout _textlayout;
        private Rect _sliderbarrect;
        private Vector2 _slidercirclecenterpoint;
        public ProgressBarComponent(int _width, int _height, string _text, int minValue, int maxValue, int startingValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;

            _currentValue = startingValue;

            Text = _text;
            _textlayout = new CanvasTextLayout(Global.CanvasDeviceInUse, _text, Global.DefaultFontNoWrap, 0, 0);

            //_sliderbarrect = new Rect(Transform.Position.X, Transform.Position.Y + _textlayout.LayoutBounds.Height + Global.Padding, Width, 10);

            Width = _width;
            Height = _height;
            TextLayout = new CanvasTextLayout(Global.CanvasDeviceInUse, Text, Global.DefaultFontNoWrap, 0, 0);
            RenderLayer = Global.UI_LAYER;
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

            var rectangleHeight = Height;

            var rect = new Windows.Foundation.Rect();
            rect.X = Transform.Position.X;
            rect.Width = Width;
            rect.Y = Transform.Position.Y - rectangleHeight / 2;
            rect.Height = rectangleHeight;

            int healthPercent = CurrentValue * 100 / _maxValue;

            ICanvasBrush brush;
            if (healthPercent > 50)
                brush = new CanvasSolidColorBrush(Global.CanvasDraw, Color100);
            else if (healthPercent <= 50)
                brush = new CanvasSolidColorBrush(Global.CanvasDraw, Color50);
            else
                brush = new CanvasSolidColorBrush(Global.CanvasDraw, Color30);


            Global.CanvasDraw.DrawRoundedRectangle(rect, 5, 5, brush);

            rect.Width = ((float)healthPercent / 100) * Width;

            Global.CanvasDraw.FillRoundedRectangle(rect, 5, 5, brush);

            if (!string.IsNullOrEmpty(Text))
                Global.CanvasDraw.DrawTextLayout(TextLayout, TextLayoutPosition, Colors.White);

        }
        public void RecalculateLayout()
        {

            TextLayoutPosition = new Vector2(Transform.Position.X, Transform.Position.Y - (Height * 2));
        }
    }
}
