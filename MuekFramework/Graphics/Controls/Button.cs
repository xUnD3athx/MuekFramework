using System.Numerics;
using SDL3;

namespace MuekFramework.Graphics.Controls;

public class Button : Panel
{
    public Vector2 ClickedScale { get; set; } = new Vector2(0.95f, 0.95f);
    public Vector2 PressedScale { get; set; } = new Vector2(0.98f, 0.98f);
    public Muek.MuekColor PressedColor { get; set; }

    public delegate void ButtonDelegate();

    public event ButtonDelegate OnClick;

    public Button(Muek.MuekColor color, int width, int height, int x = 0, int y = 0) : base(color, width, height, x, y)
    {
        Size = new Vector2(width, height);
        Position = new Vector2(x, y);
        PressedColor = Color;
        BorderRadius = new Vector2(4f, 4f);
        BorderColor = Muek.MuekColors.Grey;
        BorderThickness = 2;
        HoverScale = new Vector2(1.05f, 1.05f);
        IsAnimationDisabled = false;
        AnimationSpeed = .1f;
    }

    protected override void OnHover()
    {
        base.OnHover();
    }

    protected override void OnLeave()
    {
        base.OnLeave();
    }

    protected override void OnPointerPressed()
    {
        base.OnPointerPressed();
        var targetColor = PressedColor;
        var targetScale = PressedScale;
        TransitionTo(targetColor, AnimationSpeed);
        TransitionTo(targetScale, AnimationSpeed);
    }

    protected override void OnPointerReleased()
    {
        base.OnPointerReleased();
        var targetColor = Color;
        var targetScale = Vector2.One;
        TransitionTo(targetColor, AnimationSpeed);
        TransitionTo(targetScale, AnimationSpeed);
    }

    protected override void OnPointerClicked()
    {
        base.OnPointerClicked();
        Scale = ClickedScale;
        OnClick?.Invoke();
    }
}