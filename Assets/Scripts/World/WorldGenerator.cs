using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public class WorldDefinition
    {
        public string ID;
        public TileType[,] Tiles;
        public Dictionary<string, WorldObjectDefinition> Objects;
        public List<WorldObjectDefinition> ObjectList;

        public WorldDefinition(int width, int height)
        {
            ID = System.Guid.NewGuid().ToString();
            Tiles = new TileType[width,height];
            Objects = new Dictionary<string, WorldObjectDefinition>();
            ObjectList = new List<WorldObjectDefinition>();
        }

        public void ResetSaveMarkers()
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                ObjectList[i].MarkedAsSaved = false;
            }
        }

        public void DeleteUnsaved()
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (!ObjectList[i].MarkedAsSaved)
                {
                    Objects.Remove(ObjectList[i].ID);
                    ObjectList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public class WorldObjectDefinition
    {
        public string ID;
        public ObjectType Type;
        public Vector3 Position;

        public int RemainingIntValue;

        public bool MarkedAsSaved = false;

        public WorldObjectDefinition()
        {
            ID = System.Guid.NewGuid().ToString();
        }
    }

    static Dictionary<string, WorldDefinition> cache = new Dictionary<string, WorldDefinition>();

    public static void ResetGame()
    {
        cache.Clear();
    }

    public int MinSize;
    public int MaxSize;
    public int MinFuelTreeAmount = 5;
    public int MaxFuelTreeAmount = 10;

    public WorldDefinition BuildWorld()
    {
        int size = Random.Range(MinSize, MaxSize);
        WorldDefinition world = new WorldDefinition(size, size);

        BuildTilesRandom(world);

        cache[world.ID] = world;

        return world;
    }

    void BuildTilesRandom(WorldDefinition world)
    {
        int width = world.Tiles.GetLength(0);
        int height = world.Tiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                world.Tiles[x, y] = ((x == 0 && y == 0) || Random.value > 0.3f) ? TileType.Land : TileType.Water;
            }
        }

        BuildObjects(world);
    }

    void BuildObjects(WorldDefinition world)
    {
        int width = world.Tiles.GetLength(0);
        int height = world.Tiles.GetLength(1);
        float r;
        WorldObjectDefinition obj;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                r = Random.value;
                if (r >= 0.98f && world.Tiles[x,y] == TileType.Land)
                {
                    obj = new WorldObjectDefinition();
                    obj.Type = ObjectType.FuelTree;
                    obj.RemainingIntValue = Random.Range(MinFuelTreeAmount, MaxFuelTreeAmount);
                    obj.Position = new Vector3(x * WrappingWorld.Current.TileSizeX, y * WrappingWorld.Current.TileSizeY, 0);
                    world.Objects[obj.ID] = obj;
                    world.ObjectList.Add(obj);
                }
                else if (r >= 0.91f)
                {
                    obj = new WorldObjectDefinition();
                    obj.Type = ObjectType.FuelPickup;
                    obj.Position = new Vector3(x * WrappingWorld.Current.TileSizeX, y * WrappingWorld.Current.TileSizeY, 0);
                    world.Objects[obj.ID] = obj;
                    world.ObjectList.Add(obj);
                }
            }
        }
    }

    public static WorldGenerator.WorldDefinition GetWorld(string id)
    {
        return cache[id];
    }

    public static WorldGenerator Current;

	void Awake () {
        Current = this;
	}
}
