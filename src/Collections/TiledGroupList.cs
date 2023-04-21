using System.Collections.Generic;
using System.Xml;

namespace TiledCS.Collections;

public class TiledGroupList : List<TiledGroup>
{
    public static TiledGroupList ParseXml(XmlNodeList nodeListGroups)
    {
        var result = new TiledGroupList();

        foreach (XmlNode node in nodeListGroups)
        {
            result.Add(TiledGroup.ParseXml(node));
        }

        return result;
    }
}