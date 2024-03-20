using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public const float DEFAULT_MOVESPEED = 5f;
    //Movement

    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;
    
    bool Dashing;
    bool isDashing;
    bool canDash;
    UnityEngine.Animation anim;
    //References
    Animator am;
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); //If we don't do this and game starts up and don't move, the projectile weapon will have no momentum
        anim = GetComponent<UnityEngine.Animation>();
        canDash = true;
        am = GetComponent<Animator>();


    }

    void Update()
    {
        InputManagement();
    }
    
    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }

        if(isDashing)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
            am.Play("Wizzard_dash");
        }

        float moveX, moveY;
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }
        

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    //Last moved X

        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);  //Last moved Y

        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);    //While moving

        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        if(isDashing)
        {
            return;
        }


        rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
    
    private IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        canDash = false;
        isDashing = true;


        rb.velocity = new Vector2(moveDir.x * player.Stats.dashSpeed, moveDir.y * player.Stats.dashSpeed);
        yield return new WaitForSeconds(player.Stats.dashDuration);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);



        yield return new WaitForSeconds(player.Stats.dashCooldown);
        canDash = true;
    }
}