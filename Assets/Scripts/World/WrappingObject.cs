using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingObject : MonoBehaviour {

    [System.NonSerialized]
    public TileCoord Tile = new TileCoord();
    [System.NonSerialized]
    public TileCoord LastTile = new TileCoord();
    [System.NonSerialized]
    public TileCoord OriginalTile = new TileCoord();

    [System.NonSerialized]
    public string LoadedObjectDefinition;
    public virtual void OverwriteObjectDefinition(WorldGenerator.WorldObjectDefinition obj, TileWrappingMapper mapper)
    {
        obj.Position.x = transform.position.x + (WrappingWorld.Current.TileSizeX * (OriginalTile.X - mapper.GetNewX(OriginalTile.X)));
        obj.Position.y = transform.position.y + (WrappingWorld.Current.TileSizeY * (OriginalTile.Y - mapper.GetNewY(OriginalTile.Y)));
    }

    SpriteRenderer _r;

    public virtual void Start()
    {
        _r = GetComponent<SpriteRenderer>();
    }

    public void GetOriginalTile(TileWrappingMapper mapper)
    {
        LastTile.X = OriginalTile.X = mapper.GetOldX((int)Mathf.Floor(transform.position.x / WrappingWorld.Current.TileSizeX));
        LastTile.Y = OriginalTile.Y = mapper.GetOldY((int)Mathf.Floor(transform.position.y / WrappingWorld.Current.TileSizeY));
    }

    public void SetSortOrder(float startY)
    {
        if (_r != null)
            _r.sortingOrder = (int)((transform.position.y - startY) * -100) + 5000;
    }

    [System.NonSerialized]
    protected Vector3 tmpV3;
    public void Wrap(TileCoord centerTile, TileCoord offset, TileWrappingMapper mapper)
    {
        Tile.X = mapper.GetNewX(OriginalTile.X);
        Tile.Y = mapper.GetNewY(OriginalTile.Y);

        tmpV3 = transform.position;

        if (Tile.X != LastTile.X)
        {
            tmpV3.x -= (WrappingWorld.Current.TileSizeX * (LastTile.X - Tile.X));
        }

        if (Tile.Y != LastTile.Y)
            tmpV3.y -= (WrappingWorld.Current.TileSizeY * (LastTile.Y - Tile.Y));

        LastTile.X = Tile.X;
        LastTile.Y = Tile.Y;

        transform.position = tmpV3;
    }

}
