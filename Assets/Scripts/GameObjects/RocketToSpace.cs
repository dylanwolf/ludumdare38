using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketToSpace : WrappingObject {

    public float MaxDistance = 1f;
    public static RocketToSpace Current;

    public override void Start()
    {
        Current = this;
        base.Start();
    }

    public void ResetWorld()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        Tile.X = 0;
        Tile.Y = 0;
    }

}
