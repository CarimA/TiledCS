using System.Xml;

namespace TiledCS.Objects;

/// <summary>
/// Represents a point shape
/// </summary>
public class TiledPointObject : TiledObject
{
    public static TiledPointObject ParseXml(XmlNode node, XmlNode nodePoint)
    {
        var obj = new TiledPointObject();
        TiledObject.ParseXml(obj, node);

        // quite literally do not need to do anything!
        return obj;
    }
}