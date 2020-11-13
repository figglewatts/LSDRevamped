using UnityEngine;

namespace LSDR.Lua
{
    public class UnityAPI
    {
        public static Vector3 Vector3(float x, float y, float z) { return new Vector3(x, y, z); }

        public static float DeltaTime() { return Time.deltaTime; }
    }
}
