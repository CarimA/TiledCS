using System.Collections.Generic;
using System.Xml;

namespace TiledCS.Collections;

public class TiledProperties : Dictionary<string, object>
{
    public static TiledProperties ParseXml(XmlNodeList nodeList)
    {
        var result = new TiledProperties();

        foreach (XmlNode node in nodeList)
        {
            // TODO: implement the missing data types (color (a 32-bit color value), file (a relative path referencing a file), object (a reference to an object))
            result.Add(node.Attributes["name"].Value, node.Attributes["type"]?.Value switch
            {
                "bool" => bool.Parse(node.Attributes["value"]?.Value),
                "float" => float.Parse(node.Attributes["value"]?.Value),
                "int" => int.Parse(node.Attributes["value"]?.Value),
                _ => node.Attributes["value"]?.Value
            });

            /*if (property.value == null && node.InnerText != null)
            {
                property.value = node.InnerText;
            }*/
        }

        return result;
    }
}