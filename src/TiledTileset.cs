using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TiledCS.Collections;

namespace TiledCS;

/// <summary>
/// Represents a Tiled tileset
/// </summary>
public class TiledTileset
{
    /// <summary>
    /// The Tiled version used to create this tileset
    /// </summary>
    public string TiledVersion { get; set; }
    /// <summary>
    /// The tileset name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The tile width in pixels
    /// </summary>
    public int TileWidth { get; set; }
    /// <summary>
    /// The tile height in pixels
    /// </summary>
    public int TileHeight { get; set; }
    /// <summary>
    /// The total amount of tiles
    /// </summary>
    public int TileCount { get; set; }
    /// <summary>
    /// The amount of horizontal tiles
    /// </summary>
    public int Columns { get; set; }
    /// <summary>
    /// The image definition used by the tileset
    /// </summary>
    public TiledImage Image { get; set; }
    /// <summary>
    /// The amount of spacing between the tiles in pixels
    /// </summary>
    public int Spacing { get; set; }
    /// <summary>
    /// The amount of margin between the tiles in pixels
    /// </summary>
    public int Margin { get; set; }
    /// <summary>
    /// An array of tile definitions
    /// </summary>
    /// <remarks>Not all tiles within a tileset have definitions. Only those with properties, animations, terrains, ...</remarks>
    public List<TiledTile> Tiles { get; set; }
    /// <summary>
    /// An array of terrain definitions
    /// </summary>
    public List<TiledTerrain> Terrains { get; set; }
    /// <summary>
    /// An array of tileset properties
    /// </summary>
    public TiledProperties Properties { get; set; }

    /// <summary>
    /// Returns an empty instance of TiledTileset
    /// </summary>
    public TiledTileset()
    {

    }

    /// <summary>
    /// Loads a tileset in TSX format and parses it
    /// </summary>
    /// <param name="path">The file path of the TSX file</param>
    /// <exception cref="TiledException">Thrown when the file could not be found or parsed</exception>
    public TiledTileset(string path)
    {
        string? content;

        // Check the file
        if (!File.Exists(path))
        {
            throw new TiledException($"{path} not found");
        }
        else
        {
            content = File.ReadAllText(path);
        }

        if (path.EndsWith(".tsx"))
        {
            ParseXml(content);
        }
        else
        {
            throw new TiledException("Unsupported file format");
        }
    }

    /// <summary>
    /// Can be used to parse the content of a TSX tileset manually instead of loading it using the constructor
    /// </summary>
    /// <param name="xml">The tmx file content as string</param>
    /// <exception cref="TiledException"></exception>
    public void ParseXml(string xml)
    {
        try
        {
            var document = new XmlDocument();
            document.LoadXml(xml);

            var nodeTileset = document.SelectSingleNode("tileset");
            var nodeImage = nodeTileset.SelectSingleNode("image");
            var nodesTile = nodeTileset.SelectNodes("tile");
            var nodesProperty = nodeTileset.SelectNodes("properties/property");
            var nodesTerrain = nodeTileset.SelectNodes("terraintypes/terrain");

            var attrMargin = nodeTileset.Attributes["margin"];
            var attrSpacing = nodeTileset.Attributes["spacing"];

            TiledVersion = nodeTileset.Attributes["tiledversion"].Value;
            Name = nodeTileset.Attributes["name"]?.Value;
            TileWidth = int.Parse(nodeTileset.Attributes["tilewidth"].Value);
            TileHeight = int.Parse(nodeTileset.Attributes["tileheight"].Value);
            TileCount = int.Parse(nodeTileset.Attributes["tilecount"].Value);
            Columns = int.Parse(nodeTileset.Attributes["columns"].Value);

            if (attrMargin != null) Margin = int.Parse(nodeTileset.Attributes["margin"].Value);
            if (attrSpacing != null) Spacing = int.Parse(nodeTileset.Attributes["spacing"].Value);
            if (nodeImage != null) Image = ParseImage(nodeImage);

            Tiles = ParseTiles(nodesTile);
            Properties = TiledProperties.ParseXml(nodesProperty);
            Terrains = ParseTerrains(nodesTerrain);
        }
        catch (Exception ex)
        {
            throw new TiledException("Unable to parse xml data, make sure the xml data represents a valid Tiled tileset", ex);
        }
    }

    private TiledImage ParseImage(XmlNode node)
    {
        var tiledImage = new TiledImage
        {
            Source = node.Attributes["source"].Value,
            Width = int.Parse(node.Attributes["width"].Value),
            Height = int.Parse(node.Attributes["height"].Value)
        };

        return tiledImage;
    }

    private List<TiledTileAnimation> ParseAnimations(XmlNodeList nodeList)
    {
        var result = new List<TiledTileAnimation>();

        foreach (XmlNode node in nodeList)
        {
            var animation = new TiledTileAnimation
            {
                Tileid = int.Parse(node.Attributes["tileid"].Value),
                Duration = int.Parse(node.Attributes["duration"].Value)
            };

            result.Add(animation);
        }

        return result;
    }

    private List<TiledTile> ParseTiles(XmlNodeList nodeList)
    {
        var result = new List<TiledTile>();

        foreach (XmlNode node in nodeList)
        {
            var nodesProperty = node.SelectNodes("properties/property");
            var nodesAnimation = node.SelectNodes("animation/frame");
            var nodeImage = node.SelectSingleNode("image");
            var nodesObjectGroup = node.SelectNodes("objectgroup");

            var tile = new TiledTile
            {
                Id = int.Parse(node.Attributes["id"].Value),
                Type = node.Attributes["type"]?.Value,
                Terrain = node.Attributes["terrain"]?.Value.Split(',').Cast<int>().ToList(),
                Properties = TiledProperties.ParseXml(nodesProperty),
                Animation = ParseAnimations(nodesAnimation),
                Objects = TiledObjectList.ParseXml(nodesObjectGroup)
            };

            if (nodeImage != null)
            {
                var tileImage = new TiledImage
                {
                    Width = int.Parse(nodeImage.Attributes["width"].Value),
                    Height = int.Parse(nodeImage.Attributes["height"].Value),
                    Source = nodeImage.Attributes["source"].Value
                };

                tile.Image = tileImage;
            }

            result.Add(tile);
        }

        return result;
    }

    private List<TiledTerrain> ParseTerrains(XmlNodeList nodeList)
    {
        var result = new List<TiledTerrain>();

        foreach (XmlNode node in nodeList)
        {
            var terrain = new TiledTerrain
            {
                Name = node.Attributes["name"].Value,
                Tile = int.Parse(node.Attributes["tile"].Value)
            };

            result.Add(terrain);
        }

        return result;
    }
}