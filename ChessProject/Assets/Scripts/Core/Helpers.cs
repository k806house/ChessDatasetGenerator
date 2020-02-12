using UnityEngine;
using static Assets.Scripts.Core.Constants;

namespace Assets.Scripts.Core
{
    public static class Helpers
    {
        public static float GetCmFromDst(float dst, float boardWidth) => dst * BoardWidthCm / boardWidth;
        public static float GetDstFromCm(float cmDst, float boardWidth) => cmDst * boardWidth / BoardWidthCm * WorldScaleFactor;

        public static (Vector3 bounds, Vector3 center) GetObjectRendererParams(GameObject obj)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = obj.AddComponent<Renderer>();
            }
            return (renderer.bounds.extents, renderer.bounds.center);
        }
    }
}
