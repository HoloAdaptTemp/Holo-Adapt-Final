using UnityEngine;

public class MazeRawDelta : MonoBehaviour
{
    void Update()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;

        // Apply exact rotation delta
        transform.rotation *= delta;
    }
}