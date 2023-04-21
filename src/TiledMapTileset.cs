using System.Xml;

namespace TiledCS;

/// <summary>
/// Represents an element within the Tilesets array of a TiledMap object
/// </summary>
public class TiledMapTileset
{
    /// <summary>
    /// The first gid defines which gid matches the tile with source vector 0,0. Is used to determine which tileset belongs to which gid
    /// </summary>
    public int FirstGid;
    /// <summary>
    /// The tsx file path as defined in the map file itself
    /// </summary>
    public string Source;

    public static TiledMapTileset ParseXml(XmlNode node)
    {
        var tileset = new TiledMapTileset
        {
            FirstGid = int.Parse(node.Attributes["firstgid"].Value),
            Source = node.Attributes["source"].Value
        };
        return tileset;
    }
}