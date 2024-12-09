using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    // both tilemaps combined make up the entire map
    [SerializeField] private Tilemap placableTilemap;
    [SerializeField] private Tilemap nonPlacableTilemap;

    private Dictionary<Vector3Int, bool> gridStatus = new Dictionary<Vector3Int, bool>();

    void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // iterate through all tiles in the placable tilemap
        foreach (Vector3Int position in placableTilemap.cellBounds.allPositionsWithin)
        {
            if (placableTilemap.HasTile(position))
            {
                gridStatus[position] = true; // placable and unoccupied
            }
        }

        // iterate through non-placable tiles
        foreach (Vector3Int position in nonPlacableTilemap.cellBounds.allPositionsWithin)
        {
            if (nonPlacableTilemap.HasTile(position))
            {
                gridStatus[position] = false; // non-placable
            }
        }
    }

    public bool IsTilePlacable(Vector3Int gridPosition)
    {
        return gridStatus.ContainsKey(gridPosition) && gridStatus[gridPosition];
    }

    public void MarkTileOccupied(Vector3Int gridPosition)
    {
        if (gridStatus.ContainsKey(gridPosition))
        {
            gridStatus[gridPosition] = false; // mark as occupied
        }
    }

    public void MarkTileUnoccupied(Vector3Int gridPosition)
    {
        if (gridStatus.ContainsKey(gridPosition))
        {
            gridStatus[gridPosition] = true; // mark as unoccupied
        }
    }
}
