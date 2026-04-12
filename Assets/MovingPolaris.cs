using UnityEngine;

public class PolarisRandomCircle : MonoBehaviour
{
    [Header("Circle Settings")]
    public float radius = 1000f;
    public Vector3 center = Vector3.zero; // center of the circle

    [Header("Movement Settings")]
    public float moveSpeed = 100f;
    public float waitTime = 2f; // time before picking a new point

    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        // Move toward the target position
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if close to target
        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                PickNewTarget();
                timer = 0f;
            }
        }
    }

    void PickNewTarget()
    {
        // Random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Calculate position on horizontal circle (XZ plane)
        float x = center.x + Mathf.Cos(angle) * radius;
        float z = center.z + Mathf.Sin(angle) * radius;

        // Keep same height (Y)
        float y = transform.position.y;

        targetPosition = new Vector3(x, y, z);
    }
}