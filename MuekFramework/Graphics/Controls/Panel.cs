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
    public Muek.Margin Margin { get; set; } = new(5);
    public Muek.Orientation Orientation { get; set; } = Muek.Orientation.Horizontal;
    public Muek.MuekColor Color { get; set; }
    protected Muek.MuekColor RenderColor { get; set; }
    public Muek.MuekColor HoverColor { get; set; }
    public Muek.MuekColor BorderColor { get; set; } = Muek.MuekColors.Transparent;
    public Vector2 BorderRadius { get; set; } = Vector2.Zero;
    public float BorderThickness { get; set; }
    public int Opacity { get; set; } = 255;
    public int RenderLayer { get; set; }
    public float AnimationSpeed { get; set; } = .05f;
    public Vector2 HoverScale { get; set; } = new(1f, 1f);
    public bool IsAnimationDisabled { get; set; } = true;
    public Muek.ContentPosition ContentPosition { get; set; }
    public bool ClipChildren { get; set; } = true;
    public List<IControl>? Children { get; set; } = new();
    public IControl? Parent { get; set; }
    public event Muek.RenderDelegate? OnRender;
    public event Muek.InputDelegate? OnInput;
    public delegate Vector2 AlignDelegate(Vector2 offset,int index);
    public event AlignDelegate? OnAlign;
    public bool IsHovering { get; set; }
    protected bool IsPressed { get; set; }

    /// <summary>
    /// Init the panel and render it on the window.
    /// </summary>
    /// <param name="color">The color of the panel.</param>
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
            c.Save();
            if(ClipChildren)
                c.ClipRect(new SKRect(
                    Position.X - Size.X * (Scale.X - 1) / 2 + Margin.Left - BorderThickness,
                    Position.Y - Size.Y * (Scale.Y - 1) / 2 + Margin.Top - BorderThickness,
                    Position.X - Size.X * (Scale.X - 1) / 2 + Size.X * Scale.X + Margin.Left +  BorderThickness,
                    Position.Y - Size.Y * (Scale.Y - 1) / 2 + Size.Y * Scale.Y + Margin.Top + BorderThickness
                    ));
            
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
                    if(Parent == null || Parent.IsHovering) OnHover();
                    else OnLeave();
                }
                else OnLeave();
            }
            if (IsPressed) OnPointerPressed();
            AlignChildren();
            OnRender?.Invoke(c);
            c.Restore();
            //Keep hovered item on top
            {
                if (Children == null) return;
                foreach (var child in Children)
                {
                    if (child.RenderLayer != 1) continue;
                    OnRender -= child.Render();
                    OnRender += child.Render();
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

    //DO NOT CHANGE THIS IF THERE IS NO PROBLEM!
    protected void AlignChildren()
    {
        if (Children == null) return;
        foreach (var control in Children)
        {
            Vector2 offset = default;
            if (Children.IndexOf(control) > 0)
            {
                var c = Children[Children.IndexOf(control) - 1];
                if (Orientation == Muek.Orientation.Horizontal)
                    offset = new Vector2(c.Position.X + c.Size.X + c.Margin.Right - Position.X - Margin.Left + c.Margin.Right,0);
                if (Orientation == Muek.Orientation.Vertical)
                    offset = new Vector2(0, c.Position.Y + c.Size.Y + c.Margin.Bottom - Position.Y - Margin.Top + c.Margin.Bottom);
            }

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

                switch (ContentPosition)
                {
                    case Muek.ContentPosition.TopLeft:
                        break;
                    case Muek.ContentPosition.Top:
                        if(Orientation == Muek.Orientation.Horizontal)
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X / 2 - childrenSize.X / 2;
                            }
                        if (Orientation == Muek.Orientation.Vertical) 
                            offset.X += Size.X / 2 - control.Size.X / 2;
                        break;
                    case Muek.ContentPosition.TopRight:
                        if(Orientation == Muek.Orientation.Horizontal)
                            if(Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X - childrenSize.X;
                            }
                        if (Orientation == Muek.Orientation.Vertical) 
                            offset.X += Size.X - control.Size.X - control.Margin.Right - control.Margin.Left;
                        break;
                    case Muek.ContentPosition.Left:
                        if (Orientation == Muek.Orientation.Horizontal)
                            offset.Y += Size.Y / 2 - control.Size.Y / 2 - control.Margin.Top;
                        if(Orientation == Muek.Orientation.Vertical)
                            if(Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y / 2 - childrenSize.Y / 2;
                            }
                        break;
                    case Muek.ContentPosition.Center:
                        if (Orientation == Muek.Orientation.Horizontal)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X / 2 - childrenSize.X / 2;
                            }
                            offset.Y += Size.Y / 2 - control.Size.Y / 2 - control.Margin.Top;
                        }
                        if(Orientation == Muek.Orientation.Vertical)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y / 2 - childrenSize.Y / 2;
                            }
                            offset.X += Size.X / 2 - control.Size.X / 2 - control.Margin.Left;
                        }
                        break;
                    case Muek.ContentPosition.Right:
                        if (Orientation == Muek.Orientation.Horizontal)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X - childrenSize.X;
                            }
                            offset.Y += Size.Y / 2 - control.Size.Y / 2 - control.Margin.Top;
                        }
                        if(Orientation == Muek.Orientation.Vertical)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y / 2 - childrenSize.Y / 2;
                            }
                            offset.X += Size.X - control.Size.X - control.Margin.Right - control.Margin.Left;
                        }
                        break;
                    case Muek.ContentPosition.BottomLeft:
                        if (Orientation == Muek.Orientation.Horizontal)
                            offset.Y += Size.Y - control.Size.Y - control.Margin.Top - control.Margin.Bottom;
                        if(Orientation == Muek.Orientation.Vertical)
                            if(Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y - childrenSize.Y;
                            }
                        break;
                    case Muek.ContentPosition.Bottom:
                        if (Orientation == Muek.Orientation.Horizontal)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X / 2 - childrenSize.X / 2;
                            }

                            offset.Y += Size.Y - control.Size.Y - control.Margin.Top - control.Margin.Bottom;
                        }
                        if(Orientation == Muek.Orientation.Vertical)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y - childrenSize.Y;
                            }
                            offset.X += Size.X / 2 - control.Size.X / 2 - control.Margin.Left;
                        }
                        break;
                    case Muek.ContentPosition.BottomRight:
                        if (Orientation == Muek.Orientation.Horizontal)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.X += Size.X - childrenSize.X;
                            }

                            offset.Y += Size.Y - control.Size.Y - control.Margin.Top - control.Margin.Bottom;
                        }
                        if(Orientation == Muek.Orientation.Vertical)
                        {
                            if (Children.IndexOf(control) == 0)
                            {
                                offset.Y += Size.Y - childrenSize.Y;
                            }
                            offset.X += Size.X - control.Size.X - control.Margin.Right - control.Margin.Left;
                        }
                        break;
                }
            }
            Vector2? o = OnAlign?.Invoke(offset,Children.IndexOf(control));
            if(o == null) control.Position = new Vector2(
                Position.X + Margin.Left + offset.X,
                Position.Y + Margin.Top + offset.Y);
            else
            {
                control.Position = new Vector2(
                    Position.X + ((Vector2)o).X + Margin.Left,
                    Position.Y + ((Vector2)o).Y + Margin.Top);
            }
            
        }
    }

    /// <summary>
    /// <para>Add a new control as a child of this.</para>
    /// <para>When add text as child,use <see cref="AddText(Text)"/> or <see cref="AddText(string,float,Muek.ContentPosition,Muek.MuekColor?)"/> instead.</para>
    /// </summary>
    /// <param name="control">The new control.</param>
    public Panel Add(IControl control)
    {
        Children?.Add(control);
        control.Parent = this;
        OnRender += control.Render();
        OnInput += control.Input();
        return this;
    }

    /// <summary>
    /// Add multiple new controls as children of this.
    /// </summary>
    /// <param name="controls">The list of new controls.</param>
    /// <seealso cref="Add(IControl)"/>
    public Panel Add(List<IControl> controls)
    {
        foreach (var c in controls)
        {
            Add(c);
        }
        return this;
    }

    /// <summary>
    /// Add custom text to this control.
    /// </summary>
    /// <param name="text">The text created.</param>
    public Panel AddText(Text text)
    {
        if (text.Size.X < 0 || text.Size.Y < 0)
        {
            text.Size = Size;
        }

        Add(text);
        return this;
    }

    /// <summary>
    /// Add text to this control.
    /// </summary>
    /// <param name="content">The content of the text.</param>
    /// <param name="fontSize">The font size of the text.Default as 12</param>
    /// <param name="position">The position of the text.Default as <see cref="Muek.ContentPosition.Center"/>. </param>
    /// <param name="color">The color of the text.Default as <see cref="Muek.MuekColors.Black"/></param>
    public Panel AddText(string content, float fontSize = 12,
        Muek.ContentPosition position = Muek.ContentPosition.Center, Muek.MuekColor? color = null)
    {
        var text = new Text(content, (int)Size.X, (int)Size.Y)
        {
            ContentPosition = position,
            FontSize = fontSize,
        };
        if (color != null) text.Color = color;
        Add(text);
        return this;
    }

    public void Remove(IControl control)
    {
        Children?.Remove(control);
        control.Parent = null;
        OnRender -= control.Render();
        OnInput -= control.Input();
    }

    public void Clear()
    {
        if (Children != null)
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