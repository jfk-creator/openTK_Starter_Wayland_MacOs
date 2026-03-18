using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKRectangle;

public static class Renderer2D
{
    private static int _vertexArrayObject;
    private static int _vertexBufferObject;
    private static int _elementBufferObject;
    private static Shader _shader = null!;

    // A 1x1 square
    private static readonly float[] _vertices = {
         0.5f,  0.5f, 0.0f,
         0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.0f
    };

    private static readonly uint[] _indices = { 0, 1, 3, 1, 2, 3 };

    public static void Init()
    {
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        int vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }

    // Call this once per frame before you start drawing
    public static void BeginScene(Matrix4 projectionMatrix)
    {
        _shader.Use();
        _shader.SetMatrix4("projection", projectionMatrix);
    }

    // The clean API for drawing
    public static void DrawQuad(Vector2 position, Vector2 size, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    // Call this when you are done drawing for the frame
    public static void EndScene()
    {
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    // Call this when the application closes
    public static void Shutdown()
    {
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteBuffer(_elementBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        _shader.Dispose();
    }
}
