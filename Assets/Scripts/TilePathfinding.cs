using System.Collections.Generic;
using System.Diagnostics;

class TilePathfinding : Pathfinding
{
    private TileGrid tileGrid;

    public TilePathfinding(TileGrid tileGrid) : base(tileGrid)
    {
        this.tileGrid = tileGrid;
    }

    public List<Tile> FindPath(Tile startTile, Tile goalTile)
    {
        List<Node> path = base.FindPath(startTile, goalTile);
        List<Tile> tilePath = new List<Tile>();

        if (path != null)
        {
            foreach (Node node in path)
            {
                tilePath.Add(node as Tile);
            }
        }

        return tilePath;
    }
}
