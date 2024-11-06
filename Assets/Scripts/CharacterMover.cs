using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class CharacterMover : MonoBehaviour
{
    public float tileSize = 1.0f;
    public Tilemap tilemap;
    public Color color = Color.blue;
    public float speed = 2.0f;

    private Animator animator;

    protected TileGrid tileGrid;
    protected TilePathfinding pathfinding;
    protected List<Tile> path;
    protected int currentStep;
    protected PathDrawer pathDrawer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePathMovement(Vector2 targetPosition)
    {
        path = new List<Tile>();
        if (pathDrawer == null)
        {
            pathDrawer = gameObject.AddComponent<PathDrawer>();
        }

        tileGrid = new TileGrid(tilemap);
        pathfinding = new TilePathfinding(tileGrid);

        int offsetX = Mathf.Abs(tilemap.cellBounds.xMin);
        int offsetY = Mathf.Abs(tilemap.cellBounds.yMin);

        int startX = Mathf.RoundToInt(transform.position.x / tileSize) + offsetX;
        int startY = Mathf.RoundToInt(transform.position.y / tileSize) + offsetY;

        int goalX = Mathf.RoundToInt(targetPosition.x / tileSize) + offsetX;
        int goalY = Mathf.RoundToInt(targetPosition.y / tileSize) + offsetY;

        Tile startTile = tileGrid.tiles[startX, startY];
        Tile goalTile = tileGrid.tiles[goalX, goalY];

        path = pathfinding.FindPath(startTile, goalTile);

        currentStep = 0;
        pathDrawer.DrawPath(path, color);
    }
}
