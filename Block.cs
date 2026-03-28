using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game;

public class Block
{

    internal Vector2 position;
    internal Vector2 size;
    internal Color4 color;
    public bool alive;

    public Block(Vector2 startingPosition, Vector2 startingSize, Color4 playerColor)
    {
        this.position = startingPosition;
        this.size = startingSize;
        this.color = playerColor;
        this.alive = true;
    }

    public void Render()
    {
        if (this.alive) Renderer2D.DrawQuad(this.position, this.size, this.color);
    }

    virtual public void Hit()
    {
        this.alive = false;
    }
}
