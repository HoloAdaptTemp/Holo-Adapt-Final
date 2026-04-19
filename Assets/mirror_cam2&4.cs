using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    void Start()
    {
        // Flip camera horizontally
        transform.localScale = new Vector3(1, -1, 1);

        // Fix culling order inversion caused by negative scale
        Camera cam = GetComponent<Camera>();
        cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1));
    }
}