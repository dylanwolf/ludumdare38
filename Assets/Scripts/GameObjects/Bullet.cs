using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : WrappingObject {

    public float Timer;

    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public static List<Bullet> pool = new List<Bullet>();
    static Bullet tmpBullet;
    public static Bullet Spawn(Vector3 position, Vector2 direction, float timer, float speed)
    {
        tmpBullet = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isActiveAndEnabled)
            {
                tmpBullet = pool[i];
                tmpBullet.gameObject.SetActive(true);
                break;
            }
        }

        if (tmpBullet == null)
        {
            tmpBullet = Instantiate(PrefabManager.Current.BulletPrefab);
            pool.Add(tmpBullet);
        }

        tmpBullet.transform.position = position;
        tmpBullet.Timer = timer;
        WrappingWorld.Register(tmpBullet);
        tmpBullet._rb.velocity = direction * speed;
        tmpBullet.hit = false;
        return tmpBullet;
    }

    public static void Despawn(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        WrappingWorld.Deregister(bullet);
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
            Despawn(this);
    }

    const string BULLET_MESSAGE = "WasShot";
    bool hit = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Despawn(this);
        if (!hit)
            collision.SendMessage(BULLET_MESSAGE, null, SendMessageOptions.DontRequireReceiver);
        hit = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Despawn(this);
        if (!hit)
            collision.gameObject.SendMessage(BULLET_MESSAGE, null, SendMessageOptions.DontRequireReceiver);
        hit = true;
    }
}
