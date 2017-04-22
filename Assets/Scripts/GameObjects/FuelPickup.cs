using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : WrappingObject {

    static List<FuelPickup> pool = new List<FuelPickup>();

    public override void OverwriteObjectDefinition(WorldGenerator.WorldObjectDefinition obj, TileWrappingMapper mapper)
    {
        base.OverwriteObjectDefinition(obj, mapper);
        obj.Type = ObjectType.FuelPickup;
    }

    static FuelPickup tmpFuel;
    public static FuelPickup Spawn(string id, Vector3 position)
    {
        tmpFuel = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isActiveAndEnabled)
            {
                tmpFuel = pool[i];
                tmpFuel.gameObject.SetActive(true);
                break;
            }
        }

        if (tmpFuel == null)
        {
            tmpFuel = Instantiate(PrefabManager.Current.FuelPickupPrefab);
            pool.Add(tmpFuel);
        }

        tmpFuel.transform.position = position;
        tmpFuel.LoadedObjectDefinition = id;
        WrappingWorld.Register(tmpFuel);
        return tmpFuel;
    }

    

    public float Amount = 5.0f;

    const string FUEL_MESSAGE = "GainedFuel";
    public void OnTriggerEnter2D(Collider2D collision)
    {
        collision.SendMessage(FUEL_MESSAGE, Amount, SendMessageOptions.DontRequireReceiver);
        gameObject.SetActive(false);
    }
}
