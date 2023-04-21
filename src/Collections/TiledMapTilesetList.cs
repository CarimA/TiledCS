using System.Collections.Generic;
using System.Xml;

namespace TiledCS.Collections;

public class TiledMapTilesetList : List<TiledMapTileset>
{
    public static TiledMapTilesetList ParseXml(XmlNodeList nodeList)
    {
        var result = new TiledMapTilesetList();

        foreach (XmlNode node in nodeList)
        {
            result.Add(TiledMapTileset.ParseXml(node));
        }

        return result;
    }

}