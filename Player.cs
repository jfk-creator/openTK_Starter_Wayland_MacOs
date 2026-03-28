using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game;

public class Player : Block
{
    public Player(Vector2 startingPosition, Vector2 startingSize, Color4 playerColor) : base(startingPosition, startingSize, playerColor)
    {
    }

    public void Update(KeyboardState keyboard, float dt, Vector2 frameSize)
    {
        Input(keyboard, dt);
        CheckBounds(frameSize);
    }

    void Input(KeyboardState keyboard, float dt)
    {
        float inc = 500.0f * dt;
        if (keyboard.IsKeyDown(Keys.Right)) this.position.X += inc;
        if (keyboard.IsKeyDown(Keys.Left)) this.position.X -= inc;
    }

    void CheckBounds(Vector2 frameSize)
    {
        if (this.position.X - this.size.X / 2 < 0) this.position.X = this.size.X / 2;
        if (this.position.X + this.size.X / 2 > frameSize.X) this.position.X = frameSize.X - this.size.X / 2;
    }

    override public void Hit()
    {
        this.alive = true;
    }
}
