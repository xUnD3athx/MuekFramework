using System.Numerics;

namespace MuekFramework.Graphics.Controls;

public class ToggleButton : Button
{
    public bool IsChecked { get; set; }
    public Muek.MuekColor CheckedColor { get; set; }
    public Vector2 CheckedScale { get; set; } = Vector2.One;
    public event ButtonDelegate? OnCheck;
    public event ButtonDelegate? OnUncheck;
    public ToggleButton(Muek.MuekColor color, int width, int height, int x = 0, int y = 0) : base(color, width, height, x, y)
    {
        CheckedColor = Color;
        OnRender += (c =>
        {
            if (IsChecked) OnChecked();
            else OnUnchecked();
        });
    }

    protected override void OnPointerClicked()
    {
        base.OnPointerClicked();
        IsChecked = !IsChecked;
    }

    protected override void OnLeave()
    {
        IsHovering = false;
        RenderLayer = 0;
        if (!IsChecked) base.OnLeave();
        
    }

    public void OnChecked()
    {
        var targetColor = CheckedColor;
        var targetScale = CheckedScale;
        TransitionTo(targetColor, AnimationSpeed);
        TransitionTo(targetScale, AnimationSpeed);
        OnCheck?.Invoke();
    }

    public void OnUnchecked()
    {
        OnUncheck?.Invoke();
    }
    
}