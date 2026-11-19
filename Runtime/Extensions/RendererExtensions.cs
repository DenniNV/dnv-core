using UnityEngine;

namespace Libraries.DNV.Extensions
{
    public static class RendererExtensions
    {
        private static MaterialPropertyBlock _materialPropertyBlock;
        public static void ChangeSurfaceModeByTransparent(this Renderer renderer)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat("_Mode" , 1f);
            _materialPropertyBlock.SetFloat("_Surface", 1f);
            _materialPropertyBlock.SetFloat("_Blend", 3f);
        
        
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }
    
        public static void ChangeSurfaceModeByOpaque(this Renderer renderer)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_BaseColor", Color.blue);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
