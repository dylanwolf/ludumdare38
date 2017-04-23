using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlanet : MonoBehaviour {

    static List<SpacePlanet> pool = new List<SpacePlanet>();
    static SpacePlanet tmpPlanet;
    public static SpacePlanet Spawn(Vector3 position)
    {
        tmpPlanet = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isActiveAndEnabled)
            {
                tmpPlanet = pool[i];
                tmpPlanet.gameObject.SetActive(true);
                break;
            }
        }

        if (tmpPlanet == null)
        {
            tmpPlanet = Instantiate(PrefabManager.Current.PlanetPrefab);
            pool.Add(tmpPlanet);
        }

        tmpPlanet.transform.position = position;
        tmpPlanet.world = null;
        return tmpPlanet;
    }

    public static void Despawn(SpacePlanet planet)
    {
        planet.gameObject.SetActive(false);
    }

    const string PLAYER_SHIP = "Player Ship";

    WorldGenerator.WorldDefinition world;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == PLAYER_SHIP)
        {
            if (world == null)
            {
                world = WorldGenerator.Current.BuildWorld();
                GameState.SwitchToWorld(world);
            }
            else
            {
                GameState.SwitchToWorld(world);
            }
        }
    }
}
