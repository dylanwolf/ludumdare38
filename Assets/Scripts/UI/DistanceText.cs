using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceText : MonoBehaviour {

    public float Threshold = 0.01f;

    Text text;
    float lastDistance = 0;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        if (GameState.CurrentState == GameState.GameMode.SpacePlaying)
        {
            if (Mathf.Abs(lastDistance - GameState.Distance) > Threshold)
            {
                text.text = string.Format("{0:0.00}", GameState.Distance);
                lastDistance = GameState.Distance;
            }
        }
    }

}
