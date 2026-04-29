using UnityEngine;

public class rotateEngine : MonoBehaviour
{
    void Update()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;
        float flex = websocket.Instance.GloveFlex1;

        if (flex > 0.5)
        {
            transform.rotation = transform.rotation * delta;
        }
    }
}