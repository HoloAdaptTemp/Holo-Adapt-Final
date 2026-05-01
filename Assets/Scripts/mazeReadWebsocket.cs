using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MazeRawDelta : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Rotation Damping")]
    public float maxRotationSpeedDegreesPerFrame = 1f; // Max rotation angle per frame in degrees

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;

        // Extract rotation angle and axis from the delta quaternion
        delta.ToAngleAxis(out float angle, out Vector3 axis);

        // Clamp the rotation angle to the maximum allowed per frame
        angle = Mathf.Clamp(angle, -maxRotationSpeedDegreesPerFrame, maxRotationSpeedDegreesPerFrame);

        // Reconstruct the clamped quaternion
        Quaternion dampedDelta = Quaternion.AngleAxis(angle, axis);

        Quaternion newRotation = rb.rotation * dampedDelta;
        rb.MoveRotation(newRotation);
    }
}