using UnityEngine;

public class BounceAndSpin : MonoBehaviour
{
    public Vector2 velocity = new Vector2(2f, 1f);   // movement speed
    public float xLimit = 24f;                        // horizontal boundary
    public float yLimit = 13.5f;                      // vertical boundary
    public float rotationSpeed = 30f;                // spin speed (deg/sec)

    void Update()
    {
        // Move object
        transform.Translate(velocity * Time.deltaTime, Space.World);

        Vector3 pos = transform.position;

        // Bounce off left/right walls
        if (pos.x > xLimit || pos.x < -xLimit)
        {
            velocity.x = -velocity.x;
        }

        // Bounce off top/bottom walls
        if (pos.y > yLimit || pos.y < -yLimit)
        {
            velocity.y = -velocity.y;
        }

        // Rotate about its own center (local axis)
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}