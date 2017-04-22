using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : WrappingObject {

    public FuelDamage DamageToPlayer;

    public int Health = 2;

    [System.Serializable]
    public class FuelDamage
    {
        public float DamageAmount;
        public float PushbackAmount;
        public float PushbackTimer;
        [System.NonSerialized]
        public Vector3 PushbackPosition;
    }

    protected virtual void Despawn()
    {
        gameObject.SetActive(false);
    }

    const string DEAL_FUEL_DAMAGE = "DealFuelDamage";
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerManager.Bullet)
        {
            Health--;
            if (Health <= 0)
                Despawn();
        }
        else if (collision.gameObject.layer == LayerManager.Player)
        {
            DamageToPlayer.PushbackPosition = transform.position;
            collision.gameObject.SendMessage(DEAL_FUEL_DAMAGE, this.DamageToPlayer);
        }
    }


}
