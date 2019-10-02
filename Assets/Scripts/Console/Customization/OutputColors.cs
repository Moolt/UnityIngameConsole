using UnityEngine;

namespace IngameConsole
{
    public class OutputColors
    {
        public OutputColors()
        {
        }

        public OutputColors(Color error, Color info, Color warning)
        {
            Error = error;
            Info = info;
            Warning = warning;
        }

        public static OutputColors Default { get; } = new OutputColors()
        {
            Error = Color.red,
            Info = new Color32(30, 98, 206, 255),
            Warning = new Color32(170, 135, 30, 255)
        };

        public Color Error { get; set; }

        public Color Info { get; set; }

        public Color Warning { get; set; }
    }
}