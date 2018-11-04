using UnityEngine;

namespace IngameConsole
{
    public class Vector2Converter : BaseConverter<Vector2>
    {
        [ConversionMethod]
        public Vector2 Convert(float x, float y)
        {
            return new Vector2(x, y);
        }

        [ConversionMethod]
        public Vector2 Convert(float x)
        {
            return Vector2.one * x;
        }
    }
}