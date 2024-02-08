using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine
{
    public enum LoopMode
    {
        /// <summary>
        /// Play the sequence in a loop forever [A][B][C][A][B][C][A][B][C]...
        /// </summary>
        Loop,

        /// <summary>
        /// Play the sequence once [A][B][C] then pause and set time to 0 [A]
        /// </summary>
        Once,

        /// <summary>
        /// Plays back the animation once, [A][B][C]. When it reaches the end, it will keep playing the last frame and never stop playing
        /// </summary>
        ClampForever,

        /// <summary>
        /// Play the sequence in a ping pong loop forever [A][B][C][B][A][B][C][B]...
        /// </summary>
        PingPong,

        /// <summary>
        /// Play the sequence once forward then back to the start [A][B][C][B][A] then pause and set time to 0
        /// </summary>
        PingPongOnce
    }
    public enum TextSize
    {
        Default = 0,
        Small = 1
    }
    public enum GameState
    {
        
        GamePlay = 0,
        GameEnd = 1,
        Main_Menu = 2,
        Options_Menu = 3,
        Pause_Menu = 4
    }
    public enum GameUIType
    {
        Button = 0,
        Checkbox = 1,
        Lable = 2,
        Panel = 3,
        Textblock = 4,
        Textbox = 5,
        Scrollbar = 6,
        Slider = 7,
        Tile = 8
    }
    public enum Axis
    {
        X,
        Y
    }
    public enum Edge
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public enum DisplayType
    {
        Sprite = 0,
        AnimatedSprite = 1,
        TiledMap = 2,
        UI = 3,
        Text = 4,
        ScrollImage = 5,
        Card = 6,
        GenericRender = 7,
        SpriteSlice = 8
    }
    public enum Direction
    {
        Down,
        DownLeft,
        Left,
        UpLeft,
        Up,
        UpRight,
        Right,
        DownRight
    }

    public enum HorizontalAlign
    {
        Left,
        Center,
        Right
    }


    public enum VerticalAlign
    {
        Top,
        Center,
        Bottom
    }
    public enum CollidreShape
    {
        Box,
        Circle
    }
}
