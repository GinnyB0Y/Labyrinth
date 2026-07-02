using UnityEngine;

namespace Labyrinth.Rendering
{
    // Creates simple runtime-colored materials by cloning a single template Material
    // asset (Resources/Materials/RuntimeLit) instead of Shader.Find(...) + new Material(shader).
    // A shader with no material referencing it in the project has no usage data for Unity's
    // build-time shader stripping to work from, so it either gets left out of the build
    // entirely or - if forced in via Always Included Shaders - compiles a huge default set
    // of variants. Cloning one concrete material keeps exactly one variant (this one) in the
    // build, and keeps it from being stripped out to begin with.
    public static class RuntimeMaterialFactory
    {
        private const string TemplatePath = "Materials/RuntimeLit";

        private static Material _template;

        public static Material CreateOpaque(Color color)
        {
            return new Material(GetTemplate()) { color = color };
        }

        public static Material CreateEmissive(Color color, float intensity = 1f)
        {
            var material = CreateOpaque(color);
            material.SetColor("_EmissionColor", color * intensity);
            return material;
        }

        private static Material GetTemplate()
        {
            if (_template == null)
            {
                _template = Resources.Load<Material>(TemplatePath);
                if (_template == null)
                    Debug.LogError($"Missing template material at Resources/{TemplatePath}.mat");
            }
            return _template;
        }
    }
}
