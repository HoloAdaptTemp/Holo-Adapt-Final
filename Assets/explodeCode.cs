using UnityEngine;

public class ExplodedViewScroll : MonoBehaviour
{
    [Header("Explosion Control")]
    [Range(0f, 1f)]
    public float explodeAmount = 0f;

    public float explodeSpeed = 0.05f;   // slower, smoother scroll
    public float scaleFactor = 0.2f;

    public Transform originPiece; // assign Oil_pan-1 here

    private Transform[] parts;
    private Vector3[] initialLocalPositions;

    void Start()
    {
        if (originPiece == null)
        {
            Debug.LogError("Assign Oil_pan-1 as originPiece in Inspector!");
            return;
        }

        parts = GetComponentsInChildren<Transform>();
        initialLocalPositions = new Vector3[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            Transform t = parts[i];
            initialLocalPositions[i] = t.localPosition;
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        explodeAmount += scroll * explodeSpeed;
        explodeAmount = Mathf.Clamp01(explodeAmount);

        ApplyExplosion();
    }

    void ApplyExplosion()
    {
        Vector3 origin = originPiece.localPosition;

        for (int i = 0; i < parts.Length; i++)
        {
            Transform t = parts[i];

            if (t == transform) continue;         // skip root
            if (t == originPiece) continue;       // KEEP OIL PAN FIXED

            Vector3 dir = (initialLocalPositions[i] - origin);
            float dist = dir.magnitude;

            if (dist < 0.0001f) continue;

            dir.Normalize();

            Vector3 offset = dir * dist * scaleFactor * explodeAmount;

            t.localPosition = initialLocalPositions[i] + offset;
        }
    }
}