using System.Collections;
using System.Text;
using UnityEngine;
using NativeWebSocket;

public class websocket : MonoBehaviour
{
    // Singleton instance so other scripts can reference websocket.Instance
    public static websocket Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private WebSocket _websocketClient;
    private float prevPitchDeg;
    private float prevRollDeg;
    private float prevYawDeg;
    private float unwrappedPitchDeg;
    private float unwrappedRollDeg;
    private float unwrappedYawDeg;
    private bool hasPreviousRotation = false;

    // Bounds coming from the Python dummy generators:
    // - rotations: [-180, 180]
    // - accel:  [-(2^31), 2^31-1]
    // - flex:   [0.0, 1.0]
    private const float ACCEL_MIN = -100f;
    private const float ACCEL_MAX = 100f;

    private static float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        if (inMax - inMin == 0f) return outMin;
        return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
    }

    public Quaternion GloveRotation { get; private set; }
    public Vector3 GloveRotationEuler { get; private set; }
    public Quaternion GloveRotationDelta { get; private set; }
    public Vector3 GloveRotationDeltaEuler { get; private set; }
    public Vector3 GloveAcceleration { get; private set; }
    public float GloveFlex1 { get; private set; } // in range [0, 1]
    public float GloveFlex2 { get; private set; } // in range [0, 1]
    public bool GloveButton1 { get; private set; } // true if pressed, false otherwise
    public bool GloveButton2 { get; private set; } // true if pressed, false otherwise

    void Start()
    {
        ConnectWebSocket();
    }

    async void ConnectWebSocket()
    {
        _websocketClient = new WebSocket("ws://localhost:8765");

        _websocketClient.OnOpen += () =>
        {
            Debug.Log("WebSocket connection opened.");
        };

        _websocketClient.OnError += (e) =>
        {
            Debug.LogError("WebSocket error: " + e);
        };

        _websocketClient.OnClose += (e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };

        _websocketClient.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            // Parse and apply rotation
            ParseMessage(message);
        };

        await _websocketClient.Connect();
    }

    async void OnDestroy()
    {
        if (_websocketClient != null && _websocketClient.State == WebSocketState.Open)
        {
            await _websocketClient.Close();
        }
        if (Instance == this) Instance = null;
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocketClient?.DispatchMessageQueue();
#endif
    }

    void ParseMessage(string message)
    {
        string[] parts = message.Split(',');
        // CSV: pitch,roll,yaw,accel_x,accel_y,accel_z,flex_1,flex_2,button_1,button_2
        if (parts.Length >= 10)
        {
            float.TryParse(parts[0], out float pitchRaw);
            float.TryParse(parts[1], out float rollRaw);
            float.TryParse(parts[2], out float yawRaw);
            float.TryParse(parts[3], out float accelXRaw);
            float.TryParse(parts[4], out float accelYRaw);
            float.TryParse(parts[5], out float accelZRaw);
            float.TryParse(parts[6], out float flex1Raw);
            float.TryParse(parts[7], out float flex2Raw);
            float.TryParse(parts[8], out float button1Raw);
            float.TryParse(parts[9], out float button2Raw);

            // Treat the IMU angles as wrapped degrees and unwrap them locally so the
            // motion stays continuous across the +/-180 boundary.
            float pitchDeg = pitchRaw;
            float rollDeg = rollRaw;
            float yawDeg = yawRaw;

            if (!hasPreviousRotation)
            {
                prevPitchDeg = pitchDeg;
                prevRollDeg = rollDeg;
                prevYawDeg = yawDeg;
                unwrappedPitchDeg = pitchDeg;
                unwrappedRollDeg = rollDeg;
                unwrappedYawDeg = yawDeg;
                hasPreviousRotation = true;
            }

            float pitchDelta = Mathf.DeltaAngle(prevPitchDeg, pitchDeg);
            float rollDelta = Mathf.DeltaAngle(prevRollDeg, rollDeg);
            float yawDelta = Mathf.DeltaAngle(prevYawDeg, yawDeg);

            unwrappedPitchDeg += pitchDelta;
            unwrappedRollDeg += rollDelta;
            unwrappedYawDeg += yawDelta;

            GloveRotation = Quaternion.Euler(
                unwrappedPitchDeg,
                unwrappedYawDeg,
                unwrappedRollDeg
            );
            GloveRotationEuler = new Vector3(unwrappedPitchDeg, unwrappedYawDeg, unwrappedRollDeg);

            GloveRotationDelta = Quaternion.Euler(
                pitchDelta,
                yawDelta,
                rollDelta
            );
            GloveRotationDeltaEuler = new Vector3(
                pitchDelta,
                yawDelta,
                rollDelta
            );

            // Map rotation values (Python sends accel in [-100,100]) to [-100,100]
            float accelX = Map(accelXRaw, ACCEL_MIN, ACCEL_MAX, -100f, 100f);
            float accelY = Map(accelYRaw, ACCEL_MIN, ACCEL_MAX, -100f, 100f);
            float accelZ = Map(accelZRaw, ACCEL_MIN, ACCEL_MAX, -100f, 100f);

            GloveAcceleration = new Vector3(
                accelX,
                accelY,
                accelZ
            );

            // Flex already in [0,1] from the generator
            GloveFlex1 = Mathf.Clamp01(flex1Raw);
            GloveFlex2 = Mathf.Clamp01(flex2Raw);

            GloveButton1 = button1Raw > 0.5f;
            GloveButton2 = button2Raw > 0.5f;

            // Save current values for the next wrap-safe delta calculation
            prevPitchDeg = pitchDeg;
            prevRollDeg = rollDeg;
            prevYawDeg = yawDeg;

            Debug.Log(
                $"Roll: {GloveRotationEuler.z:F2}, " +
                $"Pitch: {GloveRotationEuler.x:F2}, " +
                $"Yaw: {GloveRotationEuler.y:F2} | " +
                $"Button1: {GloveButton1}, Button2: {GloveButton2}"
            );

        }
    }
}
