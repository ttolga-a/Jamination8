using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 🧩 Component References
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public PlayerInputSet input { get; private set; }
    private StateMachine stateMachine;

    // 🧍 Player States
    public Player_IdleState idleState { get; private set; }
    public Player_RunState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }

    [Header("Dash UI")]
    [SerializeField] private Image dashCooldownMask;
    // ⚙️ Movement Settings
    [Header("Movement Details")]
    public float moveSpeed = 6f;
    public float jumpForce = 5f;
    public float inAirMoveMultiplier = 0.7f;
    public float wallSlideMultiplier = 0.3f;
    public float dashDuration = 0.25f;
    public float dashSpeed = 20f;
    public float dashCooldown = 2f;
    private bool canDash = true;
    private float dashCooldownTimer = 0;
    private bool facingRight = true;
    public int facingDirection { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }
    public Vector2 wallJumpForce;

    // 🔒 Lock State
    public bool isLocked { get; private set; } = false;
    private bool inputsEnabled = true;

    // 🧱 Collision Detection
    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    [SerializeField] private Transform groundCheck;

    private bool canCheckWall = true;
    private float wallCheckDisableTimer;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    // 🧠 Unity Built-ins
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        // 🎮 Initialize States
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_RunState(this, stateMachine, "run");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
    }

    private void OnEnable()
    {
        input.Enable();

        // 🎯 Movement input
        input.Player.Movement.performed += ctx =>
        {
            if (inputsEnabled)
                moveInput = ctx.ReadValue<Vector2>();
            else
                moveInput = Vector2.zero;
        };

        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {

    if (!canDash)
    {
        dashCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer <= 0)
        {
            canDash = true;
            dashCooldownTimer = 0;
        }

        // 🎨 UI Güncellemesi
        if (dashCooldownMask != null)
            dashCooldownMask.fillAmount = dashCooldownTimer / dashCooldown;
    }
    else
    {
        // Eğer cooldown bittiyse dolgu yok
        if (dashCooldownMask != null)
            dashCooldownMask.fillAmount = 0f;
    }

    stateMachine.UpdateActiveState();

    // Diğer kodların (duvar, zemin, lock kontrolü vs) aynen kalacak ↓
    if (!canCheckWall)
    {
        wallCheckDisableTimer -= Time.deltaTime;
        if (wallCheckDisableTimer <= 0)
            canCheckWall = true;
    }

    if (canCheckWall)
    {
        wallDetected =
            Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
         && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    if (isLocked)
    {
        moveInput = Vector2.zero;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        return;
    }
    }

    private void FixedUpdate()
    {
        HandleCollisionDetection();
    }

    // 🔁 Animasyon tetikleme
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    // 🚶 Hareket
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    // 🔄 Flip yönü
    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight)
            Flip();
        else if (xVelocity < 0 && facingRight)
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection *= -1;
    }

    // ⚡ Yer kontrolü
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    // 🧱 Debug için
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        if (primaryWallCheck != null)
            Gizmos.DrawLine(primaryWallCheck.position,
                primaryWallCheck.position + Vector3.right * facingDirection * wallCheckDistance);

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position,
                secondaryWallCheck.position + Vector3.right * facingDirection * wallCheckDistance);
    }

    // ⚙️ Yardımcı Fonksiyonlar
    public void DisableWallCheckFor(float duration)
    {
        canCheckWall = false;
        wallCheckDisableTimer = duration;
    }

    // 🔒 Lock sistemi
    public void LockPlayer()
    {
        if (isLocked) return;
        isLocked = true;
        inputsEnabled = false;

        // Anında hareketi kes
        rb.linearVelocity = Vector2.zero;

        // State'i zorla idle’a çek (animasyon + velocity sıfırlansın)
        if (stateMachine.currentState != idleState)
            stateMachine.ChangeState(idleState);
    }

    public void UnlockPlayer()
    {
        if (!isLocked) return;

        isLocked = false;
        inputsEnabled = true;
    }

    public bool CanPerformDash()
    {
        return canDash;
    }

    public void StartDashCooldown()
    {
        canDash = false;
        dashCooldownTimer = dashCooldown;

        if (dashCooldownMask != null)
            dashCooldownMask.fillAmount = 1f;
    }
}
