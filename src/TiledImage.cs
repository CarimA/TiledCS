using System.Xml;

namespace TiledCS;

/// <summary>
/// Represents an image
/// </summary>
public class TiledImage
{
    /// <summary>
    /// The image width
    /// </summary>
    public int Width;

    /// <summary>
    /// The image height
    /// </summary>
    public int Height;

    /// <summary>
    /// The image source path
    /// </summary>
    public string Source;

    public static TiledImage ParseXml(XmlNode node)
    {
        var tiledImage = new TiledImage
        {
            Source = node.Attributes["source"].Value,
            Width = int.Parse(node.Attributes["width"].Value),
            Height = int.Parse(node.Attributes["height"].Value)
        };

        return tiledImage;
    }
}