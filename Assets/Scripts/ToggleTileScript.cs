using UnityEngine;
using UnityEngine.Tilemaps;

public class ToggleTileScript : Unlockable
{
    public Tilemap tilemap;
    public TileBase disabledTile;
    public TileBase enabledTile;

    protected override void Unlock()
    {
        ChangeTiles(enabledTile);
    }

    protected override void Lock()
    {
        ChangeTiles(disabledTile);
    }

    private void ChangeTiles(TileBase tileType)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilemap.SetTile(pos, tileType);
            }
        }
    }
}