using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTKRectangle;

public static class Program
{
    public static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "OpenTK Cross-Platform Rectangle",

            // CRITICAL FOR macOS: 
            // Explicitly request an OpenGL 3.3 core profile. 
            // Without this, macOS will give you a legacy 2.1 context and shaders will fail.
            Flags = ContextFlags.ForwardCompatible,
            Profile = ContextProfile.Core,
            APIVersion = new Version(3, 3)
        };

        using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
        {
            window.OnDraw = (projectionMatrix) =>
            {
                Renderer2D.BeginScene(projectionMatrix);

                Renderer2D.DrawQuad(new Vector2(200f, 200f), new Vector2(100f, 100f), Color4.LimeGreen);

                Renderer2D.EndScene();
            };
            window.Run();
        }
    }
}
