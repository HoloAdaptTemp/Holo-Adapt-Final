using UnityEngine;

public class StarsGroupYaw : MonoBehaviour
{
    void Update()
    {
        if (websocket.Instance == null) return;

        Quaternion delta = websocket.Instance.GloveRotationDelta;
        float flex = websocket.Instance.GloveFlex1;

        if (flex > 0.5)
        {
            // Get forward vector from delta
            Vector3 forward = delta * Vector3.forward;

            // Project onto horizontal plane (remove pitch)
            forward.y = 0f;

            if (forward.sqrMagnitude > 0.0001f)
            {
                Quaternion yawOnly = Quaternion.LookRotation(forward);
                transform.rotation *= yawOnly;
            }
        }
    }
}