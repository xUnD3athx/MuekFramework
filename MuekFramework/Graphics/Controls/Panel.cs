using System.Drawing;
using System.Numerics;
using SDL3;
using SkiaSharp;

namespace MuekFramework.Graphics.Controls;

/// <summary>
/// Basic panel.
/// </summary>
public class Panel : IControl
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
    public Muek.Margin Margin { get; set; } = new(5, 5, 5, 5);
    public Muek.Orientation Orientation { get; set; } = Muek.Orientation.Vertical;
    public Muek.MuekColor Color { get; set; }
    protected Muek.MuekColor RenderColor { get; set; }
    public Muek.MuekColor HoverColor { get; set; }

    public Muek.MuekColor BorderColor { get; set; } = Muek.MuekColors.Transparent;
    public Vector2 BorderRadius { get; set; } = Vector2.Zero;
    public float BorderThickness { get; set; } = 0;
    public int Opacity { get; set; } = 255;
    public int RenderLayer { get; set; }
    public float AnimationSpeed { get; set; } = .05f;
    public Vector2 HoverScale { get; set; } = new Vector2(1f, 1f);
    public bool IsAnimationDisabled { get; set; } = true;
    public List<IControl> Children { get; set; } = new();
    public event Muek.RenderDelegate? OnRender;
    public event Muek.InputDelegate? OnInput;
    protected bool IsHovering { get; set; } = false;
    protected bool IsPressed { get; set; } = false;

    /// <summary>
    /// Init the panel and render it on the window.
    /// </summary>
    /// <param name="renderColor">The color of the panel.</param>
    /// <param name="width">The width of the panel.</param>
    /// <param name="height">The height of the panel.</param>
    /// <param name="x">The x position of the panel.Default value 0.</param>
    /// <param name="y">The y position of the panel.Default value 0.</param>
    public Panel(Muek.MuekColor color, int width, int height, int x = 0, int y = 0)
    {
        Size = new Vector2(width, height);
        Position = new Vector2(x, y);
        Color = color;
        HoverColor = Color;
        RenderColor = Color;
    }

    public Muek.RenderDelegate Render()
    {
        return (c) =>
        {
            SKPaint color, borderColor;
            //Set Props
            {
                color = new SKPaint()
                    { Color = new SKColor(RenderColor.Red, RenderColor.Green, RenderColor.Blue, (byte)Opacity) };

                borderColor = new SKPaint()
                {
                    Color = new SKColor(BorderColor.Red, BorderColor.Green, BorderColor.Blue,
                        (byte)Opacity)
                };
                if (BorderThickness == 0)
                {
                    borderColor.Color = SKColors.Transparent;
                }

                borderColor.StrokeWidth = BorderThickness;
                borderColor.IsStroke = true;
                color.IsAntialias = true;
                borderColor.IsAntialias = true;
            }
            //Render
            {
                //Basic Render
                c.DrawRoundRect(
                    Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left,
                    Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top,
                    Size.X * Scale.X,
                    Size.Y * Scale.Y,
                    BorderRadius.X,
                    BorderRadius.Y,
                    color
                );
                //Border Render
                c.DrawRoundRect(
                    Position.X - Size.X * (Scale.X - 1) / 2 - BorderThickness / 2 + Margin.Left + 1,
                    Position.Y - Size.Y * (Scale.Y - 1) / 2 - BorderThickness / 2 + Margin.Top + 1,
                    Size.X * Scale.X + BorderThickness - 2,
                    Size.Y * Scale.Y + BorderThickness - 2,
                    BorderRadius.X + BorderThickness / 2,
                    BorderRadius.Y + BorderThickness / 2,
                    borderColor
                );
            }
            //Hover Event
            {
                SDL.GetMouseState(out var mousePosX, out var mousePosY);
                if (mousePosX > Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left &&
                    mousePosX < Position.X - Size.X * (Scale.X - 1) / 2 + Size.X * Scale.X + Margin.Left &&
                    mousePosY > Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top &&
                    mousePosY < Position.Y - Size.Y * (Scale.Y - 1) / 2 + Size.Y * Scale.Y + Margin.Top)
                {
                    OnHover();
                }
                else OnLeave();
            }
            if (IsPressed) OnPointerPressed();
            OnRender?.Invoke(c);

            //Keep hovered item on top
            {
                foreach (var child in Children)
                {
                    if (child.RenderLayer == 1)
                    {
                        OnRender -= child.Render();
                        OnRender += child.Render();
                    }
                }
            }
        };
    }

    public Muek.InputDelegate Input()
    {
        return (e) =>
        {
            if (e.Type == (uint)SDL.EventType.MouseButtonDown)
            {
                if (IsHovering) OnPointerClicked();
            }

            if (e.Type == (uint)SDL.EventType.MouseButtonUp)
            {
                if (IsPressed) OnPointerReleased();
            }

            OnInput?.Invoke(e);
        };
    }

    /// <summary>
    /// When mouse pointer is over the control.
    /// </summary>
    protected virtual void OnHover()
    {
        IsHovering = true;
        RenderLayer = 1;
        var targetColor = HoverColor;
        var targetScale = HoverScale;
        TransitionTo(targetColor, AnimationSpeed);
        TransitionTo(targetScale, AnimationSpeed);
    }

    /// <summary>
    /// When mouse pointer is not over the control.
    /// </summary>
    protected virtual void OnLeave()
    {
        IsHovering = false;
        RenderLayer = 0;
        var targetColor = Color;
        var targetScale = Vector2.One;
        TransitionTo(targetColor, AnimationSpeed);
        TransitionTo(targetScale, AnimationSpeed);
    }

    /// <summary>
    /// When clicked the control.
    /// </summary>
    protected virtual void OnPointerClicked()
    {
        IsPressed = true;
    }

    /// <summary>
    /// When pressed the control.
    /// </summary>
    protected virtual void OnPointerPressed()
    {
    }

    /// <summary>
    /// When released the control.
    /// </summary>
    protected virtual void OnPointerReleased()
    {
        IsPressed = false;
    }

    /// <summary>
    /// <para>Add a new control as a child of this.</para>
    /// <para>When add text as child,use <see cref="AddText"/> instead.</para>
    /// </summary>
    /// <param name="control">The new control.</param>
    public void Add(IControl control)
    {
        Children.Add(control);
        Vector2 offset = default;
        if (Children.Count > 1)
        {
            var c = Children[Children.IndexOf(control) - 1];
            if (Orientation == Muek.Orientation.Vertical)
                offset = new Vector2(c.Position.X + c.Size.X + c.Margin.Right - Position.X - Margin.Left, 0);
            if (Orientation == Muek.Orientation.Horizontal)
                offset = new Vector2(0, c.Position.Y + c.Size.Y + c.Margin.Bottom - Position.Y - Margin.Top);
        }

        control.Position = new Vector2(
            control.Position.X + Position.X + offset.X + Margin.Left,
            control.Position.Y + Position.Y + offset.Y + Margin.Top);

        OnRender += control.Render();
        OnInput += control.Input();
    }

    /// <summary>
    /// Add multiple new controls as children of this.
    /// </summary>
    /// <param name="controls">The list of new controls.</param>
    /// <seealso cref="Add(IControl)"/>
    public void Add(List<IControl> controls)
    {
        foreach (var c in controls)
        {
            Add(c);
        }
    }

    /// <summary>
    /// Add custom text to this control.
    /// </summary>
    /// <param name="text">The text created.</param>
    public void AddText(Text text)
    {
        if (text.Size.X < 0 || text.Size.Y < 0)
        {
            text.Size = Size;
        }

        Add(text);
    }

    /// <summary>
    /// Add text to this control.
    /// </summary>
    /// <param name="content">The content of the text.</param>
    /// <param name="fontSize">The font size of the text.Default as 12</param>
    /// <param name="position">The position of the text.Default as <see cref="Muek.TextPosition.Center"/>. </param>
    /// <param name="color">The color of the text.Default as <see cref="Muek.MuekColors.Black"/></param>
    public void AddText(string content, float fontSize = 12,
        Muek.TextPosition position = Muek.TextPosition.Center, Muek.MuekColor? color = null)
    {
        var text = new Text(content, (int)Size.X, (int)Size.Y)
        {
            TextPosition = position,
            FontSize = fontSize,
        };
        if (color != null) text.Color = color;
        Add(text);
    }

    public void Remove(IControl control)
    {
        Children.Remove(control);
        OnRender -= control.Render();
        OnInput -= control.Input();
    }

    public void Clear()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            Remove(Children[i]);
        }
    }

    public void TransitionTo(Vector2 targetScale, float animationSpeed = .5f)
    {
        Scale = !IsAnimationDisabled ? Vector2.Lerp(Scale, targetScale, animationSpeed) : targetScale;
    }

    public void TransitionTo(Muek.MuekColor targetColor, float animationSpeed = .5f)
    {
        if (!IsAnimationDisabled)
        {
            RenderColor = new Muek.MuekColor(
                (byte)float.Lerp(RenderColor.Red, targetColor.Red, animationSpeed),
                (byte)float.Lerp(RenderColor.Green, targetColor.Green, animationSpeed),
                (byte)float.Lerp(RenderColor.Blue, targetColor.Blue, animationSpeed),
                (byte)float.Lerp(RenderColor.Alpha, targetColor.Alpha, animationSpeed));
        }
        else
        {
            RenderColor = targetColor;
        }
    }
}