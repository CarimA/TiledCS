# Main Differences from TiledCS
 - All properties naming conventions matches that of C#'s
 - Map/Layer/Tile properties are now a dictionary of objects
 - All classes have been split up into their own files and moved to more organised namespaces
 - Layers and Objects now have a specific type (TiledTileLayer/TiledObjectLayer/TiledImageLayer for layers and TiledRectangleObject/TiledPolygonObject/TiledPolylineObject/TiledPointObject/TiledEllipseObject) for better type safety and remove the need for constant null checking (as properties not part of a layer simply do not exist on their respective types)
 - XML parsing logic has been moved to each individual type; it's easier to find what you want where you want it

 I don't particularly care to add comments as I'm making changes for my own uses, but if you find these changes useful and want to add some, be my guest.

## Credits
* [TiledCS by TheBoneJarmer](https://github.com/TheBoneJarmer/TiledCS)

## License
[MIT](LICENSE)
