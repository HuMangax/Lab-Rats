using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Title screen. Built entirely in code and styled to match the LevelSelect lab
// console (monitor grid, sweeping scanline, teal/amber palette, pulsing status).
// Reuses the Pulse / Scanline helper components declared in LevelSelect.cs.
public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "LevelSelect";

    [Header("UI References (auto-created if null)")]
    public Canvas canvas;
    public Button playButton;
    public Button quitButton;

    // ---- Lab palette (shared with LevelSelect) ----
    static readonly Color BgDark     = new Color(0.055f, 0.078f, 0.070f);
    static readonly Color GridLine   = new Color(0.35f, 0.85f, 0.68f, 0.05f);
    static readonly Color Accent     = new Color(0.27f, 0.85f, 0.64f);
    static readonly Color AccentDeep = new Color(0.10f, 0.30f, 0.25f);
    static readonly Color Hazard     = new Color(0.90f, 0.68f, 0.24f);
    static readonly Color TextBright = new Color(0.85f, 0.94f, 0.91f);
    static readonly Color TextMuted  = new Color(0.42f, 0.52f, 0.48f);
    static readonly Color FrameTeal  = new Color(0.16f, 0.32f, 0.28f);
    static readonly Color NeutralBtn = new Color(0.16f, 0.17f, 0.18f);

    void Awake()
    {
        if (canvas == null)
            CreateMenuUI();
    }

    void CreateMenuUI()
    {
        EnsureEventSystem();

        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Background + faint monitor grid.
        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = BgDark;
        StretchToFill(bg.GetComponent<RectTransform>());
        BuildGrid(bg.transform);

        // Sweeping scanline.
        GameObject scan = CreateUIElement("Scanline", canvasObj.transform);
        Image scanImg = scan.AddComponent<Image>();
        scanImg.color = new Color(Accent.r, Accent.g, Accent.b, 0.06f);
        scanImg.raycastTarget = false;
        RectTransform scanRect = scan.GetComponent<RectTransform>();
        scanRect.anchorMin = new Vector2(0f, 1f);
        scanRect.anchorMax = new Vector2(1f, 1f);
        scanRect.pivot = new Vector2(0.5f, 1f);
        scanRect.sizeDelta = new Vector2(0, 120);
        scan.AddComponent<Scanline>().Init(scanRect);

        // ---- Title ----
        GameObject title = CreateText("Title", canvasObj.transform, "LAB RATS",
            96, FontStyle.Bold, TextBright, TextAnchor.MiddleCenter);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 250);
        titleRect.sizeDelta = new Vector2(1200, 150);

        GameObject subtitle = CreateText("Subtitle", canvasObj.transform,
            "// FACILITY 7   —   SUBJECT ESCAPE PROTOCOL", 28, FontStyle.Normal,
            Accent, TextAnchor.MiddleCenter);
        RectTransform subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = new Vector2(0, 166);
        subRect.sizeDelta = new Vector2(1400, 40);
        AddHazardRule(canvasObj.transform, new Vector2(0, 120), 760f);

        // ---- Buttons ----
        playButton = CreateButton("PLAY", canvasObj.transform, new Vector2(0, 12),
            new Vector2(340, 76), AccentDeep, TextBright, 34);
        playButton.onClick.AddListener(PlayGame);

        Button levelSelectButton = CreateButton("LEVEL SELECT", canvasObj.transform,
            new Vector2(0, -84), new Vector2(340, 76), FrameTeal, TextBright, 30);
        levelSelectButton.onClick.AddListener(() => SceneManager.LoadScene("LevelSelect"));

        quitButton = CreateButton("QUIT", canvasObj.transform, new Vector2(0, -180),
            new Vector2(340, 76), NeutralBtn, TextBright, 30);
        quitButton.onClick.AddListener(QuitGame);

        // ---- Footer status ----
        AddHazardRule(canvasObj.transform, new Vector2(0, -262), 760f);

        GameObject status = CreateText("Status", canvasObj.transform, "SYSTEM ONLINE",
            22, FontStyle.Bold, Accent, TextAnchor.MiddleCenter);
        RectTransform statusRect = status.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusRect.anchoredPosition = new Vector2(0, -312);
        statusRect.sizeDelta = new Vector2(600, 30);
        status.AddComponent<Pulse>().Init(status.GetComponent<Text>(),
            Accent, new Color(0.14f, 0.34f, 0.30f), 2.6f);

        GameObject footer = CreateText("Footer", canvasObj.transform,
            "RESEARCH DIVISION   //   v1.0", 20, FontStyle.Normal, TextMuted,
            TextAnchor.MiddleCenter);
        RectTransform footRect = footer.GetComponent<RectTransform>();
        footRect.anchorMin = new Vector2(0.5f, 0);
        footRect.anchorMax = new Vector2(0.5f, 0);
        footRect.anchoredPosition = new Vector2(0, 46);
        footRect.sizeDelta = new Vector2(1000, 30);
    }

    // ---- decorative helpers ----

    void BuildGrid(Transform parent)
    {
        const int cols = 16;
        const int rows = 9;
        for (int c = 1; c < cols; c++)
        {
            GameObject line = CreateUIElement("VLine", parent);
            Image img = line.AddComponent<Image>();
            img.color = GridLine;
            img.raycastTarget = false;
            RectTransform r = line.GetComponent<RectTransform>();
            r.anchorMin = new Vector2((float)c / cols, 0f);
            r.anchorMax = new Vector2((float)c / cols, 1f);
            r.sizeDelta = new Vector2(2, 0);
            r.anchoredPosition = Vector2.zero;
        }
        for (int rr = 1; rr < rows; rr++)
        {
            GameObject line = CreateUIElement("HLine", parent);
            Image img = line.AddComponent<Image>();
            img.color = GridLine;
            img.raycastTarget = false;
            RectTransform r = line.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, (float)rr / rows);
            r.anchorMax = new Vector2(1f, (float)rr / rows);
            r.sizeDelta = new Vector2(0, 2);
            r.anchoredPosition = Vector2.zero;
        }
    }

    void AddHazardRule(Transform parent, Vector2 position, float width)
    {
        GameObject rule = CreateUIElement("HazardRule", parent);
        Image img = rule.AddComponent<Image>();
        img.color = new Color(Hazard.r, Hazard.g, Hazard.b, 0.5f);
        img.raycastTarget = false;
        RectTransform r = rule.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = position;
        r.sizeDelta = new Vector2(width, 3);
    }

    void AddBorder(Transform parent, Color color, float thickness)
    {
        MakeEdge(parent, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, thickness), color);
        MakeEdge(parent, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, thickness), color);
        MakeEdge(parent, new Vector2(0, 0), new Vector2(0, 1), new Vector2(thickness, 0), color);
        MakeEdge(parent, new Vector2(1, 0), new Vector2(1, 1), new Vector2(thickness, 0), color);
    }

    void MakeEdge(Transform parent, Vector2 aMin, Vector2 aMax, Vector2 size, Color color)
    {
        GameObject edge = CreateUIElement("Edge", parent);
        Image img = edge.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        RectTransform r = edge.GetComponent<RectTransform>();
        r.anchorMin = aMin;
        r.anchorMax = aMax;
        r.sizeDelta = size;
        r.anchoredPosition = Vector2.zero;
    }

    // ---- UI construction helpers ----

    Button CreateButton(string label, Transform parent, Vector2 position, Vector2 size,
        Color bgColor, Color textColor, int fontSize)
    {
        GameObject buttonObj = CreateUIElement(label + "Button", parent);
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.4f, 1.4f, 1.4f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        colors.selectedColor = Color.white;
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        AddBorder(buttonObj.transform, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2);

        GameObject textObj = CreateText("Text", buttonObj.transform, label, fontSize,
            FontStyle.Bold, textColor, TextAnchor.MiddleCenter);
        StretchToFill(textObj.GetComponent<RectTransform>());
        return btn;
    }

    GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.AddComponent<RectTransform>();
        obj.transform.SetParent(parent, false);
        return obj;
    }

    GameObject CreateText(string name, Transform parent, string content, int fontSize,
        FontStyle style, Color color, TextAnchor anchor)
    {
        GameObject obj = CreateUIElement(name, parent);
        Text text = obj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return obj;
    }

    void StretchToFill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }

    void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
