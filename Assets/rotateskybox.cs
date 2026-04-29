using UnityEngine;

public class SkyRotationAdvanced : MonoBehaviour
{
    [Header("Observer settings")]
    public float latitude = 40.25f;

    [Header("Polaris object")]
    public Transform polaris;

    private Vector3 celestialAxis;

    // Extract signed twist angle (in degrees) around a local axis from a quaternion.
    private static float ExtractSignedTwistAngle(Quaternion rotation, Vector3 localAxis)
    {
        Vector3 axis = localAxis.normalized;

        // Keep only quaternion vector part aligned to the target axis.
        Vector3 qVec = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 projected = Vector3.Project(qVec, axis);

        Quaternion twist = new Quaternion(projected.x, projected.y, projected.z, rotation.w).normalized;
        twist.ToAngleAxis(out float angle, out Vector3 twistAxis);

        if (angle > 180f) angle -= 360f;

        float sign = Mathf.Sign(Vector3.Dot(twistAxis, axis));
        return angle * sign;
    }

    void Start()
    {
        float latRad = latitude * Mathf.Deg2Rad;

        celestialAxis = new Vector3(
            Mathf.Cos(latRad),
            Mathf.Sin(latRad),
            0f
        ).normalized;

        if (polaris != null)
        {
            polaris.position = celestialAxis * 1000f;
        }
    }

    void Update()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;
        float flex = websocket.Instance.GloveFlex1;

        if (flex < 0.5f)
        {
            // Only use glove roll (twist about local forward), preserving direction.
            float signedRoll = ExtractSignedTwistAngle(delta, Vector3.forward);
            transform.rotation = Quaternion.AngleAxis(signedRoll, celestialAxis) * transform.rotation;
        }
    }
}