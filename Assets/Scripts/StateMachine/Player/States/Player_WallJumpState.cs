using UnityEngine;

public class Player_WallJumpState : EntityState
{
    private float controlLockTime = 0.5f; // zıpladıktan sonra kısa süre kontrol kilidi
    private float controlTimer;

    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Kilit süresi başlat
        controlTimer = controlLockTime;

        // Wall Jump kuvvetini uygula
        player.SetVelocity(
            player.wallJumpForce.x * -player.facingDirection,
            player.wallJumpForce.y
        );

        // Duvar algısını geçici olarak kapat
        player.DisableWallCheckFor(0.2f);
    }

    public override void Update()
    {
        base.Update();

        controlTimer -= Time.deltaTime;

        // Kontrol kilidi bitince yatay hareketi yavaşça geri kazandır
        if (controlTimer <= 0)
        {
            float inputX = player.moveInput.x;
            if (Mathf.Abs(inputX) > 0.05f)
            {
                // Havada yön verme hızı
                float targetVelX = inputX * player.moveSpeed * player.inAirMoveMultiplier;
                player.SetVelocity(
                    targetVelX,
                    player.rb.linearVelocity.y
                );
            }
        }

        // Düşüşe geçerse Fall state'e
        if (player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }

        // Eğer tekrar duvar algılanırsa (ve kontrol süresi geçtiyse)
        else if (controlTimer <= 0 && player.wallDetected)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
