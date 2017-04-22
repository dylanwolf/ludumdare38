using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingTile : MonoBehaviour {

    public TileCoord Tile = new TileCoord();
    public TileCoord OriginalTile = new TileCoord();

    SpriteRenderer _r;
    Collider2D _c;

    private void Awake()
    {
        _t = transform;
        _r = GetComponent<SpriteRenderer>();
        _c = GetComponent<Collider2D>();
    }

    public void ConfigureTile(TileType tileType, Sprite sprite)
    {
        _r.sprite = sprite;
        _c.enabled = tileType == TileType.Water;
        gameObject.layer = _c.enabled ? LayerManager.Water : LayerManager.Default;
    }

    Transform _t;
    Vector3 tmpV3;
    public void PlaceTile()
    {
        tmpV3 = _t.localPosition;
        tmpV3.x = WrappingWorld.Current.TileSizeX * Tile.X;
        tmpV3.y = WrappingWorld.Current.TileSizeY * Tile.Y;
        _t.localPosition = tmpV3;
    }
}
