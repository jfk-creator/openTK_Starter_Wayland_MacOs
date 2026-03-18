using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace OpenTKRectangle;


public class Window : GameWindow
{
    private Matrix4 _projectionMatrix;
    public Action<Matrix4>? OnDraw { get; set; }
    public Action<KeyboardState, double>? OnUpdate { get; set; }

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f); // Darker background

        GL.Enable(EnableCap.Multisample);

        VSync = VSyncMode.On;

        // Initialize our new Renderer API
        Renderer2D.Init();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        OnDraw?.Invoke(_projectionMatrix);
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);

        // Update our projection matrix when the window resizes.
        // This creates a coordinate system where (0,0) is bottom-left, 
        // and (Width, Height) is top-right.
        _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, 0.0f, Size.Y, -1.0f, 1.0f);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        if (KeyboardState.IsKeyDown(Keys.Escape)) { Close(); }
        OnUpdate?.Invoke(KeyboardState, e.Time);
    }

    protected override void OnUnload()
    {
        // Clean up our renderer
        Renderer2D.Shutdown();
        base.OnUnload();
    }
}
