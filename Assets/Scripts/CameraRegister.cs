using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRegister : MonoBehaviour {

    public bool IsWorld = false;

    private void Awake()
    {
        if (IsWorld)
        {
            GameState.WorldCameras.Add(gameObject);
            gameObject.SetActive(false);
        }
        else
            GameState.SpaceCameras.Add(gameObject);
    }
}
