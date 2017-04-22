using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelText : MonoBehaviour {

    float lastFuel = 0;
    Text _t;

    private void Awake()
    {
        _t = GetComponent<Text>();
    }


    // Update is called once per frame
    void Update () {
		if (lastFuel != GameState.Fuel)
        {
            _t.text = ((int)Mathf.Ceil(GameState.Fuel)).ToString();
            lastFuel = GameState.Fuel;
        }
	}
}
