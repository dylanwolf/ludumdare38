using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour {

    public static int Player;
    public static int PlayerJumping;
    public static int Water;
    public static int Bullet;
    public static int Default = 0;
	
    void Awake()
    {
        Player = LayerMask.NameToLayer("Player");
        PlayerJumping = LayerMask.NameToLayer("PlayerJumping");
        Water = LayerMask.NameToLayer("Water");
        Bullet = LayerMask.NameToLayer("Bullet");
    }
	
}
