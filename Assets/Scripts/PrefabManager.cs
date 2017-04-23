using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

    public static PrefabManager Current;

    public Bullet BulletPrefab;
    public FuelPickup FuelPickupPrefab;
    public FuelTree FuelTreePrefab;
    public SpacePlanet PlanetPrefab;

    void Awake () {
        Current = this;
	}
}
