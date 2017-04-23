using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameState  {

    public static float Fuel = 25.0f;
    public static float Distance = 0.0f;

    public static void UpdateDistance(float distance)
    {
        Distance = distance;
    }

    const string AUDIO_FUEL = "Fuel";
    public static void SpendFuel(float amount)
    {
        Fuel -= amount;
        if (Fuel < 0)
        {
            Fuel = 0;
            if (CurrentState == GameMode.SpacePlaying)
                TriggerGameOver();
        }
    }

    public static void GainFuel(float amount)
    {
        Fuel += amount;
        Soundboard.Play(AUDIO_FUEL);
    }

    public static void TriggerGameOver()
    {
        CurrentState = GameMode.GameOver;
        GameOver.HideOrShow(true);
    }

    public enum GameMode
    {
        PlanetPlaying,
        SpacePlaying,
        GameOver,
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
            WorldCameras[i].SetActive(true);
        }

        for (int i = 0; i < SpaceCameras.Count; i++)
        {
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
            WorldCameras[i].SetActive(false);
        }

        for (int i = 0; i < SpaceCameras.Count; i++)
        {
            SpaceCameras[i].SetActive(true);
        }

        WrappingWorld.Current.SaveWorldState();
        CurrentState = GameMode.SpacePlaying;
    }

    public static GameMode CurrentState = GameMode.SpacePlaying;
}
