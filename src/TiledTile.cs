using System.Collections.Generic;
using TiledCS.Collections;
using TiledCS.Objects;

namespace TiledCS
{
    /// <summary>
    /// Represents a tile within a tileset
    /// </summary>
    /// <remarks>These are not defined for all tiles within a tileset, only the ones with properties, terrains and animations.</remarks>
    public class TiledTile
    {
        /// <summary>
        /// The tile id
        /// </summary>
        public int Id;
        /// <summary>
        /// The custom tile type, set by the user
        /// </summary>
        public string Type;
        /// <summary>
        /// The terrain definitions as int array. These are indices indicating what part of a terrain and which terrain this tile represents.
        /// </summary>
        /// <remarks>In the map file empty space is used to indicate null or no value. However, since it is an int array I needed something so I decided to replace empty values with -1.</remarks>
        public List<int> Terrain;
        /// <summary>
        /// An array of properties. Is null if none were defined.
        /// </summary>
        public TiledProperties Properties;
        /// <summary>
        /// An array of tile animations. Is null if none were defined. 
        /// </summary>
        public List<TiledTileAnimation> Animation;
        /// <summary>
        /// The individual tile image
        /// </summary>
        public TiledImage Image;
        /// <summary>
        /// Objects that this tile can have
        /// </summary>
        public TiledObjectList Objects;
    }
}