using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelGauge : MonoBehaviour {

    Image img;
    public float MaxFuel = 500.0f;

	void Awake () {
        img = GetComponent<Image>();
	}
	
	void Update () {

        img.fillAmount = Mathf.Clamp(GameState.Fuel / MaxFuel, 0, MaxFuel);
	}
}
