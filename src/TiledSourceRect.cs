namespace TiledCS
{
    /// <summary>
    /// Used as data type for the GetSourceRect method. Represents basically a rectangle.
    /// </summary>
    public class TiledSourceRect
    {
        /// <summary>
        /// The x position in pixels from the tile location in the source image
        /// </summary>
        public int X;
        /// <summary>
        /// The y position in pixels from the tile location in the source image
        /// </summary>
        public int Y;
        /// <summary>
        /// The width in pixels from the tile in the source image
        /// </summary>
        public int Width;
        /// <summary>
        /// The height in pixels from the tile in the source image
        /// </summary>
        public int Height;
    }
}