using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameState  {

    public static float Fuel = 100.0f;

    public static void SpendFuel(float amount)
    {
        Fuel -= amount;
        if (Fuel < 0)
            Fuel = 0;
    }

    public static void GainFuel(float amount)
    {
        Fuel += amount;
    }

    public enum GameMode
    {
        PlanetPlaying,
        SpacePlaying,
    }

    public static List<GameObject> WorldCameras = new List<GameObject>();
    public static List<GameObject> SpaceCameras = new List<GameObject>();

    public static void SwitchToWorld(string id)
    {
        SwitchToWorld(WorldGenerator.GetWorld(id));
    }

    public static void SwitchToWorld(WorldGenerator.WorldDefinition world)
    {
        PlayerCharacter.Current.gameObject.SetActive(true);

        for (int i = 0; i < WorldCameras.Count; i++)
        {
            Debug.Log("Active: " + WorldCameras[i].name);
            WorldCameras[i].SetActive(true);
        }

        for (int i = 0; i < SpaceCameras.Count; i++)
        {
            Debug.Log("Inactive: " + SpaceCameras[i].name);
            SpaceCameras[i].SetActive(false);
        }

        CurrentState = GameMode.PlanetPlaying;
        WrappingWorld.ResetWorld();
        PlayerCharacter.Current.ResetWorld();
        WrappingWorld.Current.LoadWorld(world);
    }

    public static void SwitchToSpace()
    {
        for (int i = 0; i < WorldCameras.Count; i++)
        {
            Debug.Log("Inactive: " + WorldCameras[i].name);
            WorldCameras[i].SetActive(false);
        }

        for (int i = 0; i < SpaceCameras.Count; i++)
        {
            Debug.Log("Active: " + SpaceCameras[i].name);
            SpaceCameras[i].SetActive(true);
        }

        WrappingWorld.Current.SaveWorldState();
        CurrentState = GameMode.SpacePlaying;
    }

    public static GameMode CurrentState = GameMode.SpacePlaying;
}
