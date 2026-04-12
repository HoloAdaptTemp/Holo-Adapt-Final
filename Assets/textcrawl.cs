using UnityEngine;

public class StarWarsCrawl : MonoBehaviour
{
    public float scrollSpeed = 30f;   // Speed of the crawl
    public float tiltAngle = 20f;     // Tilt for perspective

    void Start()
    {
        // Tilt the text backward for that classic effect
        transform.rotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

    void Update()
    {
        // Move text upward over time
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }
}