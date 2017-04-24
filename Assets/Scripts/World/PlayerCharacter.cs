using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    [System.NonSerialized]
    public TileCoord Tile = new TileCoord();

    public float Speed = 10.0f;
    public float JumpFuelCost = 1f;

    public float BulletOffset = 0.35f;
    public float BulletCost = 1f;
    public float BulletTimer = 0.5f;
    public float BulletLifetime = 10f;
    public float BulletSpeed = 20f;

    public Animator Flame;
    SpriteRenderer _flameSr;

    float bulletTimerCurrent = 0;

    [System.NonSerialized]
    public TileCoord Facing = new TileCoord();

    public static PlayerCharacter Current;

    Rigidbody2D _rb;
    SpriteRenderer _r;
    CapsuleCollider2D _c;
    Animator anim;

    const string ANIM_DIRECTION = "Direction";
    const string ANIM_ISWALKING = "IsWalking";

    private void Awake()
    {
        Current = this;
        isJumpingFilter = new ContactFilter2D();
        _rb = GetComponent<Rigidbody2D>();
        _c = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        _r = GetComponent<SpriteRenderer>();

        Cursor.visible = false;

        ResetWorld();
    }

    public void Start()
    {
        _flameSr = Flame.GetComponent<SpriteRenderer>();
    }

    public void SetSortOrder(float startY)
    {
        if (_r != null)
            _r.sortingOrder = (int)((transform.position.y - startY) * -100) + 5000;
        if (_flameSr != null)
            _flameSr.sortingOrder = (int)((transform.position.y - startY) * -100) + 4999;
    }

    public void GetTile()
    {
        Tile.X = (int)Mathf.Floor(transform.position.x / WrappingWorld.Current.TileSizeX);
        Tile.Y = (int)Mathf.Floor(transform.position.y / WrappingWorld.Current.TileSizeY);
    }

    Vector3 tmpV3;
    public void ResetWorld()
    {
        isJumping = false;
        Facing.X = 0;
        Facing.Y = -1;
        pushback = Vector3.zero;
        pushbackTimer = 0;

        tmpV3 = transform.position;
        tmpV3.x = 0;
        tmpV3.y = 0;
        transform.position = tmpV3;

        movement = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        if (GameState.CurrentState == GameState.GameMode.PlanetPlaying)
        {
            HandleLeaving();
            HandleShooting();
            HandleJumping();
            HandleMovement();
        }
        else
        {
            movement = Vector2.zero;
        }

        _flameSr.enabled = isJumping;
    }

    private void FixedUpdate()
    {
        if (GameState.CurrentState == GameState.GameMode.PlanetPlaying)
        {
            ApplyMovement();   
        }
        else
        {
            _rb.velocity = Vector2.zero;
            anim.SetBool(ANIM_ISWALKING, false);
        }
    }

    public void GainedFuel(int amount)
    {
        GameState.GainFuel(amount);
    }

    const string JUMP_BUTTON = "Jump";
    const string FIRE_BUTTON = "Fire1";
    const string HORIZONTAL_AXIS = "Horizontal";
    const string VERTICAL_AXIS = "Vertical";
    private void HandleLeaving()
    {
        if (Input.GetButtonDown(JUMP_BUTTON) && Vector3.Distance(RocketToSpace.Current.transform.position, transform.position) <= RocketToSpace.Current.MaxDistance)
            GameState.SwitchToSpace();

    }

    const string AUDIO_SHOOT = "Shoot";

    private void HandleShooting()
    {
        if (bulletTimerCurrent <= 0)
        {
            if (Input.GetButtonDown(FIRE_BUTTON))
            {
                bulletTimerCurrent = BulletTimer;
                
                Bullet.Spawn(transform.position + new Vector3(_c.offset.x + (Facing.X * BulletOffset), _c.offset.y + (Facing.Y * BulletOffset) + 0.2f, 0), Facing.ToVector2(), BulletLifetime, BulletSpeed);
                GameState.SpendFuel(BulletCost);
                Soundboard.Play(AUDIO_SHOOT);
                
            }
        }
        else
        {
            bulletTimerCurrent -= Time.deltaTime;
        }
    }

    bool isJumping = false;
    bool isForcedJumping = false;
    ContactFilter2D isJumpingFilter;
    int isJumpingHitsCount = 0;
    Collider2D[] isJumpingHits = new Collider2D[10];
    private void HandleJumping()
    { 
        if (isJumping)
        {
            // If we're already jumping, see if we need to continue
            isJumpingHitsCount = _c.OverlapCollider(isJumpingFilter, isJumpingHits);
            isForcedJumping = false;
            for (int i = 0; i < isJumpingHitsCount; i++)
            {
                if (isJumpingHits[0].gameObject.layer == LayerManager.Water)
                {
                    isForcedJumping = true;
                    break;
                }
            }
            
            if (!isForcedJumping)
                isJumping = Input.GetButton(JUMP_BUTTON);
        }
        else
        {
            isJumping = Input.GetButton(JUMP_BUTTON);
        }

        if (isJumping)
            GameState.SpendFuel(JumpFuelCost * Time.deltaTime);

        gameObject.layer = isJumping ? LayerManager.PlayerJumping : LayerManager.Player;

        if (isJumping)
            Soundboard.PlayThrusters(Time.deltaTime, 0.25f);
        else
            Soundboard.ResetThrusters();
    }

    Vector2 movement;
    Vector2 tmpV2;
    private void HandleMovement()
    {
        movement.x = (Input.GetAxis(HORIZONTAL_AXIS) * Speed) + pushback.x;
        movement.y = (Input.GetAxis(VERTICAL_AXIS) * Speed) + pushback.y;

        if (pushbackTimer > 0)
        {
            pushbackTimer -= Time.fixedDeltaTime;
            if (pushbackTimer <= 0)
                pushback = Vector3.zero;
        }

        if (movement.magnitude > 0)
        {
            Facing.X = Mathf.Abs(movement.x) >= Mathf.Abs(movement.y) ? (int)Mathf.Sign(movement.x) : 0;
            Facing.Y = Mathf.Abs(movement.y) > Mathf.Abs(movement.x) ? (int)Mathf.Sign(movement.y) : 0;

            if (Facing.X == 0 && Facing.Y < 0)
            {
                anim.SetInteger(ANIM_DIRECTION, 0);
            }
            else if (Facing.X == 0 && Facing.Y > 0)
            {
                anim.SetInteger(ANIM_DIRECTION, 1);
            }
            else if (Facing.Y == 0 && Facing.X < 0)
            {
                anim.SetInteger(ANIM_DIRECTION, 2);
            }
            else if (Facing.Y == 0 && Facing.X > 0)
            {
                anim.SetInteger(ANIM_DIRECTION, 3);
            }
            anim.SetBool(ANIM_ISWALKING, !isJumping);
        }
        else
        {
            anim.SetBool(ANIM_ISWALKING, false);
        }
    }

    void ApplyMovement()
    {
        _rb.velocity = movement;
    }

    Vector3 pushback;
    float pushbackTimer;
    public void DealFuelDamage(MonsterBase.FuelDamage fd)
    {
        GameState.SpendFuel(fd.DamageAmount);
        pushback = (transform.position - fd.PushbackPosition).normalized * fd.PushbackAmount;
        Debug.Log(string.Format("Applying pushback of X {0}, Y {1}", pushback.x, pushback.y));
        pushbackTimer = fd.PushbackTimer;
    }

    bool allowExitToSpace = false;
    public void AllowExitToSpace(bool allow)
    {
        Debug.Log("Allow rocket to space: " + allow.ToString());
        allowExitToSpace = allow;
    }
}
