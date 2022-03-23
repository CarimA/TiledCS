using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiledCS.Layers;

namespace TiledCS.Collections
{
    public class TiledLayerList : List<TiledLayer>
    {
        public static TiledLayerList ParseXml(XmlNode node)
        {
            var nodesLayer = node.SelectNodes("layer");
            var nodesObjectGroup = node.SelectNodes("objectgroup");
            var nodesImageLayer = node.SelectNodes("imagelayer");

            var result = new TiledLayerList();

            foreach (XmlNode child in nodesLayer)
            {
                result.Add(TiledTileLayer.ParseXml(child));
            }

            foreach (XmlNode child in nodesObjectGroup)
            {
                result.Add(TiledObjectLayer.ParseXml(child));
            }

            foreach (XmlNode child in nodesImageLayer)
            {
                result.Add(TiledImageLayer.ParseXml(child));
            }

            return result;
        }
    }
}
