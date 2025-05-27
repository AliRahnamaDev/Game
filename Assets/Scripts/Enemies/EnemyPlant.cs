using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlant : Enemy
{
    
    void Update()
    {
        base.Update();
        
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
    }
}
