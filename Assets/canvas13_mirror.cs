using UnityEngine;

public class MirrorCanvas : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }
}