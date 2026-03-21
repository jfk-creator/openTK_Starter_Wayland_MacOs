using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game;

public static class Renderer2D
{
        
    private static Shader _shader = null!;

    // --- Triangle Buffer ---
    private static int _triangleVertexArrayObject;
    private static int _triangleVertexBufferObject;

    // --- Quad Buffer ---
    private static int _quadVertexArrayObject;
    private static int _quadVertexBufferObject;
    private static int _quadElementArrayBufferObject;

    // --- Circle Buffers ---
    private static int _circleVertexArrayObject;
    private static int _circleVertexBufferObject;
    private static int _circleVertexCount;
    private const int CircleSegments = 36;

    // --- Circle Outline Buffers ---
    private static int _circleOutlineVertexArrayObject;
    private static int _circleOutlineVertexBufferObject;
    private static int _circleOutlineVertexCount;


    public static void Init()
    {

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        InitTriangle();
        InitQuad();
        InitCircle();
        InitCircleOutline();
    }

    private static void InitTriangle()
    {
        float[] _vertices = {
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
        };

        _triangleVertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_triangleVertexArrayObject);

        _triangleVertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _triangleVertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);


        int vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

    }

    private static void InitQuad()
    {
        // A 1x1 square
        float[] _vertices = {
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.0f
        };

        uint[] _indices = { 0, 1, 3, 1, 2, 3 };

        _quadVertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_quadVertexArrayObject);

        _quadVertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _quadElementArrayBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _quadElementArrayBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(float), _indices, BufferUsageHint.StaticDraw);

        int vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }

    private static void InitCircle()
    {
        List<float> circleVertices = new List<float>();

        for (int i = 0; i < CircleSegments; i++)
        {
            float theta1 = 2.0f * MathF.PI * i / CircleSegments;
            float theta2 = 2.0f * MathF.PI * (i + 1) / CircleSegments;

            circleVertices.Add(0.0f); circleVertices.Add(0.0f); circleVertices.Add(0.0f);
            circleVertices.Add(MathF.Cos(theta1)); circleVertices.Add(MathF.Sin(theta1)); circleVertices.Add(0.0f);
            circleVertices.Add(MathF.Cos(theta2)); circleVertices.Add(MathF.Sin(theta2)); circleVertices.Add(0.0f);
        }

        _circleVertexCount = circleVertices.Count / 3;

        _circleVertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_circleVertexArrayObject);

        _circleVertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _circleVertexBufferObject);
        GL.BufferData(
                BufferTarget.ArrayBuffer,
                circleVertices.Count * sizeof(float),
                circleVertices.ToArray(),
                BufferUsageHint.StaticDraw);

        int vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }

    private static void InitCircleOutline()
    {
        List<float> outlineVertices = new List<float>();
        for (int i = 0; i < CircleSegments; i++)
        {
            float theta = 2.0f * MathF.PI * i / CircleSegments;

            outlineVertices.Add(MathF.Cos(theta));
            outlineVertices.Add(MathF.Sin(theta));
            outlineVertices.Add(0.0f);
        }

        _circleOutlineVertexCount = outlineVertices.Count / 3;
        _circleOutlineVertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_circleOutlineVertexArrayObject);
        _circleOutlineVertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _circleOutlineVertexBufferObject);
        GL.BufferData(
                BufferTarget.ArrayBuffer,
                outlineVertices.Count * sizeof(float),
                outlineVertices.ToArray(),
                BufferUsageHint.StaticDraw);

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

    public static void DrawTriangle(Vector2 position, Vector2 size, Color4 color) 
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_triangleVertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }


    public static void DrawTriangle(Vector2 position, Vector2 size, float rotation, Color4 color) 
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 rot = Matrix4.CreateRotationZ(rotation);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * rot * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_triangleVertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public static void DrawQuad(Vector2 position, Vector2 size, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_quadVertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
    }


    public static void DrawQuad(Vector2 position, Vector2 size, float rotation, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 rot = Matrix4.CreateRotationZ(rotation);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * rot * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_quadVertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
    }

    public static void DrawQuadOutline(Vector2 position, Vector2 size, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_quadVertexArrayObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
    }


    public static void DrawQuadOutline(Vector2 position, Vector2 size, float rotation, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 rot = Matrix4.CreateRotationZ(rotation);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * rot * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_quadVertexArrayObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
    }

    public static void DrawCircle(Vector2 position, float radius, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(radius, radius, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_circleVertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _circleVertexCount);
    }


    public static void DrawCircle(Vector2 position, float radius, float rotation, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(radius, radius, 1.0f);
        Matrix4 rot = Matrix4.CreateRotationZ(rotation);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * rot * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_circleVertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _circleVertexCount);
    }


    public static void DrawCircleOutline(Vector2 position, float radius, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(radius, radius, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_circleOutlineVertexArrayObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, _circleOutlineVertexCount);
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
        GL.DeleteBuffer(_triangleVertexBufferObject);
        GL.DeleteVertexArray(_triangleVertexArrayObject);

        GL.DeleteBuffer(_quadVertexBufferObject);
        GL.DeleteBuffer(_quadElementArrayBufferObject);
        GL.DeleteVertexArray(_quadVertexArrayObject);

        GL.DeleteBuffer(_circleVertexBufferObject);
        GL.DeleteVertexArray(_circleVertexArrayObject);

        GL.DeleteBuffer(_circleOutlineVertexBufferObject);
        GL.DeleteVertexArray(_circleOutlineVertexArrayObject);

        _shader.Dispose();
    }
}
