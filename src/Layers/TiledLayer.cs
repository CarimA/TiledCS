using TiledCS.Collections;

namespace TiledCS.Layers;

/// <summary>
/// Represents a tile layer as well as an object layer within a tile map
/// </summary>
public abstract class TiledLayer
{
    /// <summary>
    /// The layer id
    /// </summary>
    public int Id;
    /// <summary>
    /// The layer name
    /// </summary>
    public string Name;
    /// <summary>
    /// The layer type. Usually this is "objectgroup" or "tilelayer".
    /// </summary>
    public string Type;
    /// <summary>
    /// The tint color set by the user in hex code
    /// </summary>
    public string TintColor;
    /// <summary>
    /// Defines if the layer is visible in the editor
    /// </summary>
    public bool Visible;
    /// <summary>
    /// Is true when the layer is locked
    /// </summary>
    public bool Locked;
    /// <summary>
    /// The horizontal offset
    /// </summary>
    public int OffsetX;
    /// <summary>
    /// The vertical offset
    /// </summary>
    public int OffsetY;
    /// <summary>
    /// The layer properties if set
    /// </summary>
    public TiledProperties Properties;

}