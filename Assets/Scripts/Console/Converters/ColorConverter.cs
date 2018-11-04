using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

namespace IngameConsole
{
    public class ColorConverter : BaseConverter<Color>
    {
        [ConversionMethod]
        //RGB with alpha
        public Color Convert(float r, float g, float b, float a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        [ConversionMethod]
        //RGB without alpha
        public Color Convert(float r, float g, float b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        [ConversionMethod]
        public Color Convert(string color)
        {
            Color parsedColor;

            if (color.StartsWith("#"))
            {
                ColorUtility.TryParseHtmlString(color, out parsedColor);
            }
            else
            {
                var mapping = typeof(Color).GetStaticPropertiesOfType<Color>();

                if (mapping.ContainsKey(color.ToLower()))
                {
                    parsedColor = mapping[color.ToLower()];
                }
                else
                {
                    throw new Exception(string.Format("Unknown color {0}.", color));
                }
            }

            return parsedColor;
        }
    }
}
