using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using TiledCS.Collections;
using TiledCS.Layers;

namespace TiledCS
{
    /// <summary>
    /// Represents a Tiled map
    /// </summary>
    public class TiledMap
    {
        public const uint FlippedHorizontallyFlag = 0b10000000000000000000000000000000;
        public const uint FlippedVerticallyFlag = 0b01000000000000000000000000000000;
        public const uint FlippedDiagonallyFlag = 0b00100000000000000000000000000000;

        /// <summary>
        /// How many times we shift the FLIPPED flags to the right in order to store it in a byte.
        /// For example: 0b10100000000000000000000000000000 >> SHIFT_FLIP_FLAG_TO_BYTE = 0b00000101
        /// </summary>
        public const int ShiftFlipFlagToByte = 29;

        /// <summary>
        /// Returns the Tiled version used to create this map
        /// </summary>
        public string TiledVersion { get; set; }
        /// <summary>
        /// Returns an array of properties defined in the map
        /// </summary>
        public TiledProperties Properties { get; set; }
        /// <summary>
        /// Returns an array of tileset definitions in the map
        /// </summary>
        public TiledMapTilesetList Tilesets { get; set; }
        /// <summary>
        /// Returns an array of layers or null if none were defined
        /// </summary>
        public TiledLayerList Layers { get; set; }
        /// <summary>
        /// Returns an array of groups or null if none were defined
        /// </summary>
        public TiledGroupList Groups { get; set; }
        /// <summary>
        /// Returns the defined map orientation as a string
        /// </summary>
        public string Orientation { get; set; }
        /// <summary>
        /// Returns the render order as a string
        /// </summary>
        public string RenderOrder { get; set; }
        /// <summary>
        /// The amount of horizontal tiles
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The amount of vertical tiles
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The tile width in pixels
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// The tile height in pixels
        /// </summary>
        public int TileHeight { get; set; }
        /// <summary>
        /// Returns true if the map is configured as infinite
        /// </summary>
        public bool Infinite { get; set; }
        /// <summary>
        /// Returns the defined map background color as a hex string
        /// </summary>
        public string BackgroundColor { get; set; }
        /// <summary>
        /// Returns an empty instance of TiledMap
        /// </summary>
        public TiledMap()
        {

        }

        /// <summary>
        /// Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="path">The path to the tmx file</param>
        /// <exception cref="TiledException">Thrown when the map could not be loaded or is not in a correct format</exception>
        public TiledMap(string path)
        {
            var content = "";

            // Check the file
            if (!File.Exists(path))
            {
                throw new TiledException($"{path} not found");
            }
            else
            {
                content = File.ReadAllText(path);
            }

            if (path.EndsWith(".tmx"))
            {
                ParseXml(content);
            }
            else
            {
                throw new TiledException("Unsupported file format");
            }
        }

        /// <summary>
        /// Can be used to parse the content of a TMX map manually instead of loading it using the constructor
        /// </summary>
        /// <param name="xml">The tmx file content as string</param>
        /// <exception cref="TiledException"></exception>
        public void ParseXml(string xml)
        {
            try
            {
                // Load the xml document
                var document = new XmlDocument();
                document.LoadXml(xml);

                var nodeMap = document.SelectSingleNode("map");
                var nodesProperty = nodeMap.SelectNodes("properties/property");
                var nodesTileset = nodeMap.SelectNodes("tileset");
                var nodesGroup = nodeMap.SelectNodes("group");

                TiledVersion = nodeMap.Attributes["tiledversion"].Value;
                Orientation = nodeMap.Attributes["orientation"].Value;
                RenderOrder = nodeMap.Attributes["renderorder"].Value;
                BackgroundColor = nodeMap.Attributes["backgroundcolor"]?.Value;
                Infinite = nodeMap.Attributes["infinite"].Value == "1";

                Width = int.Parse(nodeMap.Attributes["width"].Value);
                Height = int.Parse(nodeMap.Attributes["height"].Value);
                TileWidth = int.Parse(nodeMap.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeMap.Attributes["tileheight"].Value);

                Properties = TiledProperties.ParseXml(nodesProperty);
                Tilesets = TiledMapTilesetList.ParseXml(nodesTileset);
                Layers = TiledLayerList.ParseXml(nodeMap);
                Groups = TiledGroupList.ParseXml(nodesGroup);
            }
            catch (Exception ex)
            {
                throw new TiledException("Unable to parse xml data, make sure the xml data represents a valid Tiled map", ex);
            }
        }

        /* HELPER METHODS */
        /// <summary>
        /// Locates the right TiledMapTileset object for you within the Tilesets array
        /// </summary>
        /// <param name="gid">A value from the TiledLayer.data array</param>
        /// <returns>An element within the Tilesets array or null if no match was found</returns>
        public TiledMapTileset GetTiledMapTileset(int gid)
        {
            if (Tilesets == null)
            {
                return null;
            }

            for (var i = 0; i < Tilesets.Count; i++)
            {
                if (i < Tilesets.Count - 1)
                {
                    var gid1 = Tilesets[i + 0].FirstGid;
                    var gid2 = Tilesets[i + 1].FirstGid;

                    if (gid >= gid1 && gid < gid2)
                    {
                        return Tilesets[i];
                    }
                }
                else
                {
                    return Tilesets[i];
                }
            }

            return new TiledMapTileset();
        }
        /// <summary>
        /// Loads external tilesets and matches them to firstGids from elements within the Tilesets array
        /// </summary>
        /// <param name="src">The folder where the TiledMap file is located</param>
        /// <returns>A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset object</returns>
        public Dictionary<int, TiledTileset> GetTiledTilesets(string src)
        {
            var tilesets = new Dictionary<int, TiledTileset>();
            var info = new FileInfo(src);
            var srcFolder = info.Directory;

            if (Tilesets == null)
            {
                return tilesets;
            }

            foreach (var mapTileset in Tilesets)
            {
                var path = $"{srcFolder}/{mapTileset.Source}";

                if (File.Exists(path))
                {
                    tilesets.Add(mapTileset.FirstGid, new TiledTileset(path));
                }
            }

            return tilesets;
        }
        /// <summary>
        /// Locates a specific TiledTile object
        /// </summary>
        /// <param name="mapTileset">An element within the Tilesets array</param>
        /// <param name="tileset">An instance of the TiledTileset class</param>
        /// <param name="gid">An element from within a TiledLayer.data array</param>
        /// <returns>An entry of the TiledTileset.tiles array or null if none of the tile id's matches the gid</returns>
        /// <remarks>Tip: Use the GetTiledMapTileset and GetTiledTilesets methods for retrieving the correct TiledMapTileset and TiledTileset objects</remarks>
        public TiledTile GetTiledTile(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Id == gid - mapTileset.FirstGid)
                {
                    return tile;
                }
            }

            return null;
        }
        /// <summary>
        /// This method can be used to figure out the x and y position on a Tileset image for rendering tiles. 
        /// </summary>
        /// <param name="mapTileset">An element of the Tilesets array</param>
        /// <param name="tileset">An instance of the TiledTileset class</param>
        /// <param name="gid">An element within a TiledLayer.data array</param>
        /// <returns>An int array of length 2 containing the x and y position of the source rect of the tileset image. Multiply the values by the tile width and height in pixels to get the actual x and y position. Returns null if the gid was not found</returns>
        /// <remarks>This method currently doesn't take margin into account</remarks>
        [Obsolete("Please use GetSourceRect instead because with future versions of Tiled this method may no longer be sufficient")]
        public List<int> GetSourceVector(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.FirstGid)
                {
                    return new List<int> { tileHor, tileVert };
                }

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.Width / tileset.TileWidth)
                {
                    tileHor = 0;
                    tileVert++;
                }
            }

            return null;
        }

        /// <summary>
        /// This method can be used to figure out the source rect on a Tileset image for rendering tiles.
        /// </summary>
        /// <param name="mapTileset"></param>
        /// <param name="tileset"></param>
        /// <param name="gid"></param>
        /// <returns>An instance of the class TiledSourceRect that represents a rectangle. Returns null if the provided gid was not found within the tileset.</returns>
        public TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.FirstGid)
                {
                    var result = new TiledSourceRect
                    {
                        X = tileHor * tileset.TileWidth,
                        Y = tileVert * tileset.TileHeight,
                        Width = tileset.TileWidth,
                        Height = tileset.TileHeight
                    };

                    return result;
                }

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.Width / tileset.TileWidth)
                {
                    tileHor = 0;
                    tileVert++;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledTileLayer layer, int tileHor, int tileVert)
        {
            return IsTileFlippedHorizontal(layer, tileHor + (tileVert * layer.Width));
        }
        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledTileLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FlippedHorizontallyFlag >> ShiftFlipFlagToByte)) > 0;
        }
        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledTileLayer layer, int tileHor, int tileVert)
        {
            return IsTileFlippedVertical(layer, tileHor + (tileVert * layer.Width));
        }
        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledTileLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FlippedVerticallyFlag >> ShiftFlipFlagToByte)) > 0;
        }
        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledTileLayer layer, int tileHor, int tileVert)
        {
            return IsTileFlippedDiagonal(layer, tileHor + (tileVert * layer.Width));
        }
        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledTileLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FlippedDiagonallyFlag >> ShiftFlipFlagToByte)) > 0;
        }
    }
}