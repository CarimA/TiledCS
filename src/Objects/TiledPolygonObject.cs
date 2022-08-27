using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace TiledCS.Objects;

/// <summary>
/// Represents a polygon shape
/// </summary>
public class TiledPolygonObject : TiledObject
{
    /// <summary>
    /// The array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'.
    /// </summary>
    public List<float> Points;

    public static TiledPolygonObject ParseXml(XmlNode node, XmlNode nodePolygon)
    {
        var obj = new TiledPolygonObject();
        TiledObject.ParseXml(obj, node);

        var points = nodePolygon.Attributes["points"].Value;
        var vertices = points.Split(' ');

        obj.Points = new List<float>();

        foreach (var t in vertices)
        {
            obj.Points.Add(float.Parse(t.Split(',')[0], CultureInfo.InvariantCulture));
            obj.Points.Add(float.Parse(t.Split(',')[1], CultureInfo.InvariantCulture));
        }

        return obj;
    }
}