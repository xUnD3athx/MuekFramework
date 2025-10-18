using System.Numerics;
using MuekFramework.Graphics;
using MuekFramework.Graphics.Controls;

var count = 0;
var lockCount = false;

var customText = new Text("0 click")
{
    FontSize = 24f,
    TextPosition = Muek.TextPosition.Center
};

var window = new MuekWindow("MuekFramework Sample", 1000, 800);
var mainPanel = new Panel(Muek.MuekColors.White, 800, 600, 100, 100)
{
    BorderRadius = new Vector2(10F),
    IsAnimationDisabled = false,
    AnimationSpeed = .05F,
    HoverScale = new Vector2(1.05F)
};
window.Add(mainPanel);
var helloButton = new Button(Muek.MuekColors.Muek, 200, 200)
{
    HoverColor = Muek.MuekColors.LightMuek,
    PressedColor = Muek.MuekColors.DarkMuek,
};
mainPanel.Add(helloButton);
helloButton.AddText("Hello", 24F);
helloButton.OnClick += () =>
{
    if (count == 0)
    {
        helloButton.Clear();
        helloButton.AddText(customText);
    }
    if (!lockCount) customText.Content = $"{++count} clicked";
};
var lockButton = new ToggleButton(Muek.MuekColors.MuekBlue, 100, 100)
{
    HoverColor = Muek.MuekColors.LightMuekBlue,
    PressedColor = Muek.MuekColors.DarkMuekBlue,
    CheckedColor = Muek.MuekColors.MuekRed
};
mainPanel.Add(lockButton);
lockButton.AddText("Lock", 24F);
lockButton.OnCheck += () =>
{
    lockCount = true;
};
lockButton.OnUncheck += () =>
{
    lockCount = false;
};

window.Run();