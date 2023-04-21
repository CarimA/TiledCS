using System.Collections.Generic;
using System.Xml;
using TiledCS.Objects;

namespace TiledCS.Collections;

public class TiledObjectList : List<TiledObject>
{
    public static TiledObjectList ParseXml(XmlNodeList nodeList)
    {
        var result = new TiledObjectList();

        foreach (XmlNode node in nodeList)
        {
            if (node.Name.Equals("objectgroup"))
            {
                result.AddRange(ParseXml(node.ChildNodes));
                continue;
            }

            if (node.TrySelectSingleNode("polygon", out var nodePolygon))
                result.Add(TiledPolygonObject.ParseXml(node, nodePolygon));
            else if (node.TrySelectSingleNode("polyline", out var nodePolyline))
                result.Add(TiledPolylineObject.ParseXml(node, nodePolyline));
            else if (node.TrySelectSingleNode("ellipse", out var nodeEllipse))
                result.Add(TiledEllipseObject.ParseXml(node, nodeEllipse));
            else if (node.TrySelectSingleNode("point", out var nodePoint))
                result.Add(TiledPointObject.ParseXml(node, nodePoint));
            else if (node.Attributes["gid"] != null)
                result.Add(TiledTileObject.ParseXml(node));
            else
                result.Add(TiledRectangleObject.ParseXml(node));
        }

        return result;
    }
}