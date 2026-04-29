using UnityEngine;

public class ExplodedViewScroll : MonoBehaviour
{
    [Header("Explosion Control")]
    [Range(0f, 1f)]
    public float explodeAmount = 0f;

    public float scaleFactor = 0.2f;

    public Transform originPiece; // Oil_pan-1

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
            initialLocalPositions[i] = parts[i].localPosition;
        }
    }

    void Update()
    {
        if (websocket.Instance == null) return;

        // Use glove flex directly (0 = closed, 1 = open)
        explodeAmount = Mathf.Clamp01(websocket.Instance.GloveFlex2);

        ApplyExplosion();
    }

    void ApplyExplosion()
    {
        Vector3 origin = originPiece.localPosition;

        for (int i = 0; i < parts.Length; i++)
        {
            Transform t = parts[i];

            if (t == transform) continue;
            if (t == originPiece) continue;

            Vector3 dir = initialLocalPositions[i] - origin;
            float dist = dir.magnitude;

            if (dist < 0.0001f) continue;

            dir.Normalize();

            Vector3 offset = dir * dist * scaleFactor * explodeAmount;

            t.localPosition = initialLocalPositions[i] + offset;
        }
    }
}