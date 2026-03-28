using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game;

public class Ball : Block
{

    public Vector2 vel;

    public Ball(Vector2 startingPosition, Vector2 startingSize, Color4 playerColor) : base(startingPosition, startingSize, playerColor)
    {
        this.vel = new(400.0f, 400.0f);
    }

    public void Update(Vector2 frameSize, float dt)
    {
        this.position += this.vel * dt;
        CheckBounds(frameSize);
    }

    public void HitX()
    {
        this.vel.X *= -1;
    }

    public void HitY()
    {
        this.vel.Y *= -1;
    }


    void CheckBounds(Vector2 frameSize)
    {
        if (this.position.X - this.size.X / 2 < 0 && this.vel.X < 0) this.HitX();
        if (this.position.X + this.size.X / 2 > frameSize.X && this.vel.X > 0) this.HitX();

        if (this.position.Y - this.size.Y / 2 < 0 && this.vel.Y < 0) this.HitY();
        if (this.position.Y + this.size.Y / 2 > frameSize.Y && this.vel.Y > 0) this.HitY();
    }

}
