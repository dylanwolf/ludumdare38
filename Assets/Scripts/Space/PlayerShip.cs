using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour {

    public float IdleFuelCost = 0.25f;
    public float FlyFuelCost = 1.0f;

    public float Angle = 90f;
    public float RotateSpeed = 30f;
    public float FlySpeed = 10f;
    public float DashSpeed = 20f;
    public float SlowSpeed = 5f;

    float lastAngle = 0f;
    const string HORIZONTAL_AXIS = "Horizontal";
    const string VERTICAL_AXIS = "Vertical";

    public static PlayerShip Current;
    public Transform Flame;

    Rigidbody2D _rb;

    private void Awake()
    {
        Current = this;
        _rb = GetComponent<Rigidbody2D>();
    }

    Camera[] spaceCams;

    private void Start()
    {
        spaceCams = GetComponentsInChildren<Camera>();
    }

    public void ResetGame()
    {
        Current.Angle = -90f;
        lastAngle = 0;
        transform.position = new Vector3(2.0f, 0, 0);
    }

    bool isIdle = false;
    float dashAmount = 0;
    float flySpeed = 0;
    Vector2 tmpV2;
	void Update () {
        if (GameState.CurrentState == GameState.GameMode.SpacePlaying)
        {
            isIdle = true;

            Angle += (RotateSpeed * -Input.GetAxis(HORIZONTAL_AXIS) * Time.deltaTime);
            Angle = Mathf.Repeat(Angle, 360);

            if (Angle != lastAngle)
            {
                isIdle = false;
                transform.localRotation = Quaternion.Euler(0, 0, Angle);
                for (int i = 0; i < spaceCams.Length; i++)
                    spaceCams[i].transform.localRotation = Quaternion.Euler(0, 0, -Angle);
            }

            dashAmount = Input.GetAxis(VERTICAL_AXIS);
            if (dashAmount == 0)
            {
                flySpeed = FlySpeed;
                Flame.transform.localScale = new Vector3(2, 2, 0);
                Soundboard.ResetThrusters();
            }
            else if (dashAmount < 0)
            {
                flySpeed = Mathf.Lerp(FlySpeed, SlowSpeed, -dashAmount);
                isIdle = true;
                Flame.transform.localScale = new Vector3(1, 1, 0);
                Soundboard.ResetThrusters();
            }
            else if (dashAmount > 0)
            {
                flySpeed = Mathf.Lerp(FlySpeed, DashSpeed, dashAmount);
                isIdle = true;
                Flame.transform.localScale = new Vector3(3, 3, 0);
                Soundboard.PlayThrusters(Time.deltaTime, 0.1f);
            }

            GameState.UpdateDistance(transform.position.magnitude);

            GameState.SpendFuel((isIdle ? IdleFuelCost : FlyFuelCost) * Time.fixedDeltaTime);
        }
	}

    void FixedUpdate()
    {
        if (GameState.CurrentState == GameState.GameMode.SpacePlaying && GameState.Fuel > 0)
        {
            tmpV2.x = -Mathf.Sin(Mathf.Deg2Rad * Angle);
            tmpV2.y = Mathf.Cos(Mathf.Deg2Rad * Angle);
            _rb.velocity = (tmpV2 * flySpeed);
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }
}
