using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Reached after the final chamber (Level 3). Framed as an experiment write-up:
// the lab rat has escaped every test chamber. Rising specimen-tank bubbles, a
// glowing title and a typed-out "lab report" give it an experimental feel.
public class Congratulations : MonoBehaviour
{
    [Header("Scene routing")]
    public string playAgainScene = "Level_1";
    public string mainMenuScene = "MainMenu";

    static readonly Color BgDark     = new Color(0.055f, 0.078f, 0.070f);
    static readonly Color GridLine   = new Color(0.35f, 0.85f, 0.68f, 0.05f);
    static readonly Color Accent     = new Color(0.27f, 0.85f, 0.64f);
    static readonly Color AccentDeep = new Color(0.10f, 0.30f, 0.25f);
    static readonly Color Hazard     = new Color(0.90f, 0.68f, 0.24f);
    static readonly Color TextBright = new Color(0.88f, 0.96f, 0.93f);
    static readonly Color TextMuted  = new Color(0.45f, 0.55f, 0.51f);
    static readonly Color PanelFill  = new Color(0.075f, 0.115f, 0.105f, 0.96f);

    void Awake()
    {
        Time.timeScale = 1f; // the exit door froze the game; make sure it is running here
        CreateUI();
    }

    void CreateUI()
    {
        EnsureEventSystem();

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Background + faint grid.
        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = BgDark;
        StretchToFill(bg.GetComponent<RectTransform>());
        BuildGrid(bg.transform);

        // Rising bubbles behind everything.
        GameObject bubbles = CreateUIElement("Bubbles", canvasObj.transform);
        StretchToFill(bubbles.GetComponent<RectTransform>());
        bubbles.AddComponent<BubbleField>().Init(bubbles.GetComponent<RectTransform>(),
            MakeCircleSprite(48), 26, Accent);

        // ---- Title ----
        GameObject title = CreateText("Title", canvasObj.transform, "EXPERIMENT COMPLETE",
            84, FontStyle.Bold, Accent, TextAnchor.MiddleCenter);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 300);
        titleRect.sizeDelta = new Vector2(1500, 120);
        title.AddComponent<Pulse>().Init(title.GetComponent<Text>(),
            Accent, new Color(0.55f, 1f, 0.85f), 2.2f);

        GameObject subtitle = CreateText("Subtitle", canvasObj.transform,
            "// ALL TEST CHAMBERS CLEARED", 30, FontStyle.Normal, TextMuted, TextAnchor.MiddleCenter);
        RectTransform subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = new Vector2(0, 226);
        subRect.sizeDelta = new Vector2(1200, 40);

        // ---- Lab report panel ----
        GameObject panel = CreateUIElement("ReportPanel", canvasObj.transform);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = PanelFill;
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0, 10);
        panelRect.sizeDelta = new Vector2(760, 320);
        AddBorder(panel.transform, AccentDeep, 3);

        GameObject panelHead = CreateText("ReportHead", panel.transform, "  LAB REPORT // FILE #0007",
            24, FontStyle.Bold, Accent, TextAnchor.MiddleLeft);
        RectTransform phRect = panelHead.GetComponent<RectTransform>();
        phRect.anchorMin = new Vector2(0, 1);
        phRect.anchorMax = new Vector2(1, 1);
        phRect.anchoredPosition = new Vector2(0, -36);
        phRect.sizeDelta = new Vector2(0, 36);
        AddHazardRule(panel.transform, new Vector2(0, -62), 700f);

        string[] lines =
        {
            "SUBJECT STATUS .......... LIBERATED",
            "CHAMBERS CLEARED ........ 3 / 3",
            "CONTAINMENT INTEGRITY ... BREACHED",
            "HYPOTHESIS .............. REJECTED",
            "",
            "CONCLUSION: THE RAT OUTSMARTED THE LAB.",
        };
        float lineY = -96;
        float delay = 0.35f;
        foreach (string line in lines)
        {
            bool emphasis = line.StartsWith("CONCLUSION");
            GameObject row = CreateText("Line", panel.transform, "",
                emphasis ? 26 : 24, emphasis ? FontStyle.Bold : FontStyle.Normal,
                emphasis ? Hazard : TextBright, TextAnchor.MiddleLeft);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0, 1);
            rowRect.anchorMax = new Vector2(1, 1);
            rowRect.anchoredPosition = new Vector2(48, lineY);
            rowRect.sizeDelta = new Vector2(-80, 32);
            row.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            if (line.Length > 0)
            {
                row.AddComponent<Typewriter>().Init(row.GetComponent<Text>(), line, 34f, delay);
                delay += 0.35f + line.Length * 0.028f;
            }
            lineY -= emphasis ? 46 : 34;
        }

        // ---- Buttons ----
        Button playBtn = CreateButton("PLAY AGAIN", canvasObj.transform, new Vector2(-170, -260),
            AccentDeep, TextBright);
        playBtn.onClick.AddListener(() => SceneManager.LoadScene(playAgainScene));

        Button menuBtn = CreateButton("MAIN MENU", canvasObj.transform, new Vector2(170, -260),
            new Color(0.16f, 0.17f, 0.18f), TextBright);
        menuBtn.onClick.AddListener(() => SceneManager.LoadScene(mainMenuScene));

        GameObject footer = CreateText("Footer", canvasObj.transform,
            "THANK YOU FOR PARTICIPATING IN THE STUDY", 20, FontStyle.Normal,
            TextMuted, TextAnchor.MiddleCenter);
        RectTransform footRect = footer.GetComponent<RectTransform>();
        footRect.anchorMin = new Vector2(0.5f, 0);
        footRect.anchorMax = new Vector2(0.5f, 0);
        footRect.anchoredPosition = new Vector2(0, 50);
        footRect.sizeDelta = new Vector2(1200, 30);
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

    void AddBorder(Transform parent, Color color, float thickness)
    {
        MakeEdge(parent, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, thickness), color); // top
        MakeEdge(parent, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, thickness), color); // bottom
        MakeEdge(parent, new Vector2(0, 0), new Vector2(0, 1), new Vector2(thickness, 0), color); // left
        MakeEdge(parent, new Vector2(1, 0), new Vector2(1, 1), new Vector2(thickness, 0), color); // right
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

    void AddHazardRule(Transform parent, Vector2 position, float width)
    {
        GameObject rule = CreateUIElement("HazardRule", parent);
        Image img = rule.AddComponent<Image>();
        img.color = new Color(Hazard.r, Hazard.g, Hazard.b, 0.5f);
        img.raycastTarget = false;
        RectTransform r = rule.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 1f);
        r.anchorMax = new Vector2(0.5f, 1f);
        r.anchoredPosition = position;
        r.sizeDelta = new Vector2(width, 2);
    }

    // Soft round dot used for the rising bubbles.
    Sprite MakeCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float r = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - r;
                float dy = y + 0.5f - r;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                // Bright thin ring, faint fill -> looks like a bubble.
                float ring = Mathf.SmoothStep(1f, 0f, Mathf.Abs(d - 0.82f) / 0.18f);
                float fill = Mathf.SmoothStep(1f, 0f, d) * 0.25f;
                float a = Mathf.Clamp01(Mathf.Max(ring, fill));
                if (d > 1f) a = 0f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // ---- UI construction helpers ----

    Button CreateButton(string label, Transform parent, Vector2 position, Color bgColor, Color textColor)
    {
        GameObject buttonObj = CreateUIElement(label + "Button", parent);
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.4f, 1.4f, 1.4f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 74);
        AddBorder(buttonObj.transform, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2);

        GameObject textObj = CreateText("Text", buttonObj.transform, label, 30,
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
}

// Reveals a Text one character at a time after an initial delay.
public class Typewriter : MonoBehaviour
{
    Text text;
    string full;
    float charsPerSecond;
    float delay;
    float elapsed;

    public void Init(Text text, string full, float charsPerSecond, float delay)
    {
        this.text = text;
        this.full = full;
        this.charsPerSecond = charsPerSecond;
        this.delay = delay;
        text.text = "";
    }

    void Update()
    {
        if (text == null) return;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed < delay) return;
        int shown = Mathf.Min(full.Length, Mathf.FloorToInt((elapsed - delay) * charsPerSecond));
        text.text = full.Substring(0, shown);
        if (shown >= full.Length) enabled = false;
    }
}

// Floats a field of translucent bubbles upward, wrapping around the screen.
public class BubbleField : MonoBehaviour
{
    class Bubble
    {
        public RectTransform rt;
        public float speed;
        public float wobbleAmp;
        public float wobbleFreq;
        public float phase;
        public float baseX;
    }

    Bubble[] bubbles;
    const float HalfW = 960f;
    const float HalfH = 540f;

    public void Init(RectTransform area, Sprite sprite, int count, Color color)
    {
        bubbles = new Bubble[count];
        for (int i = 0; i < count; i++)
        {
            GameObject b = new GameObject("Bubble");
            b.transform.SetParent(area, false);
            Image img = b.AddComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            float size = Random.Range(14f, 52f);
            img.color = new Color(color.r, color.g, color.b, Random.Range(0.10f, 0.35f));

            RectTransform rt = b.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(size, size);
            float x = Random.Range(-HalfW, HalfW);
            rt.anchoredPosition = new Vector2(x, Random.Range(-HalfH, HalfH));

            bubbles[i] = new Bubble
            {
                rt = rt,
                speed = Random.Range(40f, 110f),
                wobbleAmp = Random.Range(10f, 40f),
                wobbleFreq = Random.Range(0.5f, 1.6f),
                phase = Random.Range(0f, Mathf.PI * 2f),
                baseX = x,
            };
        }
    }

    void Update()
    {
        if (bubbles == null) return;
        float dt = Time.unscaledDeltaTime;
        foreach (Bubble b in bubbles)
        {
            Vector2 p = b.rt.anchoredPosition;
            p.y += b.speed * dt;
            if (p.y > HalfH + 60f)
            {
                p.y = -HalfH - 60f;
                b.baseX = Random.Range(-HalfW, HalfW);
            }
            b.phase += b.wobbleFreq * dt;
            p.x = b.baseX + Mathf.Sin(b.phase) * b.wobbleAmp;
            b.rt.anchoredPosition = p;
        }
    }
}
