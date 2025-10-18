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
    public struct Margin(float left, float top, float right, float bottom)
    {
        public readonly float Left = left;
        public readonly float Right = right;
        public readonly float Top = top;
        public readonly float Bottom = bottom;
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

    public enum TextPosition
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
        public readonly SKPaint Color;
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
            Color = new SKPaint() { Color = new SKColor(r, g, b, a) };
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
        /// The theme color of muek.
        /// </summary>
        public static readonly MuekColor Muek = new(100, 200, 150);
        public static readonly MuekColor LightMuek = new(150, 250, 200);
        public static readonly MuekColor DarkMuek = new(50, 100, 75);
        /// <summary>
        /// The theme red color of muek.Used for warnings or delete buttons.
        /// </summary>
        public static readonly MuekColor MuekRed =  new(220, 60, 60);
        public static readonly MuekColor LightMuekRed =  new(250, 120, 120);
        public static readonly MuekColor DarkMuekRed = new(100, 30, 20);
        /// <summary>
        /// The theme blue color of muek.
        /// </summary>
        public static readonly MuekColor MuekBlue = new(100, 140, 250);
        public static readonly MuekColor LightMuekBlue = new(150, 200, 250);
        public static readonly MuekColor DarkMuekBlue = new(20,50,100);
    }
}