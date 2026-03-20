using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
            APIVersion = new Version(3, 3),
            NumberOfSamples = 4, // MSAA
        };

        Vector2 v = new Vector2(0, 0);
        Vector2 p = new Vector2(200, 200);

        var gameWindowSettings = new GameWindowSettings()
        {
            UpdateFrequency = 0.0
        };

        using (var window = new Window(gameWindowSettings, nativeWindowSettings))
        {

            window.OnUpdate = (keyboard, deltaTime) =>
            {
                Vector2 a = new Vector2(0, 0);
                float acc = 1f;
                //Input Logic
                if (keyboard.IsKeyDown(Keys.Right)) a.X += acc;
                if (keyboard.IsKeyDown(Keys.Left)) a.X -= acc;
                if (keyboard.IsKeyDown(Keys.Up)) a.Y += acc;
                if (keyboard.IsKeyDown(Keys.Down)) a.Y -= acc;

                v += a * (float)deltaTime;
                p += v;
            };


            window.OnDraw = (projectionMatrix) =>
            {
                Renderer2D.BeginScene(projectionMatrix);
                Renderer2D.DrawCircleOutline(p, 60, Color4.White);

                Renderer2D.EndScene();
            };
            window.Run();
        }
    }
}
