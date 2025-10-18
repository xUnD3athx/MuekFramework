using System.Runtime.CompilerServices;
using MuekFramework.Graphics.Controls;
using SDL3;
using SkiaSharp;

namespace MuekFramework.Graphics;

public class MuekWindow
{
    public readonly string Title;
    public readonly int Width;
    public readonly int Height;
    public readonly IntPtr Window;
    public readonly IntPtr Renderer;
    public List<IControl> Children { get; set; } = new();
    
    public event Muek.RenderDelegate? OnRender;
    public event Muek.InputDelegate? OnInput;
    
    /// <summary>
    /// This will create a new window and display it.
    /// </summary>
    /// <param name="title">This is the title of the window</param>
    /// <param name="width">This is the width of the window</param>
    /// <param name="height">This is the height of the window</param>
    public MuekWindow(string title, int width, int height)
    {
        Title = title;
        Width = width;
        Height = height;
        
        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }
        if (!SDL.CreateWindowAndRenderer(Title, 
                Width, Height,
                SDL.WindowFlags.OpenGL,
                out Window, out Renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window: {SDL.GetError()}");
            return;
        }
    }
    
    /// <summary>
    /// Run the window.
    /// </summary>
    public void Run()
    {
        while (true)
        {
            while (SDL.PollEvent(out var e))
            {
                if (e.Type == (uint)SDL.EventType.Quit)
                {
                    Quit();
                    return;
                }
                Input(e);
            }
            Render();
            SDL.Delay(10);
        }
    }
    /// <summary>
    /// This will quit the application.
    /// </summary>
    public void Quit()
    {
        SDL.DestroyRenderer(Renderer);
        SDL.DestroyWindow(Window);
        SDL.Quit();
    }
    
    private void Render()
    {
        SDL.SetRenderDrawColor(Renderer, 0, 0, 0, 0);
        SDL.RenderClear(Renderer);
        IntPtr texture = SDL.CreateTexture(Renderer,
            SDL.PixelFormat.ARGB8888, SDL.TextureAccess.Streaming, Width, Height);
        SDL.LockTexture(texture, IntPtr.Zero, out IntPtr pixels, out int pitch);
        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        var canvas = SKSurface.Create(info, pixels, pitch).Canvas;
        OnRender?.Invoke(canvas);
        SDL.UnlockTexture(texture);
        SDL.RenderTexture(Renderer, texture,IntPtr.Zero,IntPtr.Zero);
        SDL.DestroyTexture(texture);
        SDL.RenderPresent(Renderer);
    }
    
    private void Input(SDL.Event e)
    {
        OnInput?.Invoke(e);
    }
    
    public void Add(IControl control)
    {
        Children.Add(control);
        OnRender += control.Render();
        OnInput += control.Input();
    }
}