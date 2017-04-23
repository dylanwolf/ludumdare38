using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supernova : MonoBehaviour {

    public Transform Sun;
    public Transform SupernovaSprite;

    Vector3 tmpV3;

    float Size = 0.1f;

    public float SunPulse = 3.0f;
    public float MinPulse = 0.5f;
    public float MaxPulse = 2.5f;
    float pulseTimer = 0;

    public float SunSpinSpeed = 10.0f;
    float spinAngle = 0;

    public float Speed = 0.5f;

    public static Supernova Current;

    private void Awake()
    {
        Current = this;
    }

    public void ResetGame()
    {
        Size = 0.1f;
    }

    private void Update()
    {
        if (GameState.CurrentState == GameState.GameMode.PlanetPlaying || GameState.CurrentState == GameState.GameMode.SpacePlaying)
        {
            Size += Speed * Time.deltaTime * (1 + (Size / 25f));
        }

        pulseTimer += Time.deltaTime;
        pulseTimer = Mathf.Repeat(pulseTimer, 1.0f);

        tmpV3 = Sun.transform.localScale;
        tmpV3.x = tmpV3.y = Mathf.Lerp(MinPulse, MaxPulse, (Mathf.Sin(Mathf.PI * 2 * pulseTimer) + 1) / 2.0f);
        Sun.transform.localScale = tmpV3;

        spinAngle += SunSpinSpeed * Time.deltaTime;
        spinAngle = Mathf.Repeat(spinAngle, 360);
        Sun.transform.localRotation = Quaternion.Euler(0, 0, spinAngle);

        tmpV3 = SupernovaSprite.transform.localScale;
        tmpV3.x = tmpV3.y = Size;
        SupernovaSprite.transform.localScale = tmpV3;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerShip.Current.gameObject)
        {
            GameState.TriggerGameOver();
        }
    }
}
