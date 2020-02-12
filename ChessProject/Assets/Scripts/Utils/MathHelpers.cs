using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class MathHelpers
    {
        public static float DegreeToRadian(float angle) => Mathf.PI * angle / 180f;
        public static Vector3 VectorFromAngle(float theta) => new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta));
    }
}