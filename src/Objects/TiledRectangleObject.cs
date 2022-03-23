using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace TiledCS.Objects
{
    /// <summary>
    /// Represents a polygon shape
    /// </summary>
    public class TiledRectangleObject : TiledObject
    {
        /// <summary>
        /// The object's width in pixels
        /// </summary>
        public float Width;
        /// <summary>
        /// The object's height in pixels
        /// </summary>
        public float Height;

        public static TiledRectangleObject ParseXml(XmlNode node)
        {
            var obj = new TiledRectangleObject();
            TiledObject.ParseXml(obj, node);

            if (node.Attributes["width"] != null)
            {
                obj.Width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);
            }

            if (node.Attributes["height"] != null)
            {
                obj.Height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);
            }

            return obj;
        }
    }
}