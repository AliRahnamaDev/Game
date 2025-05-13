using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    protected override void Awake()
    {
      base.Awake();
    }

    protected override void Update()
    {
        base.Update() ;
        HandleMovement() ;
        HandleCollisions() ;
        if (!isGroundInfrontDetected || isWallDetected)
        {
            if(isGrounded ==false)
                return;
            idleTimer = idleDuration ;
            rb.velocity = Vector2.zero ;
            Flip() ;
        }
    }
    private void HandleMovement()
    {
        if(idleTimer > 0)
            return;
        rb.velocity = new Vector2(speed*facingDirection, rb.velocity.y);
    }
    
}
