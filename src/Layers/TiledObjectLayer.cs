using System.Xml;
using TiledCS.Collections;

namespace TiledCS.Layers;

public class TiledObjectLayer : TiledLayer
{
    /// <summary>
    /// The list of objects in case of an objectgroup layer. Is null when the layer has no objects.
    /// </summary>
    public TiledObjectList Objects;

    public static TiledObjectLayer ParseXml(XmlNode node)
    {
        var nodesProperty = node.SelectNodes("properties/property");
        var nodesObject = node.SelectNodes("object");
        var attrVisible = node.Attributes["visible"];
        var attrLocked = node.Attributes["locked"];
        var attrTint = node.Attributes["tintcolor"];
        var attrOffsetX = node.Attributes["offsetx"];
        var attrOffsetY = node.Attributes["offsety"];

        var tiledLayer = new TiledObjectLayer
        {
            Id = int.Parse(node.Attributes["id"].Value),
            Name = node.Attributes["name"].Value,
            Objects = TiledObjectList.ParseXml(nodesObject),
            Type = "objectgroup",
            Visible = true
        };

        if (attrVisible != null) tiledLayer.Visible = attrVisible.Value == "1";
        if (attrLocked != null) tiledLayer.Locked = attrLocked.Value == "1";
        if (attrTint != null) tiledLayer.TintColor = attrTint.Value;
        if (attrOffsetX != null) tiledLayer.OffsetX = int.Parse(attrOffsetX.Value);
        if (attrOffsetY != null) tiledLayer.OffsetY = int.Parse(attrOffsetY.Value);
        if (nodesProperty != null) tiledLayer.Properties = TiledProperties.ParseXml(nodesProperty);

        return tiledLayer;
    }
}