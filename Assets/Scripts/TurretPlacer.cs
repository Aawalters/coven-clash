using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private Tilemap placableTilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = placableTilemap.WorldToCell(mouseWorldPos);

            if (gridManager.IsTilePlacable(gridPosition))
            {
                PlaceTurret(gridPosition);
                gridManager.MarkTileOccupied(gridPosition);
            }
            else
            {
                Debug.Log("Tile is not placable or already occupied!");
            }
        }
    }

    private void PlaceTurret(Vector3Int gridPosition)
    {
        Vector3 worldPosition = placableTilemap.GetCellCenterWorld(gridPosition);
        Instantiate(turretPrefab, worldPosition, Quaternion.identity);
    }
}
