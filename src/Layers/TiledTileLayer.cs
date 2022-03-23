using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiledCS.Collections;

namespace TiledCS.Layers
{
    public class TiledTileLayer : TiledLayer
    {
        /// <summary>
        /// Total horizontal tiles
        /// </summary>
        public int Width;
        /// <summary>
        /// Total vertical tiles
        /// </summary>
        public int Height;
        /// <summary>
        /// An int array of gid numbers which define which tile is being used where. The length of the array equals the layer width * the layer height. Is null when the layer is not a tilelayer.
        /// </summary>
        public int[] Data;
        /// <summary>
        /// A parallel array to data which stores the rotation flags of the tile.
        /// Bit 3 is horizontal flip,
        /// bit 2 is vertical flip, and
        /// bit 1 is (anti) diagonal flip.
        /// Is null when the layer is not a tilelayer.
        /// </summary>
        public byte[] DataRotationFlags;

        public static TiledTileLayer ParseXml(XmlNode node)
        {
            var nodeData = node.SelectSingleNode("data");
            var nodesProperty = node.SelectNodes("properties/property");
            var encoding = nodeData.Attributes["encoding"].Value;
            var attrVisible = node.Attributes["visible"];
            var attrLocked = node.Attributes["locked"];
            var attrTint = node.Attributes["tintcolor"];
            var attrOffsetX = node.Attributes["offsetx"];
            var attrOffsetY = node.Attributes["offsety"];

            var tiledLayer = new TiledTileLayer
            {
                Id = int.Parse(node.Attributes["id"].Value),
                Name = node.Attributes["name"].Value,
                Height = int.Parse(node.Attributes["height"].Value),
                Width = int.Parse(node.Attributes["width"].Value),
                Type = "tilelayer",
                Visible = true
            };

            if (attrVisible != null) tiledLayer.Visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.Locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.TintColor = attrTint.Value;
            if (attrOffsetX != null) tiledLayer.OffsetX = int.Parse(attrOffsetX.Value);
            if (attrOffsetY != null) tiledLayer.OffsetY = int.Parse(attrOffsetY.Value);
            if (nodesProperty != null) tiledLayer.Properties = TiledProperties.ParseXml(nodesProperty);

            if (encoding == "csv")
            {
                var csvs = nodeData.InnerText.Split(',');

                tiledLayer.Data = new int[csvs.Length];
                tiledLayer.DataRotationFlags = new byte[csvs.Length];

                // Parse the comma separated csv string and update the inner data as well as the data rotation flags
                for (var i = 0; i < csvs.Length; i++)
                {
                    var rawId = uint.Parse(csvs[i]);
                    var hor = ((rawId & TiledMap.FlippedHorizontallyFlag));
                    var ver = ((rawId & TiledMap.FlippedVerticallyFlag));
                    var dia = ((rawId & TiledMap.FlippedDiagonallyFlag));
                    tiledLayer.DataRotationFlags[i] = (byte)((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);

                    // assign data to rawID with the rotation flags cleared
                    tiledLayer.Data[i] = (int)(rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag | TiledMap.FlippedDiagonallyFlag));
                }
            }
            else if (encoding == "base64")
            {
                var compression = nodeData.Attributes["compression"]?.Value;

                using (var base64DataStream = new MemoryStream(Convert.FromBase64String(nodeData.InnerText)))
                {
                    if (compression == null)
                    {
                        // Parse the decoded bytes and update the inner data as well as the data rotation flags
                        var rawBytes = new byte[4];
                        tiledLayer.Data = new int[base64DataStream.Length];
                        tiledLayer.DataRotationFlags = new byte[base64DataStream.Length];

                        for (var i = 0; i < base64DataStream.Length; i++)
                        {
                            base64DataStream.Read(rawBytes, 0, rawBytes.Length);
                            var rawId = BitConverter.ToUInt32(rawBytes, 0);
                            var hor = ((rawId & TiledMap.FlippedHorizontallyFlag));
                            var ver = ((rawId & TiledMap.FlippedVerticallyFlag));
                            var dia = ((rawId & TiledMap.FlippedDiagonallyFlag));
                            tiledLayer.DataRotationFlags[i] = (byte)((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);

                            // assign data to rawID with the rotation flags cleared
                            tiledLayer.Data[i] = (int)(rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag | TiledMap.FlippedDiagonallyFlag));
                        }
                    }
                    else if (compression == "zlib")
                    {
                        // .NET doesn't play well with the headered zlib data that Tiled produces,
                        // so we have to manually skip the 2-byte header to get what DeflateStream's looking for
                        // Should an external library be used instead of this hack?
                        base64DataStream.ReadByte();
                        base64DataStream.ReadByte();

                        using (var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress))
                        {
                            // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                            var decompressedDataBuffer = new byte[4]; // size of each tile
                            var dataRotationFlagsList = new List<byte>();
                            var layerDataList = new List<int>();

                            while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length)
                            {
                                var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                                var hor = ((rawId & TiledMap.FlippedHorizontallyFlag));
                                var ver = ((rawId & TiledMap.FlippedVerticallyFlag));
                                var dia = ((rawId & TiledMap.FlippedDiagonallyFlag));
                                dataRotationFlagsList.Add((byte)((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte));

                                // assign data to rawID with the rotation flags cleared
                                layerDataList.Add((int)(rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag | TiledMap.FlippedDiagonallyFlag)));
                            }

                            tiledLayer.Data = layerDataList.ToArray();
                            tiledLayer.DataRotationFlags = dataRotationFlagsList.ToArray();
                        }
                    }
                    else if (compression == "gzip")
                    {
                        using (var decompressionStream = new GZipStream(base64DataStream, CompressionMode.Decompress))
                        {
                            // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                            var decompressedDataBuffer = new byte[4]; // size of each tile
                            var dataRotationFlagsList = new List<byte>();
                            var layerDataList = new List<int>();

                            while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length)
                            {
                                var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                                var hor = ((rawId & TiledMap.FlippedHorizontallyFlag));
                                var ver = ((rawId & TiledMap.FlippedVerticallyFlag));
                                var dia = ((rawId & TiledMap.FlippedDiagonallyFlag));

                                dataRotationFlagsList.Add((byte)((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte));

                                // assign data to rawID with the rotation flags cleared
                                layerDataList.Add((int)(rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag | TiledMap.FlippedDiagonallyFlag)));
                            }

                            tiledLayer.Data = layerDataList.ToArray();
                            tiledLayer.DataRotationFlags = dataRotationFlagsList.ToArray();
                        }
                    }
                    else
                    {
                        throw new TiledException("Zstandard compression is currently not supported");
                    }
                }
            }
            else
            {
                throw new TiledException("Only CSV and Base64 encodings are currently supported");
            }

            return tiledLayer;
        }
    }
}
