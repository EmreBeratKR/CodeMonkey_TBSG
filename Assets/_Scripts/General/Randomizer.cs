using UnityEngine;

namespace General
{
    public static class Randomizer
    {
        public static Vector3 GetInsideUnitCircleXZ()
        {
            var position = Random.insideUnitCircle;
            return new Vector3(position.x, 0f, position.y);
        }
        
        public static Vector3 GetInsideUnitCircleXZNormalized()
        {
            return GetInsideUnitCircleXZ().normalized;
        }
    }
}