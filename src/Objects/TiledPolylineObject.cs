using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace TiledCS.Objects;

/// <summary>
/// Represents a poly line shape
/// </summary>
public class TiledPolylineObject : TiledObject
{
    /// <summary>
    /// The array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'.
    /// </summary>
    public List<float> Points;

    public static TiledPolylineObject ParseXml(XmlNode node, XmlNode nodePolyline)
    {
        var obj = new TiledPolylineObject();
        TiledObject.ParseXml(obj, node);

        var points = nodePolyline.Attributes["points"].Value;
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