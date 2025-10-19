using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityState
{
    public Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;

    protected float stateTimer;
    protected bool triggerCalled;

    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }
    public virtual void Update()
    {
        
        if (player.isLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("xVelocity", 0);
            return;
        }


        stateTimer -= Time.deltaTime;
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
            stateMachine.ChangeState(player.dashState);
 
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
    private bool CanDash()
    {
        if (player.wallDetected || stateMachine.currentState == player.dashState || !player.CanPerformDash())
            return false;
        return true;
    }
}
