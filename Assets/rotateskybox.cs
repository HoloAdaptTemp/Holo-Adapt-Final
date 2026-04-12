using UnityEngine;

public class SkyRotationAdvanced : MonoBehaviour
{
    [Header("Observer settings")]
    public float latitude = 40.25f;      // Your location latitude in degrees

    [Header("Sky rotation")]
    public float rotationSpeed = 5f;     // Degrees per second, increase for faster demo

    [Header("Polaris object")]
    public Transform polaris;            // Optional: visual marker for Polaris

    private Vector3 celestialAxis;       // Axis for rotation

    void Start()
    {
        // Calculate Earth's rotation axis based on latitude
        // This ensures Polaris stays fixed
        float latRad = latitude * Mathf.Deg2Rad;

        // Axis vector points from origin toward Polaris
        celestialAxis = new Vector3(
            Mathf.Cos(latRad), // x component
            Mathf.Sin(latRad), // y component
            0f                 // z component
        ).normalized;

        if (polaris != null)
        {
            // Align Polaris along the axis at a far distance
            polaris.position = celestialAxis * 1000f;
        }
    }

    void Update()
    {
        // Rotate the sky sphere around the celestial axis
        transform.Rotate(celestialAxis, rotationSpeed * Time.deltaTime, Space.World);
    }
}