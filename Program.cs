using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game;

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

        var gameWindowSettings = new GameWindowSettings()
        {
            UpdateFrequency = 0.0
        };

        using (var window = new Window(gameWindowSettings, nativeWindowSettings))
        {
            Vector2 frameSize = window.FramebufferSize;
            Player p1 = new(new Vector2(frameSize.X / 2, 75), new Vector2(200, 50), Color4.DarkCyan);
            Ball ball = new(new Vector2(frameSize.X / 2, frameSize.Y / 2 - 200), new Vector2(50, 50), Color4.DeepPink);
            Block[] blocks = new Block[9];

            for (int i = 0; i < blocks.Length; ++i)
            {
                float width = frameSize.X / (blocks.Length * 2);
                float gap = 75;
                Vector2 startingPos = new(width * (i + 1) + (gap * i + 1), frameSize.Y - 100);
                Vector2 size = new(100, 50);
                blocks[i] = new Block(startingPos, size, Color4.OrangeRed);
            }

            window.OnUpdate = (keyboard, deltaTime) =>
            {
                p1.Update(keyboard, (float)deltaTime, window.FramebufferSize);
                ball.Update(frameSize, (float)deltaTime);
                CheckCollision(ball, p1);

                for (int i = 0; i < blocks.Length; ++i)
                {
                    CheckCollision(ball, blocks[i]);
                }
            };

            window.OnDraw = (projectionMatrix) =>
            {
                Renderer2D.BeginScene(projectionMatrix);

                p1.Render();

                for (int i = 0; i < blocks.Length; ++i)
                {
                    blocks[i].Render();
                }

                ball.Render();
                Renderer2D.EndScene();
            };
            window.Run();
        }
    }

    static void CheckCollision(Ball ball, Block block)
    {
        float ballYUp = ball.position.Y + (ball.size.Y / 2);
        float ballYDown = ball.position.Y - (ball.size.Y / 2);


        float ballXLeft = ball.position.X + (ball.size.X / 2);
        float ballXRight = ball.position.X - (ball.size.X / 2);

        float blockYUp = block.position.Y + (block.size.Y / 2);
        float blockYDown = block.position.Y - (block.size.Y / 2);

        float blockXLeft = block.position.X + (block.size.X / 2);
        float blockXRight = block.position.X - (block.size.X / 2);

        if (ballXLeft < blockXLeft && ballXLeft > blockXRight || ballXRight < blockXLeft && ballXRight > blockXRight)
        {

            //Y Up
            if (ballYUp > blockYUp && ballYDown < blockYUp && ball.vel.Y < 0 && block.alive)
            {
                ball.HitY();
                block.Hit();
            }

            //Y Down
            if (ballYUp > blockYDown && ballYDown < blockYDown && ball.vel.Y > 0 && block.alive)
            {
                ball.HitY();
                block.Hit();
            }

        }


    }
}
