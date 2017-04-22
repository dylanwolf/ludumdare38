using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTree : WrappingObject {

    static List<FuelTree> pool = new List<FuelTree>();
    static FuelTree tmpTree;

    public static FuelTree Spawn(string id, Vector3 position, int fuels)
    {
        tmpTree = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isActiveAndEnabled)
            {
                tmpTree = pool[i];
                tmpTree.gameObject.SetActive(true);
                break;
            }
        }

        if (tmpTree == null)
        {
            tmpTree = Instantiate(PrefabManager.Current.FuelTreePrefab);
            pool.Add(tmpTree);
        }

        tmpTree.transform.position = position;
        tmpTree.LoadedObjectDefinition = id;
        tmpTree.Fuels = fuels;
        WrappingWorld.Register(tmpTree);
        return tmpTree;
    }

    public override void OverwriteObjectDefinition(WorldGenerator.WorldObjectDefinition obj, TileWrappingMapper mapper)
    {
        base.OverwriteObjectDefinition(obj, mapper);
        obj.Type = ObjectType.FuelTree;
        obj.RemainingIntValue = Fuels;
    }

    public float MinScatterX = 0.1f;
    public float MaxScatterX = 0.25f;
    public float ScatterY = 0.05f;

    [System.NonSerialized]
    public int Fuels = 10;

    public void WasShot()
    {
        if (Fuels > 0)
        {
            tmpV3 = transform.position;
            tmpV3.x += Random.Range(MinScatterX, MaxScatterX) * (Random.value > 0.5f ? -1 : 1);
            tmpV3.y += Random.Range(-ScatterY, ScatterY);
            FuelPickup.Spawn(System.Guid.NewGuid().ToString(), tmpV3);
            Fuels--;

            if (Fuels == 0)
                gameObject.SetActive(false);
        }
    }
}
