using System.Numerics;

namespace MuekFramework.Graphics.Controls;

public class Button : Panel
{
    public bool IsDisabled { get; set; }
    public Vector2 ClickedScale { get; set; } = new(0.95f, 0.95f);
    public Vector2 PressedScale { get; set; } = new(0.98f, 0.98f);
    public Muek.MuekColor PressedColor { get; set; }
    public Muek.MuekColor DisabledColor { get; set; } = Muek.MuekColors.MuekGrey;

    public delegate void ButtonDelegate();

    public event ButtonDelegate? OnClick;

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
        OnRender += _ =>
        {
            if (!IsDisabled) return;
            var targetColor = DisabledColor;
            TransitionTo(targetColor,AnimationSpeed);
        };
    }

    protected override void OnLeave()
    {
        if (!IsDisabled) base.OnLeave();
        else
        {
            IsHovering = false;
            RenderLayer = 0;
            var targetScale = Vector2.One;
            TransitionTo(targetScale, AnimationSpeed);
        }
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
        if (IsDisabled) return;
        base.OnPointerClicked();
        Scale = ClickedScale;
        OnClick?.Invoke();
    }
}