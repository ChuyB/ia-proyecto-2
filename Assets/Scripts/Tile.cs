using UnityEngine;
using UnityEngine.Tilemaps;

class Tile : Node
{
    public bool isWalkable;

    public Tile(Vector3 position, bool isWalkable) : base(position)
    {
        this.isWalkable = isWalkable;
    }
}

class TileGrid : Graph
{
    public Tile[,] tiles;
    public int width, height;
    private int offsetX, offsetY;

    public TileGrid(Tilemap tilemap)
    {

        // Offset to handle negative coords
        offsetX = Mathf.Abs(tilemap.cellBounds.xMin);
        offsetY = Mathf.Abs(tilemap.cellBounds.yMin);

        width = tilemap.cellBounds.xMax + offsetX;
        height = tilemap.cellBounds.yMax + offsetY;

        tiles = new Tile[width, height];

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
                bool isWalkable = tilemap.GetTile(cellPosition) != null;
                Tile tile = new Tile(worldPosition, isWalkable);
                tiles[x + offsetX, y + offsetY] = tile;
                AddNode(tile);
            }
        }

        CreateConnections();
    }

    private void CreateConnections()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (!tile.isWalkable) continue;

                if (x > 0 && tiles[x - 1, y].isWalkable) AddConnection(tile, tiles[x - 1, y], Vector3.Distance(tile.position, tiles[x - 1, y].position));
                if (x < width - 1 && tiles[x + 1, y].isWalkable) AddConnection(tile, tiles[x + 1, y], Vector3.Distance(tile.position, tiles[x + 1, y].position));
                if (y > 0 && tiles[x, y - 1].isWalkable) AddConnection(tile, tiles[x, y - 1], Vector3.Distance(tile.position, tiles[x, y - 1].position));
                if (y < height - 1 && tiles[x, y + 1].isWalkable) AddConnection(tile, tiles[x, y + 1], Vector3.Distance(tile.position, tiles[x, y + 1].position));
            }
        }
    }
}
