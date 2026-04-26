using UnityEngine;

public class SkyRotationAdvanced : MonoBehaviour
{
    [Header("Observer settings")]
    public float latitude = 40.25f;

    [Header("Polaris object")]
    public Transform polaris;

    private Vector3 celestialAxis;

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
            // Get axis-angle from delta
            delta.ToAngleAxis(out float angle, out Vector3 axis);

            // Ensure axis is meaningful
            if (angle > 180f) angle -= 360f;

            // Determine roll sign relative to glove forward direction
            Vector3 gloveForward = delta * Vector3.forward;

            // Project onto plane perpendicular to sky axis
            Vector3 projected = Vector3.ProjectOnPlane(gloveForward, celestialAxis).normalized;

            // Reference direction to define sign
            Vector3 refDir = Vector3.Cross(celestialAxis, Vector3.up);
            if (refDir.sqrMagnitude < 0.01f)
                refDir = Vector3.Cross(celestialAxis, Vector3.right);

            float sign = Mathf.Sign(Vector3.Dot(projected, refDir));

            float signedAngle = angle * sign;

            transform.rotation = Quaternion.AngleAxis(signedAngle, celestialAxis) * transform.rotation;
        }
    }
}