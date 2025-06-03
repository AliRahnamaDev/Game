using System.Collections.Generic;
using UnityEngine;

public class InputRecorder : MonoBehaviour
{
    public struct InputFrame
    {
        public float time;
        public float horizontal;
        public bool jumpPressed;
        public bool firePressed;
    }

    public List<InputFrame> recordedInputs = new List<InputFrame>();
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        InputFrame frame = new InputFrame
        {
            time = Time.time - startTime,
            horizontal = Input.GetAxisRaw("Horizontal"),
            jumpPressed = Input.GetButtonDown("Jump"),
            firePressed = Input.GetButtonDown("Fire1")
        };

        recordedInputs.Add(frame);
    }
}

