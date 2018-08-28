using System;

namespace UnityEditor.ShaderGraph
{
    public enum SurfaceType
    {
        Opaque,
        Transparent
    }

    public enum AlphaModeNoMultiply
    {
        Alpha,
        PremultipliedAlpha,
        Additive,
    }

    public enum AlphaMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply
    }
}
