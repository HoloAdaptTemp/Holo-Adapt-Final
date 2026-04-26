using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RadialMenuController : MonoBehaviour
{
    public Transform pointer;
    public int optionCount = 6;

    public GameObject radialCanvas;
    public RectTransform labelParent;
    public GameObject labelPrefab;

    public bool mainSceneSwitcher = true; // SHOULD ONLY BE TRUE FOR ONE CAMERA

    public string[] optionLabels = new string[6]
    {
        "Cancel", "Cameras", "Radial Menu", "Scene 3", "Scene 4", "Scene 5"
    };

    private string[] sceneNames = new string[6]
    {
        "",
        "holoadapt_opening",
        "ES@P",
        "SampleScene",
        "RealStarsSkyboxFreeAsset",
        "engine"
    };

    public float radius = 150f;

    public bool radialMenuIsActive = false;

    private int currentIndex = -1;
    private int selectedIndex = -1;

    private float roll = 0f;

    private TMP_Text[] labels;

    void Start()
    {
        if (radialCanvas != null)
            radialCanvas.SetActive(false);

        GenerateLabels();
    }

    void Update()
    {
        if (websocket.Instance == null) return;

        bool button = websocket.Instance.GloveButton1;
        float rollDelta = websocket.Instance.GloveRotationDeltaEuler.z;

        if (button && !radialMenuIsActive)
        {
            OpenMenu();
            roll = 0f;
        }

        if (button && radialMenuIsActive)
        {
            roll += rollDelta;
            roll = (roll % 360f + 360f) % 360f;

            UpdateSelection(roll);
        }

        if (!button && radialMenuIsActive)
        {
            ConfirmSelection();
            CloseMenu();
        }
    }

    void GenerateLabels()
    {
        float sliceSize = 360f / optionCount;

        labels = new TMP_Text[optionCount];

        ClearLabels();

        for (int i = 0; i < optionCount; i++)
        {
            GameObject label = Instantiate(labelPrefab, labelParent);
            RectTransform rect = label.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            float angle = -i * sliceSize + 60f;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad),
                Mathf.Sin(rad)
            ) * radius;

            rect.anchoredPosition = pos;

            TMP_Text text = label.GetComponentInChildren<TMP_Text>(true);

            if (text != null)
            {
                Debug.Log("Setting label to:", text);
                text.SetText(optionLabels[i]);
                labels[i] = text;
            }
        }
    }

    void ClearLabels()
    {
        for (int i = labelParent.childCount - 1; i >= 0; i--)
        {
            Transform child = labelParent.GetChild(i);

            if (child.name.Contains("Pointer"))
                continue;

            Destroy(child.gameObject);
        }
    }

    void OpenMenu()
    {
        radialMenuIsActive = true;

        if (radialCanvas != null)
            radialCanvas.SetActive(true);
    }

    void CloseMenu()
    {
        radialMenuIsActive = false;

        if (radialCanvas != null)
            radialCanvas.SetActive(false);

        ClearHighlight();
    }

    void UpdateSelection(float roll)
    {
        float sliceSize = 360f / optionCount;

        float correctedRoll = (roll + 360f) % 360f;

        int index = Mathf.FloorToInt(correctedRoll / sliceSize);
        index = Mathf.Clamp(index, 0, optionCount - 1);

        currentIndex = index;

        UpdateHighlight(index);

        pointer.localRotation = Quaternion.Euler(0, 0, -correctedRoll);
    }

    void UpdateHighlight(int index)
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null) continue;

            labels[i].color = (i == index) ? Color.yellow : Color.white;
        }
    }

    void ClearHighlight()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null) continue;

            labels[i].color = Color.white;
        }
    }

    void ConfirmSelection()
    {
        selectedIndex = currentIndex;

        Debug.Log("Selected option: " + optionLabels[selectedIndex]);

        ExecuteOption(selectedIndex);
    }

    void ExecuteOption(int index)
    {
        if (index == 0)
        {
            Debug.Log("Cancelled");
            return;
        }

        if (mainSceneSwitcher == true)
        {
            Debug.Log("Loading " + sceneNames[index]);
            SceneManager.LoadScene(sceneNames[index]);
        }
    }
}