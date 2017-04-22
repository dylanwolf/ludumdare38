using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WrappingWorld : MonoBehaviour {

    public static WrappingWorld Current;

    #region World Configuration
    public float TileSizeX;
    public float TileSizeY;
    #endregion

    string loadedWorld;

    public Sprite[] TileTypeSprites;

    [System.NonSerialized]
    public TileCoord GridSize = new TileCoord();

    public void Awake()
    {
        Current = this;
    }

    public void Start()
    {
    }

    bool isLoading = false;
    public void LoadWorld(WorldGenerator.WorldDefinition world)
    {
        isLoading = true;
        ResetWorld();
        RocketToSpace.Current.gameObject.SetActive(true);
        PlayerCharacter.Current.gameObject.SetActive(true);
        PlayerCharacter.Current.ResetWorld();
        RocketToSpace.Current.ResetWorld();

        loadedWorld = world.ID;

        int width = world.Tiles.GetLength(0);
        int height = world.Tiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnTile(x, y, world.Tiles[x,y]);
            }
        }

        for (int i = 0; i < world.ObjectList.Count; i++)
        {
            switch (world.ObjectList[i].Type)
            {
                case ObjectType.FuelPickup:
                    FuelPickup.Spawn(world.ObjectList[i].ID, world.ObjectList[i].Position);
                    break;

                case ObjectType.FuelTree:
                    FuelTree.Spawn(world.ObjectList[i].ID, world.ObjectList[i].Position, world.ObjectList[i].RemainingIntValue);
                    break;
            }
        }

        Register(RocketToSpace.Current);
        isLoading = false;
    }

    public void SaveWorldState()
    {
        WorldGenerator.WorldDefinition world = WorldGenerator.GetWorld(loadedWorld);

        world.ResetSaveMarkers();
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].LoadedObjectDefinition == null || Objects[i] == RocketToSpace.Current)
                continue;

            if (world.Objects.ContainsKey(Objects[i].LoadedObjectDefinition))
            {
                Objects[i].OverwriteObjectDefinition(world.Objects[Objects[i].LoadedObjectDefinition], mapper);
                world.Objects[Objects[i].LoadedObjectDefinition].MarkedAsSaved = true;
            }
            else
            {
                WorldGenerator.WorldObjectDefinition newObj = new WorldGenerator.WorldObjectDefinition();
                Objects[i].OverwriteObjectDefinition(newObj, mapper);
                world.ObjectList.Add(newObj);
                world.Objects[newObj.ID] = newObj;
                newObj.MarkedAsSaved = true;
            }
        }
        world.DeleteUnsaved();
    }

    #region Tile Pool
    public WrappingTile TilePrefab;
    public static List<WrappingTile> tilePool = new List<WrappingTile>();

    static WrappingTile tmpTile;
    public static WrappingTile SpawnTile(int x, int y, TileType tileType)
    {
        tmpTile = null;
        for (int i = 0; i < tilePool.Count; i++)
        {
            if (!tilePool[i].isActiveAndEnabled)
            {
                tilePool[i].gameObject.SetActive(true);
                tmpTile = tilePool[i];
                break;
            }
        }

        if (tmpTile == null)
        {
            tmpTile = (WrappingTile)Instantiate(Current.TilePrefab);
            tilePool.Add(tmpTile);
        }

        tmpTile.ConfigureTile(tileType, Current.TileTypeSprites[(int)tileType]);
        Register(tmpTile, x, y);
        return tmpTile;
    }
    #endregion

    public static void ResetWorld()
    {
        mapper.Clear();

        for (int i = 0; i < Tiles.Count; i++)
            Tiles[i].gameObject.SetActive(false);
        Tiles.Clear();

        for (int i = 0; i < Objects.Count; i++)
            Objects[i].gameObject.SetActive(false);
        Objects.Clear();

        Current.tileOffset.X = 0;
        Current.tileOffset.Y = 0;

        Current.GridSize.X = 0;
        Current.GridSize.Y = 0;
        Current.lastPlayerTileX = null;
        Current.lastPlayerTileY = null;
    }

    public static List<WrappingTile> Tiles = new List<WrappingTile>();
    public static List<WrappingObject> Objects = new List<WrappingObject>();

    public static void Register(WrappingObject obj)
    {
        obj.GetOriginalTile(mapper);
        if (!Objects.Contains(obj))
            Objects.Add(obj);
    }

    public static void Deregister(WrappingObject obj)
    {
        Objects.Remove(obj);
    }

    static TileWrappingMapper mapper = new TileWrappingMapper();

    public static void Register(WrappingTile tile, int x, int y)
    {
        tile.OriginalTile.X = tile.Tile.X = x;
        tile.OriginalTile.Y = tile.Tile.Y = y;
        mapper.MapX(x, x);
        mapper.MapY(y, y);
        Tiles.Add(tile);

        if (x > Current.GridSize.X)
            Current.GridSize.X = x;
        if (y > Current.GridSize.Y)
            Current.GridSize.Y = y;
    }

    int? lastPlayerTileX;
    int? lastPlayerTileY;

    TileCoord tileOffset = new TileCoord();

    TileCoord direction = new TileCoord();
    TileCoord offset = new TileCoord();

    public void Update()
    {
        if (isLoading)
            return;

        offset.X = (int)Mathf.Floor(GridSize.X / 2);
        offset.Y = (int)Mathf.Ceil(GridSize.Y / 2);

        // Identify object tiles
        PlayerCharacter.Current.GetTile();
        PlayerCharacter.Current.SetSortOrder(tileOffset.Y * TileSizeY);
        for (int i = 0; i < Objects.Count; i++)
        {
            Objects[i].SetSortOrder(tileOffset.Y * TileSizeY);
            Objects[i].Wrap(PlayerCharacter.Current.Tile, tileOffset, mapper);
        }

        // See if tile has wrapped
        if ((!lastPlayerTileX.HasValue || lastPlayerTileX.Value != PlayerCharacter.Current.Tile.X) ||
            (!lastPlayerTileY.HasValue || lastPlayerTileY.Value != PlayerCharacter.Current.Tile.Y))
        {
            // Determine which direction changed
            direction.X = lastPlayerTileX.HasValue ? (PlayerCharacter.Current.Tile.X - lastPlayerTileX.Value) : 0;
            direction.Y = lastPlayerTileY.HasValue ? (PlayerCharacter.Current.Tile.Y - lastPlayerTileY.Value) : 0;

            for (int i = 0; i < Tiles.Count; i++)
            {
                // Wrap left to right
                if ((!lastPlayerTileX.HasValue || direction.X > 0) && Tiles[i].Tile.X < (PlayerCharacter.Current.Tile.X - offset.X))
                {
                    if (Tiles[i].Tile.X == tileOffset.X)
                        tileOffset.X++;
                    Tiles[i].Tile.X += (GridSize.X + 1);
                    mapper.MapX(Tiles[i].OriginalTile.X, Tiles[i].Tile.X);
                }

                // Wrap right to left
                if ((!lastPlayerTileX.HasValue || direction.X < 0) && Tiles[i].Tile.X > (PlayerCharacter.Current.Tile.X + offset.X))
                {
                    Tiles[i].Tile.X -= (GridSize.X + 1);
                    if (Tiles[i].Tile.X < tileOffset.X)
                        tileOffset.X = Tiles[i].Tile.X;
                    mapper.MapX(Tiles[i].OriginalTile.X, Tiles[i].Tile.X);
                }

                // Wrap bottom to top
                if ((!lastPlayerTileY.HasValue || direction.Y > 0) && Tiles[i].Tile.Y < (PlayerCharacter.Current.Tile.Y - offset.Y))
                {
                    if (Tiles[i].Tile.Y == tileOffset.Y)
                        tileOffset.Y++;
                    Tiles[i].Tile.Y += (GridSize.Y + 1);
                    mapper.MapY(Tiles[i].OriginalTile.Y, Tiles[i].Tile.Y);
                }

                // Wrap top to bottom
                if ((!lastPlayerTileY.HasValue || direction.Y < 0) && Tiles[i].Tile.Y > (PlayerCharacter.Current.Tile.Y + offset.Y))
                {
                    Tiles[i].Tile.Y -= (GridSize.Y + 1);
                    if (Tiles[i].Tile.Y < tileOffset.Y)
                        tileOffset.Y = Tiles[i].Tile.Y;
                    mapper.MapY(Tiles[i].OriginalTile.Y, Tiles[i].Tile.Y);
                }

                Tiles[i].PlaceTile();
            }

            lastPlayerTileX = PlayerCharacter.Current.Tile.X;
            lastPlayerTileY = PlayerCharacter.Current.Tile.Y;
        }
    }
}
