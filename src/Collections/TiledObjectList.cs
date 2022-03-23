using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiledCS.Objects;

namespace TiledCS.Collections
{
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

                var nodePolygon = node.SelectSingleNode("polygon");
                var nodePolyline = node.SelectSingleNode("polyline");
                var nodePoint = node.SelectSingleNode("point");
                var nodeEllipse = node.SelectSingleNode("ellipse");

                if (nodePolygon != null)
                    result.Add(TiledPolygonObject.ParseXml(node, nodePolygon));
                else if (nodePolyline != null)
                    result.Add(TiledPolylineObject.ParseXml(node, nodePolyline));
                else if (nodeEllipse != null)
                    result.Add(TiledEllipseObject.ParseXml(node, nodeEllipse));
                else if (nodePoint != null)
                    result.Add(TiledPointObject.ParseXml(node, nodePoint));
                else
                    result.Add(TiledRectangleObject.ParseXml(node));
            }

            return result;
        }
    }
}
