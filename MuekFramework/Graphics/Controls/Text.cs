using System.Numerics;
using SkiaSharp;

namespace MuekFramework.Graphics.Controls;

public class Text : IControl
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    /// <summary>
    /// <para>The scale of the control.This will not change the position and the size of the control.</para>
    /// <para>When using scale in text control,only x value of the scale is enabled.</para>
    /// </summary>
    public Vector2 Scale { get; set; } = Vector2.One;

    public Muek.Margin Margin { get; set; } = new(5, 5, 5, 5);
    public Muek.TextPosition TextPosition { get; set; } = Muek.TextPosition.TopLeft;
    public int Opacity { get; set; } = 255;
    public int RenderLayer { get; set; }
    public List<IControl>? Children { get; set; } = null;
    public event Muek.RenderDelegate? OnRender;
    public event Muek.InputDelegate? OnInput;
    public string Content { get; set; }
    public float FontSize { get; set; } = 12F;
    public Muek.MuekColor Color { get; set; } = Muek.MuekColors.Black;

    public Text(string content, int width = -1, int height = -1, int x = 0, int y = 0)
    {
        Content = content;
        Position = new Vector2(x, y);
        Size = new Vector2(width, height);
    }

    public Muek.RenderDelegate Render()
    {
        return (c =>
        {
            SKTextAlign align = SKTextAlign.Left;
            var color = new SKPaint()
            {
                Color = new SKColor(Color.Red, Color.Green, Color.Blue, (byte)Opacity)
            };
            Vector2 textPosition;
            float vCenter = Size.X / 2;
            float hCenter = Size.Y / 2 - FontSize / 1.5f;
            float right = Size.X;
            float bottom = Size.Y - FontSize * 1.5f;
            switch (TextPosition)
            {
                case Muek.TextPosition.TopLeft:
                    textPosition = new(Position.X, Position.Y + FontSize);
                    align = SKTextAlign.Left;
                    break;
                case Muek.TextPosition.Top:
                    textPosition = new(Position.X + vCenter, Position.Y + FontSize);
                    align = SKTextAlign.Center;
                    break;
                case Muek.TextPosition.TopRight:
                    textPosition = new(Position.X + right, Position.Y + FontSize);
                    align = SKTextAlign.Right;
                    break;
                case Muek.TextPosition.Left:
                    textPosition = new(Position.X, Position.Y + FontSize + hCenter);
                    align = SKTextAlign.Left;
                    break;
                case Muek.TextPosition.Center:
                    textPosition = new(Position.X + vCenter, Position.Y + FontSize + hCenter);
                    align = SKTextAlign.Center;
                    break;
                case Muek.TextPosition.Right:
                    textPosition = new(Position.X + right, Position.Y + FontSize + hCenter);
                    align = SKTextAlign.Right;
                    break;
                case Muek.TextPosition.BottomLeft:
                    textPosition = new(Position.X, Position.Y + FontSize + bottom);
                    align = SKTextAlign.Left;
                    break;
                case Muek.TextPosition.Bottom:
                    textPosition = new(Position.X + vCenter, Position.Y + FontSize + bottom);
                    align = SKTextAlign.Center;
                    break;
                case Muek.TextPosition.BottomRight:
                    textPosition = new(Position.X + right, Position.Y + FontSize + bottom);
                    align = SKTextAlign.Right;
                    break;
                default:
                    textPosition = new(Position.X, Position.Y + FontSize);
                    align = SKTextAlign.Left;
                    break;
            }

            c.DrawText(Content,
                new SKPoint(textPosition.X, textPosition.Y),
                align,
                new SKFont(SKTypeface.FromFamilyName("Source Han Serif SC"), FontSize * Scale.X),
                color);
            OnRender?.Invoke(c);
        });
    }

    public Muek.InputDelegate Input()
    {
        return (e => { OnInput?.Invoke(e); });
    }
}