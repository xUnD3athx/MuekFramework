using System.Numerics;
using SDL3;

namespace MuekFramework.Graphics.Controls;

public class ScrollPanel : Panel
{
    private float _scrollX;
    private float _scrollY;

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
    public ScrollPanel(int width, int height, int x = 0, int y = 0) : base(Muek.MuekColors.Transparent, width, height, x, y)
    {
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
                    ScrollX += e.Wheel.X  * ScrollSpeedX;
                }

                if (e.Wheel.X < 0)
                {
                    ScrollX += e.Wheel.X  * ScrollSpeedX;
                }
            }
        };
        OnRender += c =>
        {
            var childrenSize = GetChildrenSize();
        };
    }

    private Vector2 GetChildrenSize()
    {
        var childrenSize = Vector2.Zero;
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