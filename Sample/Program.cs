using System.Numerics;
using MuekFramework.Graphics;
using MuekFramework.Graphics.Controls;

var count = 0;

var customText = new Text("0 click")
{
    FontSize = 24f,
    ContentPosition = Muek.ContentPosition.Center
};

var window = new MuekWindow("MuekFramework Sample", 1000, 800);
var mainPanel = new Panel(Muek.MuekColors.White, 800, 600, 100, 100)
{
    BorderRadius = new Vector2(10F),
    IsAnimationDisabled = false,
    AnimationSpeed = .05F,
    HoverScale = new Vector2(1.05F),
    ContentPosition = Muek.ContentPosition.Center
};
var helloButton = new Button(Muek.MuekColors.Muek, 200, 200)
{
    HoverColor = Muek.MuekColors.LightMuek,
    PressedColor = Muek.MuekColors.DarkMuek
};

helloButton.OnClick += () =>
{
    if (count == 0)
    {
        helloButton.Clear();
        helloButton.AddText(customText);
    }
    customText.Content = $"{++count} clicked";
};
var lockButton = new ToggleButton(Muek.MuekColors.MuekBlue, 100, 100)
{
    HoverColor = Muek.MuekColors.LightMuekBlue,
    PressedColor = Muek.MuekColors.DarkMuekBlue,
    CheckedColor = Muek.MuekColors.MuekRed
};
lockButton.OnCheck += () =>
{
    helloButton.IsDisabled = true;
};
lockButton.OnUncheck += () =>
{
    helloButton.IsDisabled = false;
};

window.Add([
    mainPanel.Add([
        helloButton.AddText("Hello", 24F),
        lockButton.AddText("Lock", 24F),
        new ScrollBar(60,200).Add([
            new Button(Muek.MuekColors.Muek,50,50).AddText("1"),
            new Button(Muek.MuekColors.Muek,50,50).AddText("2"),
            new Button(Muek.MuekColors.Muek,50,50).AddText("3"),
            new Button(Muek.MuekColors.Muek,50,50).AddText("4")
        ])
        ])
]);

window.Run();