using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float climbAnimationSpeed = 1f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawn;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    
    float startingGravityScale = 8f;
    bool isAlive = true;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        startingGravityScale = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        
        Run();
        FlipSprite();
        ClimbLadder();
        GameOver();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive)
        {
            return;
        }

        moveInput = value.Get<Vector2>();
        //Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if(!isAlive)
        {
            return;
        }

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2 (0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }

        Instantiate(bullet, bulletSpawn.position, transform.rotation);
    }

    void ClimbLadder()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;


            if (playerHasVerticalSpeed)
            {
                //Debug.Log("Climbing");
                myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
                myAnimator.SetFloat("climbingSpeed", climbAnimationSpeed);
            }

            if (!playerHasVerticalSpeed)
            {
                //Debug.Log("Standing on Ladder");
                myAnimator.SetFloat("climbingSpeed", 0f);
            }
        }
        else
        {
            //Debug.Log("NotClimbing");
            myAnimator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = startingGravityScale;
        }
    }

    void Run()
    { 
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool isMovingHorizontal = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", isMovingHorizontal);
    }
    void FlipSprite()
    {
        bool isMovingHorizontal = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (isMovingHorizontal)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void GameOver()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            var cineCam = FindObjectOfType<CameraScript>();
            cineCam.KillCam();
            isAlive = false;
            myAnimator.SetTrigger("isDead");
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        else
        {
            return;
        }
    }
}
