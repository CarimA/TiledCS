using System.Collections.Generic;
using System.Xml;
using TiledCS.Collections;
using TiledCS.Objects;

namespace TiledCS
{
    /// <summary>
    /// Represents a layer or object group
    /// </summary>
    public class TiledGroup
    {
        /// <summary>
        /// The group's id
        /// </summary>
        public int Id;
        /// <summary>
        /// The group's name
        /// </summary>
        public string Name;
        /// <summary>
        /// The group's visibility
        /// </summary>
        public bool Visible;
        /// <summary>
        /// The group's locked state
        /// </summary>
        public bool Locked;
        /// <summary>
        /// The group's user properties
        /// </summary>
        public TiledProperties Properties;
        /// <summary>
        /// The group's layers
        /// </summary>
        public TiledLayerList Layers;
        /// <summary>
        /// The group's objects
        /// </summary>
        public TiledObjectList Objects;
        /// <summary>
        /// The group's subgroups
        /// </summary>
        public TiledGroupList Groups;

        public static TiledGroup ParseXml(XmlNode node)
        {
            var nodesProperty = node.SelectNodes("properties/property");
            var nodesGroup = node.SelectNodes("group");
            var nodesLayer = node.SelectNodes("layer");
            var attrVisible = node.Attributes["visible"];
            var attrLocked = node.Attributes["locked"];

            var tiledGroup = new TiledGroup
            {
                Id = int.Parse(node.Attributes["id"].Value),
                Name = node.Attributes["name"].Value
            };

            if (attrVisible != null) tiledGroup.Visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledGroup.Locked = attrLocked.Value == "1";
            if (nodesProperty != null) tiledGroup.Properties = TiledProperties.ParseXml(nodesProperty);
            if (nodesGroup != null) tiledGroup.Groups = TiledGroupList.ParseXml(nodesGroup);
            if (nodesLayer != null) tiledGroup.Layers = TiledLayerList.ParseXml(node);

            return tiledGroup;
        }
    }
}