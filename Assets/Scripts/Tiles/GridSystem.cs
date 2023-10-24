using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem<TGridObject>
{
    private int width, height; 
    private float cellWidth, cellHeight;

    //This 2D array will contain coordinates for the ENTIRE grid, including "invalid" tiles.
    private Vector2Int[,] gridTileVectors;

    //The Grid System keeps an internal list of valid tiles.
    public List<Vector2Int> validTiles;
    private GameObject gridHolder;
    private Tilemap ground;
    private Tilemap walls;
    private TGridObject[,] gridObjects;

    //Create a grid.
    public GridSystem(int width, int height, float cellWidth, float cellHeight, GameObject gridHolder, Tilemap ground, Tilemap walls, Func<GridSystem<TGridObject>, Vector2Int, TGridObject> CreateGridObject )
    {
        this.width = width;
        this.height = height;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.gridHolder = gridHolder;
        this.ground = ground;
        this.walls = walls;

        gridTileVectors = new Vector2Int[width, height];
        gridObjects = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Log each grid "cell" in gridTileVectors.
                Vector2Int gridVector = new Vector2Int(x, y);
                gridTileVectors[x, y] = gridVector;
                gridObjects[x, y] = CreateGridObject(this, gridVector);
            }
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridVector)
    {
        Vector3Int gridCell = new Vector3Int(gridVector.x, gridVector.y, 0);
        //Convert a Vector2 grid cell into its corresponding world location.
        Vector3 worldPosition = new Vector3 (gridVector.x * cellWidth, gridVector.y * cellHeight, 0);
        return worldPosition;
    }

    public Vector3 GetCenteredWorldPosition(Vector2Int gridVector)
    {
        //Finds the grid vector's coordinates in worldspace...
        Vector3 realVector = GetWorldPosition(gridVector);

        //... then finds the closest cell in the tileset...
        Vector3Int tilesetVector = ground.WorldToCell(realVector);

        //...Then finds the center of the closest grid cell.
        return ground.GetCellCenterWorld(tilesetVector);
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        //Convert a Vector3 position into a Vector2 corresponding to its grid cell.
        //Uses FloorToInt because grid vectors mark the bottom-left corner. If the click is anywhere in the tile, it should round down to that tile's Vector2.
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x / cellWidth), Mathf.FloorToInt(worldPosition.y / cellHeight));
    } 

    public void CreateDebugObject(GameObject debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridVector = new Vector2Int(x, y);
                if (CheckTile(gridVector))
                {
                    GameObject debugObject = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridVector), Quaternion.identity, gridHolder.transform);
                    DebugTile debugTile = debugObject.GetComponent<DebugTile>();
                    debugTile.SetGridObject(GetGridObject(gridVector) as GridObject);
                }
            }
        }
    }

    public bool CheckTile(Vector2Int gridVector)
    {
        //Get the input vector's location in worldspace...
        Vector3 worldVector = GetWorldPosition(gridVector);
        //... then find the corresponding tileset tile...
        Vector3Int tileVector = ground.WorldToCell(worldVector);

        //If there is not a ground tile, or if there is a wall tile, returns false. This automatically rules out out-of-bounds tiles.
        if (!ground.HasTile(tileVector) || walls.HasTile(tileVector))
            return false;
        else return true;
    }

    public TGridObject GetGridObject(Vector2Int gridVector)
    {
        return gridObjects[gridVector.x, gridVector.y];
    }
}