using System.Numerics;
using MuekFramework.Graphics;
using MuekFramework.Graphics.Controls;

MuekWindow window = new MuekWindow("MuekFramework Sample", 1000, 800);
var mainPanel = new Panel(Muek.MuekColors.White, 800, 600, 100, 100)
{
    BorderRadius = new Vector2(10F),
    IsAnimationDisabled = false,
    HoverScale = new Vector2(1.1F)
};
window.Add(mainPanel);
var helloButton = new Button(Muek.MuekColors.Muek, 200, 200, 300, 200)
{
    HoverColor = Muek.MuekColors.LightMuek,
    PressedColor = Muek.MuekColors.DarkMuek,
};
mainPanel.Add(helloButton);
helloButton.OnClick += () =>
{
    if (helloButton.Children.Count == 0) helloButton.AddText(out Text text, "Hello", 24F);
};

window.Run();