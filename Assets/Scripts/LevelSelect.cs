using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Lab-themed "test chamber" selection screen. Everything is built in code so the
// scene only needs this one component. Each level is a containment chamber the
// player can select; cards react to the cursor (lift + glow) and carry a pulsing
// "ONLINE" status light so the screen feels like a live lab console.
public class LevelSelect : MonoBehaviour
{
    [Serializable]
    public struct LevelEntry
    {
        public string name;
        public string sceneName;
        public bool locked;
    }

    // All three chambers ship unlocked. (The scene may override this array; the
    // serialized copy in LevelSelect.unity is kept in sync with these defaults.)
    public LevelEntry[] levels = new[]
    {
        new LevelEntry { name = "Level 1", sceneName = "Level_1", locked = false },
        new LevelEntry { name = "Level 2", sceneName = "Level_2", locked = false },
        new LevelEntry { name = "Level 3", sceneName = "Level_3", locked = false },
    };

    // ---- Lab palette ----
    static readonly Color BgDark      = new Color(0.055f, 0.078f, 0.070f);
    static readonly Color GridLine    = new Color(0.35f, 0.85f, 0.68f, 0.05f);
    static readonly Color Accent      = new Color(0.27f, 0.85f, 0.64f);   // teal-green
    static readonly Color AccentDim   = new Color(0.16f, 0.34f, 0.30f);
    static readonly Color Hazard      = new Color(0.90f, 0.68f, 0.24f);   // amber
    static readonly Color TextBright  = new Color(0.85f, 0.94f, 0.91f);
    static readonly Color TextMuted   = new Color(0.42f, 0.52f, 0.48f);
    static readonly Color FrameNormal = new Color(0.16f, 0.32f, 0.28f);
    static readonly Color FrameHover  = new Color(0.30f, 0.82f, 0.62f);
    static readonly Color ChamberFill = new Color(0.085f, 0.13f, 0.12f);
    static readonly Color ScreenFill  = new Color(0.05f, 0.085f, 0.078f);
    static readonly Color LockFill    = new Color(0.11f, 0.12f, 0.12f);

    void Awake()
    {
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

        // Background + faint monitor grid.
        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = BgDark;
        StretchToFill(bg.GetComponent<RectTransform>());
        BuildGrid(bg.transform);

        // Sweeping scanline for the "live console" feel.
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

        // ---- Header ----
        GameObject header = CreateText("Header", canvasObj.transform, "SELECT  TEST  CHAMBER",
            56, FontStyle.Bold, TextBright, TextAnchor.MiddleCenter);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.5f, 1f);
        headerRect.anchorMax = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0, -90);
        headerRect.sizeDelta = new Vector2(1200, 90);

        GameObject subhead = CreateText("Subheader", canvasObj.transform,
            "// FACILITY 7   —   SUBJECT: LAB RAT   —   CLEARANCE: GRANTED",
            24, FontStyle.Normal, Accent, TextAnchor.MiddleCenter);
        RectTransform subRect = subhead.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 1f);
        subRect.anchorMax = new Vector2(0.5f, 1f);
        subRect.anchoredPosition = new Vector2(0, -158);
        subRect.sizeDelta = new Vector2(1400, 40);
        AddHazardRule(canvasObj.transform, new Vector2(0, 344), 900f);

        // ---- Chamber grid ----
        int count = levels.Length;
        float cellWidth = 380f;
        float cellHeight = 470f;
        float spacingX = 60f;
        float gridWidth = count * cellWidth + (count - 1) * spacingX;
        float startX = -gridWidth / 2f + cellWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * (cellWidth + spacingX);
            CreateChamber(levels[i], canvasObj.transform, new Vector2(x, -10),
                new Vector2(cellWidth, cellHeight), i);
        }

        // ---- Footer ----
        AddHazardRule(canvasObj.transform, new Vector2(0, -360), 900f);
        GameObject footer = CreateText("Footer", canvasObj.transform,
            "> AWAITING SUBJECT INPUT", 20, FontStyle.Normal, TextMuted, TextAnchor.MiddleCenter);
        RectTransform footRect = footer.GetComponent<RectTransform>();
        footRect.anchorMin = new Vector2(0.5f, 0.5f);
        footRect.anchorMax = new Vector2(0.5f, 0.5f);
        footRect.anchoredPosition = new Vector2(0, -404);
        footRect.sizeDelta = new Vector2(800, 30);

        Button backBtn = CreateSimpleButton("< BACK TO MENU", canvasObj.transform,
            new Vector2(0, -470), new Vector2(280, 62), AccentDim, TextBright, 26);
        backBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    // A single containment-chamber card.
    void CreateChamber(LevelEntry level, Transform parent, Vector2 position, Vector2 size, int index)
    {
        // Card root doubles as the outer frame + click/hover target.
        GameObject card = CreateUIElement("Chamber_" + index, parent);
        Image frame = card.AddComponent<Image>();
        frame.color = level.locked ? new Color(0.14f, 0.14f, 0.15f) : FrameNormal;
        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        // Inner recessed body.
        GameObject body = CreateUIElement("Body", card.transform);
        Image bodyImg = body.AddComponent<Image>();
        bodyImg.color = level.locked ? LockFill : ChamberFill;
        bodyImg.raycastTarget = false;
        RectTransform bodyRect = body.GetComponent<RectTransform>();
        Inset(bodyRect, 6);

        // Corner brackets (scanner reticle look).
        AddCornerBrackets(body.transform, level.locked ? TextMuted : Accent);

        // Chamber index label, e.g. "CHAMBER 01".
        GameObject tag = CreateText("Tag", body.transform, "CHAMBER " + (index + 1).ToString("00"),
            22, FontStyle.Bold, level.locked ? TextMuted : Accent, TextAnchor.MiddleCenter);
        RectTransform tagRect = tag.GetComponent<RectTransform>();
        tagRect.anchorMin = new Vector2(0, 1);
        tagRect.anchorMax = new Vector2(1, 1);
        tagRect.anchoredPosition = new Vector2(0, -34);
        tagRect.sizeDelta = new Vector2(0, 34);

        // Big "specimen window" showing the level number.
        GameObject screen = CreateUIElement("Screen", body.transform);
        Image screenImg = screen.AddComponent<Image>();
        screenImg.color = ScreenFill;
        screenImg.raycastTarget = false;
        RectTransform screenRect = screen.GetComponent<RectTransform>();
        screenRect.anchorMin = new Vector2(0.5f, 0.5f);
        screenRect.anchorMax = new Vector2(0.5f, 0.5f);
        screenRect.anchoredPosition = new Vector2(0, 40);
        screenRect.sizeDelta = new Vector2(size.x - 80, 210);

        GameObject bigNum = CreateText("BigNum", screen.transform, (index + 1).ToString(),
            150, FontStyle.Bold, level.locked ? new Color(0.28f, 0.28f, 0.3f) : TextBright,
            TextAnchor.MiddleCenter);
        StretchToFill(bigNum.GetComponent<RectTransform>());

        // Level name plate.
        GameObject nameObj = CreateText("Name", body.transform, level.name.ToUpper(),
            30, FontStyle.Bold, level.locked ? TextMuted : TextBright, TextAnchor.MiddleCenter);
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0);
        nameRect.anchoredPosition = new Vector2(0, 96);
        nameRect.sizeDelta = new Vector2(0, 40);

        // Status row: pulsing light + label.
        GameObject light = CreateUIElement("StatusLight", body.transform);
        Image lightImg = light.AddComponent<Image>();
        lightImg.color = level.locked ? new Color(0.5f, 0.3f, 0.3f) : Accent;
        lightImg.raycastTarget = false;
        RectTransform lightRect = light.GetComponent<RectTransform>();
        lightRect.anchorMin = new Vector2(0.5f, 0);
        lightRect.anchorMax = new Vector2(0.5f, 0);
        lightRect.anchoredPosition = new Vector2(-70, 54);
        lightRect.sizeDelta = new Vector2(16, 16);
        if (!level.locked)
            light.AddComponent<Pulse>().Init(lightImg, Accent, new Color(0.1f, 0.3f, 0.25f), 2.6f);

        CreateText("Status", body.transform, level.locked ? "SEALED" : "ONLINE",
            22, FontStyle.Bold, level.locked ? new Color(0.7f, 0.4f, 0.4f) : Accent,
            TextAnchor.MiddleLeft, out RectTransform statusRect);
        statusRect.anchorMin = new Vector2(0.5f, 0);
        statusRect.anchorMax = new Vector2(0.5f, 0);
        statusRect.anchoredPosition = new Vector2(30, 54);
        statusRect.sizeDelta = new Vector2(180, 30);

        // Hover-only "> ENTER" prompt overlaid on the screen.
        GameObject prompt = CreateText("Prompt", screen.transform, "> ENTER",
            34, FontStyle.Bold, Accent, TextAnchor.MiddleCenter);
        Text promptText = prompt.GetComponent<Text>();
        promptText.color = new Color(Accent.r, Accent.g, Accent.b, 0f);
        StretchToFill(prompt.GetComponent<RectTransform>());

        if (level.locked)
            return;

        // Wire up interaction: whole card clickable + hover lift/glow.
        Button btn = card.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        string sceneName = level.sceneName;
        btn.onClick.AddListener(() => SceneManager.LoadScene(sceneName));

        ChamberHover hover = card.AddComponent<ChamberHover>();
        hover.Init(rect, frame, FrameNormal, FrameHover, bigNum.GetComponent<Text>(),
            TextBright, ScreenFill, screenImg, promptText);
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

    void AddCornerBrackets(Transform parent, Color color)
    {
        Vector2[] anchors =
        {
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)
        };
        foreach (Vector2 a in anchors)
        {
            float dirX = a.x < 0.5f ? 1f : -1f;
            float dirY = a.y < 0.5f ? 1f : -1f;
            MakeBar(parent, a, new Vector2(28, 4), new Vector2(dirX * 16, dirY * 10), color);
            MakeBar(parent, a, new Vector2(4, 28), new Vector2(dirX * 10, dirY * 16), color);
        }
    }

    void MakeBar(Transform parent, Vector2 anchor, Vector2 size, Vector2 offset, Color color)
    {
        GameObject bar = CreateUIElement("Bar", parent);
        Image img = bar.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        RectTransform r = bar.GetComponent<RectTransform>();
        r.anchorMin = anchor;
        r.anchorMax = anchor;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = size;
        r.anchoredPosition = offset;
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

    // ---- UI construction helpers ----

    Button CreateSimpleButton(string label, Transform parent, Vector2 position,
        Vector2 size, Color bgColor, Color textColor, int fontSize)
    {
        GameObject buttonObj = CreateUIElement(label + "Button", parent);
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.35f, 1.35f, 1.35f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

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
        return CreateText(name, parent, content, fontSize, style, color, anchor, out _);
    }

    GameObject CreateText(string name, Transform parent, string content, int fontSize,
        FontStyle style, Color color, TextAnchor anchor, out RectTransform rect)
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
        rect = obj.GetComponent<RectTransform>();
        return obj;
    }

    void Inset(RectTransform rect, float amount)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(amount, amount);
        rect.offsetMax = new Vector2(-amount, -amount);
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

// Lifts and lights up a chamber card while the cursor is over it. Added in code,
// so it does not need its own file.
public class ChamberHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform target;
    Graphic frame;
    Color frameNormal, frameHover;
    Graphic bigNum;
    Color numNormal, numHover;
    Graphic screen;
    Color screenNormal, screenHover;
    Graphic prompt;
    Color promptColor;
    float t;
    bool hovering;

    public void Init(RectTransform target, Graphic frame, Color frameNormal, Color frameHover,
        Graphic bigNum, Color numHover, Color screenNormal, Graphic screen, Graphic prompt)
    {
        this.target = target;
        this.frame = frame;
        this.frameNormal = frameNormal;
        this.frameHover = frameHover;
        this.bigNum = bigNum;
        this.numNormal = bigNum.color;
        this.numHover = numHover;
        this.screen = screen;
        this.screenNormal = screenNormal;
        this.screenHover = new Color(frameHover.r * 0.18f, frameHover.g * 0.22f, frameHover.b * 0.2f);
        this.prompt = prompt;
        this.promptColor = prompt.color;
    }

    public void OnPointerEnter(PointerEventData e) => hovering = true;
    public void OnPointerExit(PointerEventData e) => hovering = false;

    void Update()
    {
        t = Mathf.MoveTowards(t, hovering ? 1f : 0f, Time.unscaledDeltaTime * 7f);
        if (target != null)
            target.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.05f, t);
        if (frame != null)
            frame.color = Color.Lerp(frameNormal, frameHover, t);
        if (screen != null)
            screen.color = Color.Lerp(screenNormal, screenHover, t);
        if (bigNum != null)
            bigNum.color = Color.Lerp(numNormal, numHover, t * 0.4f);
        if (prompt != null)
            prompt.color = new Color(promptColor.r, promptColor.g, promptColor.b, t);
    }
}

// Sinusoidally pulses a graphic's color between two values.
public class Pulse : MonoBehaviour
{
    Graphic graphic;
    Color a, b;
    float speed;

    public void Init(Graphic graphic, Color a, Color b, float speed)
    {
        this.graphic = graphic;
        this.a = a;
        this.b = b;
        this.speed = speed;
    }

    void Update()
    {
        if (graphic == null) return;
        float k = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
        graphic.color = Color.Lerp(a, b, k);
    }
}

// Slides a highlight bar down the screen and wraps, like a CRT scan.
public class Scanline : MonoBehaviour
{
    RectTransform rect;
    float y;

    public void Init(RectTransform rect)
    {
        this.rect = rect;
    }

    void Update()
    {
        if (rect == null) return;
        y -= Time.unscaledDeltaTime * 260f;
        if (y < -1200f) y = 0f;
        rect.anchoredPosition = new Vector2(0, y);
    }
}
