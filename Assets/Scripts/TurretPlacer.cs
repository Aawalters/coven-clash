using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TurretPlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private Tilemap placableTilemap;
    [SerializeField] private Tilemap highlightTileMap;
    [SerializeField] private Tile redTile;
    [SerializeField] private Tile greenTile;
    [SerializeField] private float outlineThickness = 0.1f;
    private bool isPlacingTurret = false;
    private GameObject currentTurret;
    private GameObject previewTurret;
    private Vector3Int previewPosition;
    private List<LineRenderer> outlineRenderers = new List<LineRenderer>();

    void Update()
    {
        if (isPlacingTurret)
        {
            //UpdateGridHighlights();

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = placableTilemap.WorldToCell(mouseWorldPos);
            UpdatePreviewTurretPosition(gridPosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (gridManager.IsTilePlacable(gridPosition))
                {
                    PlaceTurret(gridPosition);
                    gridManager.MarkTileOccupied(gridPosition);
                    ExitTurretPlacingMode();
                }
                else
                {
                    Debug.Log("Tile is not placable or already occupied!");
                }
            }
        }
    }

    public void EnterTurretPlacingMode(GameObject turretPrefab)
    {
        isPlacingTurret = true;
        currentTurret = turretPrefab;
        highlightTileMap.gameObject.SetActive(true);

        previewTurret = Instantiate(currentTurret);
        previewTurret.SetActive(true);

        UpdateGridHighlights();
    }

    public void ExitTurretPlacingMode()
    {
        isPlacingTurret = false;
        highlightTileMap.gameObject.SetActive(false);

        if (previewTurret != null)
        {
            Destroy(previewTurret);
        }

        ClearOutlines();
    }

    private void UpdateGridHighlights()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = placableTilemap.WorldToCell(mouseWorldPos);

        highlightTileMap.ClearAllTiles();
        ClearOutlines();

        BoundsInt bounds = placableTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (gridManager.IsTilePlacable(tilePosition))
                {
                    highlightTileMap.SetTile(tilePosition, greenTile); // valid placement
                    DrawTileOutline(tilePosition, Color.white); // green outline for valid tiles
                }
                else
                {
                    highlightTileMap.SetTile(tilePosition, redTile); // invalid placement
                    DrawTileOutline(tilePosition, Color.white); // red outline for invalid tiles
                }
            }
        }
    
    }

    private void UpdatePreviewTurretPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = placableTilemap.GetCellCenterWorld(gridPosition);
        Turret hopefullyTurret = previewTurret.GetComponent<Turret>();
        // gotta check if the position is valid
        if (gridManager.IsTilePlacable(gridPosition))
        {
            hopefullyTurret.transform.position = worldPosition;
            hopefullyTurret._spriteRenderer.material.color = Color.white; // show valid placement
        }
        else
        {
            hopefullyTurret.transform.position = worldPosition;
            hopefullyTurret._spriteRenderer.material.color = Color.red; // show invalid placement
        }
    }

    private void PlaceTurret(Vector3Int gridPosition)
    {
        Vector3 worldPosition = placableTilemap.GetCellCenterWorld(gridPosition);
        GameObject newTurret = Instantiate(turretPrefab, worldPosition, Quaternion.identity);
        newTurret.SetActive(true);
        
    }

    // draw the outline for a given tile at the grid position
    private void DrawTileOutline(Vector3Int gridPosition, Color outlineColor)
    {
        Vector3 worldPosition = placableTilemap.GetCellCenterWorld(gridPosition);
        Vector3 size = placableTilemap.cellSize;

        // create or get a LineRenderer for this tile
        LineRenderer lineRenderer = new GameObject("TileOutline").AddComponent<LineRenderer>();
        outlineRenderers.Add(lineRenderer);

        // set the line renderer properties
        lineRenderer.startWidth = outlineThickness;
        lineRenderer.endWidth = outlineThickness;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use default sprite shader
        lineRenderer.startColor = outlineColor;
        lineRenderer.endColor = outlineColor;
        lineRenderer.positionCount = 5;

        // define the four corners of the rectangle for the tile outline
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(worldPosition.x - size.x / 2, worldPosition.y - size.y / 2, 0);
        corners[1] = new Vector3(worldPosition.x + size.x / 2, worldPosition.y - size.y / 2, 0);
        corners[2] = new Vector3(worldPosition.x + size.x / 2, worldPosition.y + size.y / 2, 0);
        corners[3] = new Vector3(worldPosition.x - size.x / 2, worldPosition.y + size.y / 2, 0);
        corners[4] = corners[0]; // gotta close the loop!

        // set the positions of the outline
        lineRenderer.SetPositions(corners);
    }

    // clear all the outlines
    private void ClearOutlines()
    {
        foreach (LineRenderer lr in outlineRenderers)
        {
            Destroy(lr.gameObject);
        }
        outlineRenderers.Clear();
    }
}
