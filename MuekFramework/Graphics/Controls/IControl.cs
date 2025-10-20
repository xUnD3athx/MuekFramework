using System.Numerics;

namespace MuekFramework.Graphics.Controls;

/// <summary>
/// Basic control.Could be rendered on window.
/// </summary>
public interface IControl
{
    /// <summary>
    /// The position of the control.
    /// </summary>
    public Vector2 Position { get; set; }
    /// <summary>
    /// The size of the control.
    /// </summary>
    public Vector2 Size { get; set; }
    /// <summary>
    /// The scale of the control.This will not change the position and the size of the control.
    /// </summary>
    public Vector2 Scale { get; set; }
    /// <summary>
    /// The margin of the control.<seealso cref="Muek.Margin"/>
    /// </summary>
    public Muek.Margin Margin { get; set; }
    public int Opacity { get; set; }
    public int RenderLayer { get; set; }
    
    /// <summary>
    /// The children of the control.
    /// </summary>
    public List<IControl>? Children { get; set; }
    protected internal IControl? Parent { get; set; }
    public bool IsHovering { get; set; }
    public Muek.RenderDelegate Render();
    public Muek.InputDelegate Input();
    public event Muek.RenderDelegate? OnRender;
    public event Muek.InputDelegate? OnInput;
}