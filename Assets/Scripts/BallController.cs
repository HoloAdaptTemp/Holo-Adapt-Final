using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    private Rigidbody mazeRb;
    private bool isResetting = false;
    private bool wasButton2Pressed = false;
    private Quaternion initialMazeLocalRotation;
    private bool hasInitialMazeRotation = false;

    [Header("Maze Reference")]
    public Transform mazeTransform;

    [Header("Reset Settings")]
    public float waitTime = 0.1f;
    public float fallThreshold = -1.0f;
    public float xRst = -8f;
    public float zRst = 5f;
    public float yRst = 2f; // Height above the maze floor

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float flexThreshold = 0.7f;
    private bool canJump = true;

    [Header("Win Text")]
    public GameObject winTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (winTextObject != null) winTextObject.SetActive(false);

        // Auto-assign if the maze is the parent of the ball
        if (mazeTransform == null && transform.parent != null)
        {
            mazeTransform = transform.parent;
        }

        if (mazeTransform != null)
        {
            mazeRb = mazeTransform.GetComponent<Rigidbody>();
            initialMazeLocalRotation = mazeTransform.localRotation;
            hasInitialMazeRotation = true;
        }
    }

    void Update()
    {
        HandleJump();
        HandleManualReset();
    }

    private void HandleJump()
    {
        if (websocket.Instance == null) return;

        bool isFlexing = websocket.Instance.GloveFlex1 > flexThreshold;

        if (isFlexing && canJump && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(JumpCooldown());
        }
    }

    private void HandleManualReset()
    {
        if (websocket.Instance == null)
        {
            wasButton2Pressed = false;
            return;
        }

        bool isButton2Pressed = websocket.Instance.GloveButton2;

        // Trigger reset only on button-down edge, not while held.
        if (isButton2Pressed && !wasButton2Pressed && !isResetting)
        {
            StartCoroutine(ResetBallRoutine());
        }

        wasButton2Pressed = isButton2Pressed;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }

    IEnumerator ResetBallRoutine()
    {
        isResetting = true;
        yield return new WaitForSeconds(waitTime);

        if (mazeTransform != null)
        {
            if (hasInitialMazeRotation)
            {
                if (mazeRb != null)
                {
                    mazeRb.linearVelocity = Vector3.zero;
                    mazeRb.angularVelocity = Vector3.zero;
                    mazeRb.rotation = mazeTransform.parent != null
                        ? mazeTransform.parent.rotation * initialMazeLocalRotation
                        : initialMazeLocalRotation;
                }
                else
                {
                    mazeTransform.localRotation = initialMazeLocalRotation;
                }

                Physics.SyncTransforms();
            }

            // Define the local spawn point
            Vector3 localRst = new Vector3(xRst, yRst, zRst);

            // Convert that local point to a World Position based on the Maze's current rotation/pos
            Vector3 worldRst = mazeTransform.TransformPoint(localRst);

            // Reset physical forces
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = worldRst;
        }

        isResetting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            other.gameObject.SetActive(false);
            if (winTextObject != null) winTextObject.SetActive(true);
        }
    }
}