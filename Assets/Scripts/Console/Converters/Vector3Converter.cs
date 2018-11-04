using UnityEngine;

namespace IngameConsole
{
    public class Vector3Converter : BaseConverter<Vector3>
    {
        [ConversionMethod]
        public Vector3 Convert(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        [ConversionMethod]
        public Vector3 Convert(float x, float y)
        {
            return new Vector3(x, y);
        }

        [ConversionMethod]
        public Vector3 Convert(float x)
        {
            return Vector3.one * x;
        }
    }
}