using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game;

public class Shader : IDisposable
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexCode = File.ReadAllText(vertexPath);
        string fragmentCode = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexCode);
        CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentCode);
        CompileShader(fragmentShader);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        LinkProgram(Handle);

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    private void CompileShader(int shader)
    {
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine(infoLog);
        }
    }

    private void LinkProgram(int program)
    {
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            Console.WriteLine(infoLog);
        }
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
        GC.SuppressFinalize(this);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, false, ref data);
    }

    public void SetColor4(string name, Color4 data)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform4(location, data);
    }
}
