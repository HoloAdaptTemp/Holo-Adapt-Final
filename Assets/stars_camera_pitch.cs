using UnityEngine;

public class StarsCameraPitch : MonoBehaviour
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
            forward.x = 0f;

            if (forward.sqrMagnitude > 0.0001f)
            {
                Quaternion pitchOnly = Quaternion.LookRotation(forward);
                transform.rotation *= pitchOnly;
            }
        }
    }
}