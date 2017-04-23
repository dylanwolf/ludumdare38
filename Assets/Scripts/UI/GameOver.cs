using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    static List<Graphic> Current = new List<Graphic>();

    void Awake () {
        Current.AddRange(GetComponents<Graphic>());
        HideOrShow(false);
	}
	
	public static void HideOrShow(bool visible)
    {
        for (int i = 0; i < Current.Count; i++)
            Current[i].enabled = visible;
    }
}
