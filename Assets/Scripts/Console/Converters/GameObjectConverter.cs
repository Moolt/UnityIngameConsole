using System;
using UnityEngine;

namespace IngameConsole
{
    public class GameObjectConverter : BaseConverter<GameObject>
    {
        [ConversionMethod]
        public GameObject Convert(string name)
        {
            var converted = GameObject.Find(name);

            if (converted == null)
            {
                throw new Exception(string.Format("GameObject with name <b>{0}</b> not found.", name));
            }

            return converted;
        }

    }
}