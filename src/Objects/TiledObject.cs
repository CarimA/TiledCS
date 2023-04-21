using System.Globalization;
using System.Xml;
using TiledCS.Collections;

namespace TiledCS.Objects;

/// <summary>
/// Represents an tiled object defined in object layers
/// </summary>
public abstract class TiledObject
{
    /// <summary>
    /// The object id
    /// </summary>
    public int Id;
    /// <summary>
    /// The object's name
    /// </summary>
    public string Name;
    /// <summary>
    /// The object type if defined. Null if none was set.
    /// </summary>
    public string Type;
    /// <summary>
    /// The object's x position in pixels
    /// </summary>
    public float X;
    /// <summary>
    /// The object's y position in pixels
    /// </summary>
    public float Y;
    /// <summary>
    /// The object's rotation
    /// </summary>
    public int Rotation;
    /// <summary>
    /// The tileset gid when the object is linked to a tile
    /// </summary>
    public int Gid;
    /// <summary>
    /// An array of properties. Is null if none were defined.
    /// </summary>
    public TiledProperties Properties;

    internal static void ParseXml(TiledObject obj, XmlNode node)
    {
        obj.Id = int.Parse(node.Attributes["id"].Value);
        obj.Name = node.Attributes["name"]?.Value;
        obj.Type = node.Attributes["type"]?.Value;
        obj.Gid = int.Parse(node.Attributes["gid"]?.Value ?? "0");
        obj.X = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
        obj.Y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);
        obj.Properties = TiledProperties.ParseXml(node.SelectNodes("properties/property"));

        if (node.Attributes["rotation"] != null)
        {
            obj.Rotation = int.Parse(node.Attributes["rotation"].Value);
        }
    }
}