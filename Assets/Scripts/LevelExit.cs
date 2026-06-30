using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Goal door: when the player walks into its trigger, the level is "cleared"
// and a Level Clear screen appears with Next Level / Main Menu options.
public class LevelExit : MonoBehaviour
{
    [Header("Leave blank to use the next scene in Build Settings (else Level Select)")]
    public string nextSceneName = "";

    private bool cleared;
    private GameObject canvasObj;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;
        if (other.GetComponent<PlayerScript>() == null) return;
        ClearLevel();
    }

    void ClearLevel()
    {
        cleared = true;
        Time.timeScale = 0f;
        CreateClearUI();
    }

    void CreateClearUI()
    {
        EnsureEventSystem();

        canvasObj = new GameObject("LevelClearCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject overlay = CreateUIElement("Overlay", canvasObj.transform);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.6f);
        StretchToFill(overlay.GetComponent<RectTransform>());

        GameObject panel = CreateUIElement("Panel", canvasObj.transform);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.13f, 0.16f, 0.14f, 0.96f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(420, 360);

        GameObject title = CreateUIElement("Title", panel.transform);
        Text titleText = title.AddComponent<Text>();
        titleText.text = "LEVEL CLEAR!";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 54;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.45f, 0.95f, 0.4f);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.anchoredPosition = new Vector2(0, -65);
        titleRect.sizeDelta = new Vector2(0, 90);

        GameObject subtitle = CreateUIElement("Subtitle", panel.transform);
        Text subText = subtitle.AddComponent<Text>();
        subText.text = "The test subject escaped!";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 22;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.7f, 0.7f, 0.6f);
        RectTransform subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0, 1);
        subRect.anchorMax = new Vector2(1, 1);
        subRect.anchoredPosition = new Vector2(0, -120);
        subRect.sizeDelta = new Vector2(0, 40);

        Button nextBtn = CreateButton("Next Level", panel.transform, new Vector2(0, -30),
            new Color(0.3f, 0.6f, 0.3f), new Color(0.95f, 0.95f, 0.9f));
        nextBtn.onClick.AddListener(LoadNextLevel);

        Button menuBtn = CreateButton("Main Menu", panel.transform, new Vector2(0, -125),
            new Color(0.5f, 0.45f, 0.4f), new Color(0.95f, 0.95f, 0.9f));
        menuBtn.onClick.AddListener(GoToMainMenu);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("LevelSelect"); // no further levels yet
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ---- UI helpers (mirrors PauseMenu's construction) ----
    Button CreateButton(string label, Transform parent, Vector2 position, Color bgColor, Color textColor)
    {
        GameObject buttonObj = CreateUIElement(label + "Button", parent);
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = buttonObj.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(bgColor.r + 0.15f, bgColor.g + 0.15f, bgColor.b + 0.15f);
        colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f);
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300, 70);

        GameObject textObj = CreateUIElement("Text", buttonObj.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = textColor;
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
