using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MazeRawDelta : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;

        Quaternion newRotation = rb.rotation * delta;
        rb.MoveRotation(newRotation);
    }
}