using UnityEngine;

public class CameraNod : MonoBehaviour
{
    [Header("Nod Settings")]
    public float nodSpeed = 0.5f;     // how fast it nods
    public float maxDownAngle = -90f; // lowest point
    public float maxUpAngle = 0f;     // highest point

    private float amplitude;
    private float midpoint;
    private float timeCounter;

    private float initialYRotation;
    private float initialZRotation;

    void Start()
    {
        Vector3 startRot = transform.localEulerAngles;

        // Store original Y and Z to preserve direction
        initialYRotation = startRot.y;
        initialZRotation = startRot.z;

        // Calculate amplitude & midpoint for sine wave
        amplitude = (maxUpAngle - maxDownAngle) / 2f;
        midpoint = maxDownAngle + amplitude;

        timeCounter = 0f;
    }

    void Update()
    {
        timeCounter += nodSpeed * Time.deltaTime;

        // Smooth sine wave between min and max
        float xRotation = midpoint + amplitude * Mathf.Sin(timeCounter * Mathf.PI * 2);

        transform.localRotation = Quaternion.Euler(xRotation, initialYRotation, initialZRotation);
    }
}