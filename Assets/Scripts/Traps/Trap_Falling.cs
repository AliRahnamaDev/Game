using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Falling : MonoBehaviour
{
  private Animator animator;
  private Rigidbody2D rb;
  private BoxCollider2D[] boxcolliders;
  
  public Vector3[] waypoints;
  public float speed;
  public float travelDistance;
  private int waypointIndex;
  public bool canMove;

  [Header("Falling details")] 
  public float fallDely = 0.5f;

  private void Start()
  {
    SetupWaypoints();
    animator = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
    boxcolliders = GetComponents<BoxCollider2D>();
  }

  private void Update()
  {
    HandleMovement();
  }

  private void SetupWaypoints()
  {
    waypoints = new Vector3[2];
    float yOffset = travelDistance / 2;
    waypoints[0]=transform.position+new Vector3(0,yOffset,0);
    waypoints[1]=transform.position+new Vector3(0,-yOffset,0);
  }

  private void HandleMovement()
  {
    if (!canMove)
      return;
    transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex], speed * Time.deltaTime);
    if (Vector2.Distance(transform.position, waypoints[waypointIndex])<0.1f)
    {
      waypointIndex++;
      if (waypointIndex >= waypoints.Length)
        waypointIndex = 0;
    }
  }

  private void SwitchOffPlatform()
  {
    canMove = false;
    animator.SetTrigger("Deactive");
    rb.isKinematic = false;
    rb.gravityScale = 3.5f;
    rb.drag = 0.5f;

    foreach (BoxCollider2D box in boxcolliders)
    {
      box.enabled = false;
    }
  }
  
  // private void OnTriggerEnter2D(Collider2D other)
  // {
  //   Player player = other.gameObject.GetComponent<Player>();
  //   if(player != null)
  //   {
  //     Invoke("SwitchOffPlatform", fallDely);
  //   }
  // }
  
  private void OnTriggerEnter2D(Collider2D other)
  {
    Player player = other.gameObject.GetComponent<Player>();
    if (player != null)
    {
      StartCoroutine(FallDelayRoutine());
    }
  }

  private IEnumerator FallDelayRoutine()
  {
    Vector3 originalPosition = transform.position;
    Vector3 loweredPosition = originalPosition + new Vector3(0, -0.2f, 0);

    float t = 0f;
    float compressTime = 0.1f;

    // Move down slightly to simulate pressure
    while (t < compressTime)
    {
      transform.position = Vector3.Lerp(originalPosition, loweredPosition, t / compressTime);
      t += Time.deltaTime;
      yield return null;
    }

    transform.position = loweredPosition;
    yield return new WaitForSeconds(fallDely); // wait before falling

    SwitchOffPlatform();
  }
}

