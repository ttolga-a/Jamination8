using UnityEngine;

public class Player_AirState : EntityState
{
    public Player_AirState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);
        else
            player.SetVelocity(0, rb.linearVelocity.y);

        if (player.wallDetected) stateMachine.ChangeState(player.wallSlideState);
    }
}
