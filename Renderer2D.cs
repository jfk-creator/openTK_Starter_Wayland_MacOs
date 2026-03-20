using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKRectangle;

public static class Renderer2D
{
    private static int _vertexArrayObject;
    private static int _vertexBufferObject;
    private static int _elementBufferObject;
    private static Shader _shader = null!;

    // --- Circle Buffers ---
    private static int _circleVertexArrayObject;
    private static int _circleVertexBufferObject;
    private static int _circleVertexCount;
    private const int CircleSegments = 36;

    // --- Circle Outline Buffers ---
    private static int _circleOutlineVertexArrayObject;
    private static int _circleOutlineVertexBufferObject;
    private static int _circleOutlineVertexCount;

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
        InitQuad();
        InitCircle();
        InitCircleOutline();
    }

    private static void InitQuad()
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
        GL.BufferData(BufferTarget.ArrayBuffer, circleVertices.Count * sizeof(float), circleVertices.ToArray(), BufferUsageHint.StaticDraw);

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
        GL.BufferData(BufferTarget.ArrayBuffer, outlineVertices.Count * sizeof(float), outlineVertices.ToArray(), BufferUsageHint.StaticDraw);

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

    public static void DrawQuadOutline(Vector2 position, Vector2 size, Color4 color)
    {
        Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
        Matrix4 translation = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        Matrix4 model = scale * translation;

        _shader.SetMatrix4("model", model);
        _shader.SetColor4("color", color);

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
    }

    public static void DrawQuadOutline(Vector2 position, Vector2 size, float thickness, Color4 color)
    {
        float halfWidth = size.X / 2.0f;
        float halfHeight = size.Y / 2.0f;

        DrawQuad(new Vector2(position.X, position.Y + halfHeight), new Vector2(size.X + thickness, thickness), color);
        DrawQuad(new Vector2(position.X, position.Y - halfHeight), new Vector2(size.X + thickness, thickness), color);
        DrawQuad(new Vector2(position.X - halfWidth, position.Y), new Vector2(thickness, size.Y - thickness), color);
        DrawQuad(new Vector2(position.X + halfWidth, position.Y), new Vector2(thickness, size.Y - thickness), color);
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
        // Cleanup Quad
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteBuffer(_elementBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);

        // Cleanup Circle
        GL.DeleteBuffer(_circleVertexBufferObject);
        GL.DeleteVertexArray(_circleVertexArrayObject);
        GL.DeleteBuffer(_circleOutlineVertexBufferObject);
        GL.DeleteVertexArray(_circleOutlineVertexArrayObject);

        _shader.Dispose();
    }
}
