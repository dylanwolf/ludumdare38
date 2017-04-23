using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGenerator : MonoBehaviour {

    public static SpaceGenerator Current;
	
	void Awake () {
        Current = this;
	}

    private void Start()
    {
        ResetGame();
    }

    public int MinPlanets;
    public int MaxPlanets;
    public float Distance = 50.0f;

    static List<SpacePlanet> planets = new List<SpacePlanet>();

    void ResetGame()
    {
        for (int i = 0; i < planets.Count; i++)
            SpacePlanet.Despawn(planets[i]);
        planets.Clear();

        int planetCount = Random.Range(MinPlanets, MaxPlanets);
        for (int i = 0; i < planetCount; i++)
        {
            planets.Add(GeneratePlanet());
        }
    }

    Vector3 tmpV3;
    SpacePlanet GeneratePlanet()
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        float distance = Random.Range(5f, Distance);

        tmpV3.x = -Mathf.Sin(angle);
        tmpV3.y = Mathf.Cos(angle);

        return SpacePlanet.Spawn(tmpV3.normalized * distance);
    }
}
