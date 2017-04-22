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

    Rigidbody2D _rb;

    private void Awake()
    {
        Current = this;
        _rb = GetComponent<Rigidbody2D>();
    }

    Camera spaceCam;

    private void Start()
    {
        spaceCam = GetComponentInChildren<Camera>();
    }

    public void ResetGame()
    {
        Current.Angle = 90f;
        lastAngle = 0;
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
            if (Angle != lastAngle)
            {
                isIdle = false;
                transform.localRotation = Quaternion.Euler(0, 0, Angle);
                spaceCam.transform.localRotation = Quaternion.Euler(0, 0, -Angle);
            }

            dashAmount = Input.GetAxis(VERTICAL_AXIS);
            if (dashAmount == 0)
                flySpeed = FlySpeed;
            else if (dashAmount < 0)
            {
                Debug.Log("Slow");
                flySpeed = Mathf.Lerp(FlySpeed, SlowSpeed, -dashAmount);
                isIdle = true;
            }
            else if (dashAmount > 0)
            {
                Debug.Log("Dash");
                flySpeed = Mathf.Lerp(FlySpeed, DashSpeed, dashAmount);
                isIdle = true;
            }

            GameState.SpendFuel((isIdle ? IdleFuelCost : FlyFuelCost) * Time.fixedDeltaTime);
        }
	}

    void FixedUpdate()
    {
        if (GameState.CurrentState == GameState.GameMode.SpacePlaying)
        {
            tmpV2.x = -Mathf.Sin(Mathf.Deg2Rad * Angle);
            tmpV2.y = Mathf.Cos(Mathf.Deg2Rad * Angle) ;
            _rb.velocity = (tmpV2 * flySpeed);
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }
}
