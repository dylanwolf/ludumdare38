using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileCoord {

    public int X;
    public int Y;

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }
}
