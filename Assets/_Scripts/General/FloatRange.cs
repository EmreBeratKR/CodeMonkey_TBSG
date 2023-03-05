using System;
using UnityEngine;

namespace General
{
    [Serializable]
    public struct FloatRange
    {
        public float a;
        public float b;


        public float Clamp(float value)
        {
            return Mathf.Clamp(value, a, b);
        }
    }
}