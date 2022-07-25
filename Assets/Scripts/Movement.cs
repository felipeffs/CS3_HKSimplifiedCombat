using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // Referencias
    GeneralControls input;
    Rigidbody2D rb;
    CollisionDetection cd;
    Animator animator;
    BoxCollider2D charCollider;


    [Header("Basic Movement")]
    [SerializeField, Min(1f)] float moveSpeed = 5;
    [SerializeField, Min(1f)] float airControl = 3;
    Vector2 direction;


    [Header("Wall Sliding")]
    [SerializeField, Min(0.5f)] float slipSpeed = 3f;
    [SerializeField] bool isWallSliding;
    Vector2 newVelocity;
    bool isNotComingOutOfWallSliding;
    bool isCollidingWithRightWall;
    bool isCollidingWithLeftWall;


    [Header("Jump")]
    [SerializeField] float jumpSpeed = 300f;
    [SerializeField, Range(0, 1)] float jumpChargeTime = .35f;
    [SerializeField, Min(1f)] float fallMultiplier = 5f;
    [SerializeField] bool isFullJump;
    Vector2 jumpVelocity;
    float fallVelocity;
    Coroutine currentCO_JumpTimer;


    [Header("Wall Jump")]
    [SerializeField] bool isWallJumping;
    [SerializeField] float wallJumpTimer = .1f;
    [SerializeField] float wallJumpForce = 200f;
    [SerializeField] Vector2 wallJumpDirection = new Vector2(-1f, 1.5f);
    Coroutine currentCO_WallJumpTimer;

    // Animation strings hash
    int isWalkingHash;
    int isWallSlidingHash;
    int isFallingHash;
    int isJumpingHash;
    int isWallJumpingHash;
    SpriteRenderer spriteRenderer;


    // Pra uso geral
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    Coroutine currentCO_DisableControls;


    // Variáveis para deixar legivel
    bool isGoingDown;

    //
    bool isFalling;
    bool isJumping;


    //
    PlayerAttack playerAttack;
    IAction attack;

    private void Awake()
    {
        input = new GeneralControls();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CollisionDetection>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isWallSlidingHash = Animator.StringToHash("isWallSliding");
        isFallingHash = Animator.StringToHash("isFalling");
        isJumpingHash = Animator.StringToHash("isJumping");
        isWallJumpingHash = Animator.StringToHash("isWallJumping");
        spriteRenderer = rb.gameObject.GetComponent<SpriteRenderer>();
        charCollider = rb.gameObject.GetComponent<BoxCollider2D>();

        //
        playerAttack = GetComponent<PlayerAttack>();
        attack = playerAttack.GetComponent<IAction>();
    }

    private void Start()
    {
        Setup();
    }

    private void OnValidate()
    {
        Setup();
    }

    private void Setup()
    {
        fallVelocity = Physics2D.gravity.y * (fallMultiplier - 1);
        jumpVelocity = Vector2.up * jumpSpeed;
    }

    private void FixedUpdate()
    {
        direction = input.CharacterControls.Movement.ReadValue<Vector2>();
        Fall();
        Walk();
        WallSlide();
        Flip();
        AnimationController();
    }

    // Auxilios
    private void ChangeVelocity(float xVelocity, float yVelocity)
    {
        newVelocity.x = xVelocity;
        newVelocity.y = yVelocity;
        rb.velocity = newVelocity;
    }

    private void SafeStartCourotine(ref Coroutine currentCourotine, IEnumerator courotine)
    {
        if (currentCourotine != null)
        {
            StopCoroutine(currentCourotine);
        }
        currentCourotine = StartCoroutine(courotine);
    }

    // Estados
    private void Walk()
    {
        if (isWallSliding || isWallJumping) return;

        if (cd.isGrounded)
        {
            ChangeVelocity(direction.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            ChangeVelocity(direction.x * airControl, rb.velocity.y);
        }
    }

    private void AnimationController()
    {
        //Wall Sliding Animation
        if (isWallSliding)
        {
            animator.SetBool(isWallSlidingHash, true);
            return;
        }
        else
        {
            animator.SetBool(isWallSlidingHash, false);
        }

        //Wall Jumping Animation
        if (isWallJumping)
        {
            animator.SetBool(isWallJumpingHash, true);
        }
        else
        {
            animator.SetBool(isWallJumpingHash, false);
        }

        //Walk Animation
        if (cd.isGrounded && direction.x != 0)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else
        {
            animator.SetBool(isWalkingHash, false);
        }

        //Jumping Animation
        if (isJumping)
        {
            animator.SetBool(isJumpingHash, true);
        }
        else
        {
            animator.SetBool(isJumpingHash, false);
        }

        //Falling Animation
        if (isFalling)
        {
            animator.SetBool(isFallingHash, true);
        }
        else
        {
            animator.SetBool(isFallingHash, false);
        }
    }

    private void Flip()
    {
        if (rb.velocity.x == 0) return;

        // if (isWallSliding)
        // {
        //     spriteRenderer.flipX = cd.isTouchingLeftWall ? false : true;
        //     return;
        // }

        if (rb.velocity.x < 0.2f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x > -0.2f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void WallSlide()
    {
        isCollidingWithLeftWall = cd.isTouchingLeftWall && direction.x == -1;
        isCollidingWithRightWall = cd.isTouchingRightWall && direction.x == 1;
        isNotComingOutOfWallSliding = isWallSliding && ((cd.isTouchingRightWall && direction.x != -1) || (cd.isTouchingLeftWall && direction.x != 1));

        isWallSliding = (!cd.isGrounded && (isCollidingWithRightWall || isCollidingWithLeftWall)) // Inicia o Wall sliding
        || (isGoingDown && isNotComingOutOfWallSliding); // Continua no Wall sliding

        if (isWallSliding)
        {
            ChangeVelocity(rb.velocity.x, -slipSpeed);
        }
    }

    private void Fall()
    {
        isGoingDown = rb.velocity.y < 0;
        isFalling = isGoingDown && !isWallJumping;

        if (cd.isGrounded) return;

        if (isFalling && !isFullJump)
        {
            ChangeVelocity(rb.velocity.x, rb.velocity.y + fallVelocity * Time.fixedDeltaTime);
            isJumping = false;
        }
        else if (isFalling)
        {
            isJumping = false;
        }
    }

    private void Jump()
    {
        if (cd.isGrounded)
        {
            SafeStartCourotine(ref currentCO_JumpTimer, CO_JumpTimer());
        }
        else if (isWallSliding)
        {
            SafeStartCourotine(ref currentCO_WallJumpTimer, CO_WallJumpTimer());
        }

    }

    // Coroutines
    IEnumerator CO_JumpTimer()
    {
        isJumping = true;
        isFullJump = false;
        float currentChargeTime = 0;
        while (jumpChargeTime > currentChargeTime && !isWallSliding)
        {
            rb.velocity = jumpVelocity * Time.fixedDeltaTime;
            yield return waitForFixedUpdate;
            currentChargeTime += Time.fixedDeltaTime;
        }
        isFullJump = true;
    }

    IEnumerator CO_WallJumpTimer()
    {
        isWallJumping = true;
        isFullJump = true;
        float timer = 0;
        rb.velocity = Vector2.zero;
        SafeStartCourotine(ref currentCO_DisableControls, CO_DisableControls(wallJumpTimer));
        if ((cd.isTouchingRightWall && wallJumpDirection.x > 0) || (cd.isTouchingLeftWall && wallJumpDirection.x < 0))
        {
            wallJumpDirection.x *= -1;
        }

        while (timer < wallJumpTimer)
        {
            rb.velocity = wallJumpDirection * wallJumpForce * Time.fixedDeltaTime;
            yield return null;
            timer += Time.deltaTime;
        }
        yield return new WaitForSeconds(0.1f);
        isWallJumping = false;
    }

    IEnumerator CO_DisableControls(float seconds)
    {
        input.CharacterControls.Disable();
        yield return new WaitForSeconds(seconds);
        input.CharacterControls.Enable();
    }

    // Metodos para Eventos do InputMap
    private void StartedJump(InputAction.CallbackContext context) => Jump();

    private void OnReleaseJumpButton(InputAction.CallbackContext context)
    {
        if (currentCO_JumpTimer != null)
            StopCoroutine(currentCO_JumpTimer);
        if (!isFullJump)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        attack.PerformAction(spriteRenderer.flipX);
    }

    private void OnEnable()
    {
        input.Enable();
        input.CharacterControls.Jump.started += StartedJump;
        input.CharacterControls.Jump.canceled += OnReleaseJumpButton;
        input.CharacterControls.Attack.started += OnAttack;
    }

    private void OnDisable()
    {
        input.Disable();
        input.CharacterControls.Jump.started -= StartedJump;
        input.CharacterControls.Jump.canceled -= OnReleaseJumpButton;
        input.CharacterControls.Attack.started -= OnAttack;
    }
}