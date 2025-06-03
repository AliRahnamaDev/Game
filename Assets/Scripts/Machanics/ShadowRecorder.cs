using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct ShadowState
{
    public Vector2 position;
    public bool isJumping;
    public bool isDashing;
    public bool isWallSliding;
    public bool isRunning;
    public bool isFacingRight;
    public float time;
}

public class ShadowRecorder : MonoBehaviour
{
    public List<ShadowState> recordedStates = new List<ShadowState>();
    public bool isRecording = false;
    private float timer = 0f;

    public Player player;

    void Update()
    {
        if (isRecording)
        {
            timer += Time.deltaTime;
            recordedStates.Add(new ShadowState()
            {
                position = transform.position,
                isJumping = player._rb.velocity.y != 0 && !player.isGrounded,
                isDashing = player.isDashing,
                isWallSliding = player.isWallDitected && !player.isGrounded,
                isRunning = player._rb.velocity.x != 0 && player.isGrounded,
                isFacingRight = player.isFacingRight,
                time = timer
            });
        }
    }

    public void StartRecording()
    {
        recordedStates.Clear();
        timer = 0f;
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }
}
