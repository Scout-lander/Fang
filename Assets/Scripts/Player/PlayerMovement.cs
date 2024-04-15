using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Terresquall;

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
    private bool movementLocked = false;
    
    // Dashing
    bool Dashing;
    bool isDashing;
    bool canDash;
    private float dashSpeed;
    private float SBuff;

    // Stun
    public bool isStunned;
    public bool canBeStunned = true;
    public ParticleSystem stunParticles; // Reference to the particle system


    //References
    UnityEngine.Animation anim;
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
        dashSpeed = player.Stats.moveSpeed * 20;
        SBuff = player.Stats.moveSpeed * 20;
        
    }

    void Update()
    {
        InputManagement();
    }
    
    void FixedUpdate()
    {
        if (!isStunned && !isDashing)
            Move();
    }

    void InputManagement()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }

        if (isStunned || isDashing)
            return;


        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
            am.Play("Wizzard_dash");
        }
        

        float moveX, moveY;
        if (VirtualJoystick.CountActiveInstances() > 0)
        {
            moveX = VirtualJoystick.GetAxisRaw("Horizontal");
            moveY = VirtualJoystick.GetAxisRaw("Vertical");
        }
        else
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

        if(isDashing || isStunned)
        {
            return;
        }


        rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
    
    public void DoDash()
    {
        StartCoroutine(Dash());
    }
    public IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        canDash = false;
        isDashing = true;


        rb.velocity = new Vector2(moveDir.x * dashSpeed, moveDir.y * dashSpeed);
        am.Play("Wizzard_dash");
        yield return new WaitForSeconds(player.Stats.dashDuration);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);



        yield return new WaitForSeconds(player.Stats.dashCooldown);
        canDash = true;
    }

    public void ActiveSpeedBuff()
    {
        rb.velocity = new Vector2(moveDir.x * player.Stats.moveSpeed * SBuff, moveDir.y * player.Stats.moveSpeed * SBuff);
    }

    public void DeactiveSpeedBuff()
    {
        rb.velocity = new Vector2(moveDir.x * player.Stats.moveSpeed / SBuff, moveDir.y * player.Stats.moveSpeed / SBuff);
    }

    public void LockMovement(bool lockMovement)
    {
        movementLocked = lockMovement;
        if (lockMovement)
        {
            // Freeze movement along all axes (X, Y, and rotation around the Z-axis)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            // Unfreeze movement along all axes
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void Knockback(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public IEnumerator ApplyKnockback(Vector2 direction, float force, float duration)
    {
        Knockback(direction, force); // Apply knockback
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero; // Reset velocity after duration
    }

    public IEnumerator ApplyStun(Vector2 sourcePosition, float knockbackForce, float knockbackDuration)
    {
        if (!isStunned && canBeStunned)
        { Debug.Log("Stunning player");

            isStunned = true;
            canBeStunned = false;
            am.SetBool("Stunned", true);

            // Activate stun particles
            Knockback(((Vector2)transform.position - sourcePosition).normalized * knockbackForce, 0.3f);
            yield return new WaitForSeconds(0.25f);

            //if (stunParticles) Destroy(Instantiate(stunParticles, transform.position, Quaternion.identity), 1f);
            if (stunParticles != null)
            {
                GameObject instantiatedParticles = Instantiate(stunParticles.gameObject, transform.position, Quaternion.identity);
                Destroy(instantiatedParticles, 2f);
            }

            LockMovement(true);
            yield return new WaitForSeconds(2); //How Long the stun lasts

            am.SetBool("Stunned", false);
            isStunned = false;
            LockMovement(false);

            yield return new WaitForSeconds(4); //How long until you can be Stunned again
            canBeStunned = true;
        }
    }
}