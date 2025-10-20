using System.Drawing;
using SDL3;
using SkiaSharp;

namespace MuekFramework.Graphics;

public static class Muek
{
    public delegate void RenderDelegate(SKCanvas c);

    public delegate void InputDelegate(SDL.Event e);

    /// <summary>
    /// This is the margin struct.Used to set the margin value of a control.
    /// </summary>
    /// <param name="left">The left value of the margin.</param>
    /// <param name="top">The top value of the margin.</param>
    /// <param name="right">The right value of the margin.</param>
    /// <param name="bottom">The bottom value of the margin.</param>
    public struct Margin
    {
        public readonly float Left;
        public readonly float Right;
        public readonly float Top;
        public readonly float Bottom;
        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public Margin(float vertical, float horizontal)
        {
            Left = Right = vertical;
            Top = Bottom = horizontal;
        }

        public Margin(float margin)
        {
            Left = Right = Top = Bottom = margin;
        }
    }

    /// <summary>
    /// <para>When the orientation of a control was set to <see cref="Orientation.Vertical"/>,
    /// The children of the control will display from left to right.
    /// </para>
    /// <para>When the orientation of a control was set to <see cref="Orientation.Horizontal"/>,
    /// The children of the control will display from top to bottom.</para>
    /// </summary>
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public enum ContentPosition
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public class MuekColor
    {
        public readonly byte Red, Green, Blue, Alpha;

        /// <summary>
        /// <para>Create a color using RGB.</para>
        /// <para>If you want to use HSL,use <see cref="MuekColor.FromHsl"/> instead.</para>
        /// </summary>
        /// <param name="r">The red value of the color.From 0 to 255.</param>
        /// <param name="g">The green value of the color.From 0 to 255.</param>
        /// <param name="b">The blue value of the color.From 0 to 255.</param>
        /// <param name="a">The alpha value of the color.From 0 to 255.Default with value 255.</param>
        public MuekColor(byte r = 255, byte g = 255, byte b = 255, byte a = 255)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        /// <summary>
        /// Create a color using HSL.
        /// </summary>
        /// <param name="h">The hue value of the color.</param>
        /// <param name="s">The saturation value of the color.</param>
        /// <param name="l">The lightness value of the color.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <returns>Return the color created</returns>
        public MuekColor FromHsl(float h, float s, float l, byte a = 255)
        {
            return new MuekColor(
                SKColor.FromHsl(h, s, l, a).Red,
                SKColor.FromHsl(h, s, l, a).Green,
                SKColor.FromHsl(h, s, l, a).Blue,
                SKColor.FromHsl(h, s, l, a).Alpha
            );
            
        }
    }

    public struct MuekColors
    {
        public static readonly MuekColor White = new();
        public static readonly MuekColor Black = new(0, 0, 0);
        public static readonly MuekColor Transparent = new(255, 255, 255, 0);
        public static readonly MuekColor Grey = new(128, 128, 128);
        /// <summary>
        /// <font color="rgb(100,200,150)">Muek</font><br/>
        /// The theme color of muek.
        /// </summary>
        public static readonly MuekColor Muek = new(100, 200, 150);
        /// <summary>
        /// <font color="rgb(150,250,200)">LightMuek</font>
        /// </summary>
        public static readonly MuekColor LightMuek = new(150, 250, 200);
        /// <summary>
        /// <font color="rgb(50, 100, 75)">DarkMuek</font>
        /// </summary>
        public static readonly MuekColor DarkMuek = new(50, 100, 75);
        /// <summary>
        /// <font color="rgb(220, 60, 60)">MuekRed</font><br/>
        /// The theme red color of muek.Used for warnings or delete buttons.
        /// </summary>
        public static readonly MuekColor MuekRed =  new(220, 60, 60);
        /// <summary>
        /// <font color="rgb(250, 120, 120)">LightMuekRed</font>
        /// </summary>
        public static readonly MuekColor LightMuekRed =  new(250, 120, 120);
        /// <summary>
        /// <font color="rgb(100, 30, 20)">DarkMuekRed</font>
        /// </summary>
        public static readonly MuekColor DarkMuekRed = new(100, 30, 20);
        /// <summary>
        /// <font color="rgb(100, 140, 250)">MuekBlue</font><br/>
        /// The theme blue color of muek.
        /// </summary>
        public static readonly MuekColor MuekBlue = new(100, 140, 250);
        /// <summary>
        /// <font color="rgb(150, 200, 250)">LightMuekBlue</font>
        /// </summary>
        public static readonly MuekColor LightMuekBlue = new(150, 200, 250);
        /// <summary>
        /// <font color="rgb(20,50,100)">DarkMuekBlue</font>
        /// </summary>
        public static readonly MuekColor DarkMuekBlue = new(20,50,100);
        /// <summary>
        /// <font color="rgb(100,110,105)">MuekGery</font><br/>
        /// The theme grey color of muek.Used for disabled controls.
        /// </summary>
        public static readonly MuekColor MuekGrey = new(100,110,105);
    }
}