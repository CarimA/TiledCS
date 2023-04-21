using System.Xml;

namespace TiledCS.Objects;

/// <summary>
/// Represents a polygon shape
/// </summary>
public class TiledTileObject : TiledObject
{
    /// <summary>
    /// The object's tile GID
    /// </summary>
    public int Gid;

    public static TiledTileObject ParseXml(XmlNode node)
    {
        var obj = new TiledTileObject();
        TiledRectangleObject.ParseXml(obj, node);

        if (node.Attributes["gid"] != null)
        {
            obj.Gid = int.Parse(node.Attributes["gid"].Value);
        }

        return obj;
    }
}