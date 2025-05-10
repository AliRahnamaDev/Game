using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    public float movespeed = 3; 
    public Transform[] waypoint;
    public int waypointindex = 1;
    public bool canMove = true;
    public float coolTime = 0.5f;
    public int moveDirection = 1;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        transform.position = waypoint[0].position;
    }

    
    void Update()
    {
        animator.SetBool("isActive", canMove);
        if (!canMove)
            return;
        transform.position = Vector2.MoveTowards(transform.position, waypoint[waypointindex].position,movespeed *Time.deltaTime);
        
        if(Vector2.Distance(transform.position,waypoint[waypointindex].position)<0.1f)
        {
            if (waypointindex == waypoint.Length - 1 || waypointindex == 0)
            {
                moveDirection *= -1;
            }
            
            waypointindex=waypointindex+moveDirection;
        }
        
    }

    private IEnumerator StopMovement(float dely)
    {
        canMove = false;
        yield return new WaitForSeconds(dely);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
}