using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiledCS.Collections;

namespace TiledCS.Layers
{
    public class TiledImageLayer : TiledLayer
    {
        public TiledImage Image;

        public static TiledImageLayer ParseXml(XmlNode node)
        {
            var nodesProperty = node.SelectNodes("properties/property");
            var nodeImage = node.SelectSingleNode("image");
            var attrVisible = node.Attributes["visible"];
            var attrLocked = node.Attributes["locked"];
            var attrTint = node.Attributes["tintcolor"];
            var attrOffsetX = node.Attributes["offsetx"];
            var attrOffsetY = node.Attributes["offsety"];

            var tiledLayer = new TiledImageLayer
            {
                Id = int.Parse(node.Attributes["id"].Value),
                Name = node.Attributes["name"].Value,
                Type = "imagelayer",
                Visible = true
            };

            if (attrVisible != null) tiledLayer.Visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.Locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.TintColor = attrTint.Value;
            if (attrOffsetX != null) tiledLayer.OffsetX = int.Parse(attrOffsetX.Value);
            if (attrOffsetY != null) tiledLayer.OffsetY = int.Parse(attrOffsetY.Value);
            if (nodesProperty != null) tiledLayer.Properties = TiledProperties.ParseXml(nodesProperty);
            if (nodeImage != null) tiledLayer.Image = TiledImage.ParseXml(nodeImage);

            return tiledLayer;
        }
    }
}