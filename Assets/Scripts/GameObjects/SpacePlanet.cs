using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlanet : MonoBehaviour {

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
