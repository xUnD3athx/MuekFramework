using System.Numerics;
using SDL3;
using SkiaSharp;

namespace MuekFramework.Graphics.Controls;

public class ScrollPanel : Panel
{
    private float _scrollX;
    private float _scrollY;
    public Muek.MuekColor ScrollBarColor { get; set; } = Muek.MuekColors.Transparent;
    public Muek.MuekColor ScrollThumbColor { get; set; } = Muek.MuekColors.Black;
    public int ScrollBarOpacity { get; set; } = 50;
    public Vector2 ScrollThumbBorderRadius { get; set; } = new(1);
    public float ScrollBarWidth { get; set; } = 5f;
    public bool ShowScrollBar { get; set; } = true;
    
    public float ScrollX
    {
        get => _scrollX;
        set => _scrollX = float.Clamp(value, 0, 100);
    }

    public float ScrollY
    {
        get => _scrollY;
        set => _scrollY = float.Clamp(value, 0, 100);
    }

    public float ScrollSpeedX { get; set; } = 10f;
    public float ScrollSpeedY { get; set; } = 10f;
    public ScrollPanel(Muek.MuekColor color, int width, int height, int x = 0, int y = 0) : base(color, width, height, x, y)
    {
        Orientation = Muek.Orientation.Vertical;
        Scroll();
    }
    public ScrollPanel(int width, int height, int x = 0, int y = 0) : base(Muek.MuekColors.Black, width, height, x, y)
    {
        Opacity = 0;
        Orientation = Muek.Orientation.Vertical;
        Scroll();
    }

    private void Scroll()
    {
        OnAlign += (offset,index) =>
        {
            var childrenSize = GetChildrenSize();
            if (childrenSize.X < Size.X)
            {
                ScrollX = 0;
            }
            if (childrenSize.Y < Size.Y)
            {
                ScrollY = 0;
            }
            var offsetX = (childrenSize.X - Size.X) * _scrollX / 100f;
            var offsetY = (childrenSize.Y - Size.Y) * _scrollY / 100f;
            if(index == 0) offset -= new Vector2(offsetX, offsetY);
            return offset;
        };
        OnInput += e =>
        {
            if (e.Type == (uint)SDL.EventType.MouseWheel)
            {
                if (!IsHovering) return;
                if (e.Wheel.Y > 0)
                {
                    ScrollY -= e.Wheel.Y * ScrollSpeedY;
                }

                if (e.Wheel.Y < 0)
                {
                    ScrollY -= e.Wheel.Y * ScrollSpeedY;
                }

                if (e.Wheel.X > 0)
                {
                    ScrollX += e.Wheel.X * ScrollSpeedX;
                }

                if (e.Wheel.X < 0)
                {
                    ScrollX += e.Wheel.X * ScrollSpeedX;
                }
            }
        };
        OnTopRender += c =>
        {
            if (ShowScrollBar)
            {
                //Render Scroll Bar
                {
                    //X
                    c.DrawRoundRect(
                        Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left - BorderThickness,
                        Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top - BorderThickness + Size.Y -
                        ScrollBarWidth,
                        Size.X - ScrollBarWidth, ScrollBarWidth,
                        ScrollThumbBorderRadius.X, ScrollThumbBorderRadius.Y,
                        new SKPaint
                        {
                            Color = new SKColor(ScrollBarColor.Red, ScrollBarColor.Green, ScrollBarColor.Blue,
                                (byte)(ScrollBarOpacity / 255f * ScrollBarColor.Alpha))
                        });
                    //Y
                    c.DrawRoundRect(
                        Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left - BorderThickness + Size.X -
                        ScrollBarWidth,
                        ScrollBarWidth + Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top - BorderThickness,
                        ScrollBarWidth, Size.Y - ScrollBarWidth,
                        ScrollThumbBorderRadius.X, ScrollThumbBorderRadius.Y,
                        new SKPaint()
                        {
                            Color = new SKColor(ScrollBarColor.Red, ScrollBarColor.Green, ScrollBarColor.Blue,
                                (byte)(ScrollBarOpacity / 255f * ScrollBarColor.Alpha))
                        });
                }

                //Render Scroll Thumb
                {
                    var childrenSize = GetChildrenSize();
                    if (childrenSize.X > Size.X)
                    {
                        var scrollThumbXLength = float.Clamp(Size.X - (childrenSize.X - Size.X - ScrollBarWidth),
                            ScrollBarWidth, Size.X);
                        var scrollThumbXPosition = (Size.X - scrollThumbXLength - ScrollBarWidth) * _scrollX / 100f;
                        c.DrawRoundRect(
                            Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left - BorderThickness +
                            scrollThumbXPosition,
                            Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top - BorderThickness + Size.Y -
                            ScrollBarWidth,
                            scrollThumbXLength, ScrollBarWidth,
                            ScrollThumbBorderRadius.X, ScrollThumbBorderRadius.Y,
                            new SKPaint()
                            {
                                Color = new SKColor(ScrollThumbColor.Red, ScrollThumbColor.Green, ScrollThumbColor.Blue,
                                    (byte)(ScrollBarOpacity / 255f * ScrollThumbColor.Alpha))
                            });
                    }

                    if (childrenSize.Y > Size.Y)
                    {
                        var scrollThumbYLength = float.Clamp(Size.Y - (childrenSize.Y - Size.Y - ScrollBarWidth),
                            ScrollBarWidth, Size.Y);
                        var scrollThumbYPosition = ScrollBarWidth + (Size.Y - scrollThumbYLength) * _scrollY / 100f;
                        c.DrawRoundRect(
                            Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left - BorderThickness + Size.X -
                            ScrollBarWidth,
                            Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top - BorderThickness +
                            scrollThumbYPosition,
                            ScrollBarWidth, scrollThumbYLength,
                            ScrollThumbBorderRadius.X, ScrollThumbBorderRadius.Y,
                            new SKPaint
                            {
                                Color = new SKColor(ScrollThumbColor.Red, ScrollThumbColor.Green, ScrollThumbColor.Blue,
                                    (byte)(ScrollBarOpacity / 255f * ScrollThumbColor.Alpha))
                            });
                    }
                }
            }
        };
    }

    private Vector2 GetChildrenSize()
    {
        var childrenSize = Vector2.Zero;
        if (Children == null) return childrenSize;
        foreach (var child in Children)
        {
            switch (Orientation)
            {
                case Muek.Orientation.Horizontal:
                    childrenSize.X += child.Size.X;
                    break;
                case Muek.Orientation.Vertical:
                    childrenSize.Y += child.Size.Y;
                    break;
            }

            childrenSize.X += child.Margin.Left + child.Margin.Right;
            childrenSize.Y += child.Margin.Top + child.Margin.Bottom;
        }
        return childrenSize;
    }
}