using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "SampleScene";

    [Header("UI References (auto-created if null)")]
    public Canvas canvas;
    public Button playButton;
    public Button quitButton;

    void Awake()
    {
        if (canvas == null)
            CreateMenuUI();
    }

    void CreateMenuUI()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.12f, 0.12f, 0.15f, 1f);
        StretchToFill(bg.GetComponent<RectTransform>());

        GameObject title = CreateUIElement("Title", canvasObj.transform);
        Text titleText = title.AddComponent<Text>();
        titleText.text = "LAB RATS";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 80;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.9f, 0.85f, 0.7f);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(600, 120);

        GameObject subtitle = CreateUIElement("Subtitle", canvasObj.transform);
        Text subText = subtitle.AddComponent<Text>();
        subText.text = "Escape the Maze";
        subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subText.fontSize = 30;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.7f, 0.65f, 0.55f);
        RectTransform subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = new Vector2(0, 120);
        subRect.sizeDelta = new Vector2(400, 50);

        playButton = CreateButton("Play", canvasObj.transform, new Vector2(0, -20),
            new Color(0.3f, 0.6f, 0.3f), new Color(0.95f, 0.95f, 0.9f));
        playButton.onClick.AddListener(PlayGame);

        quitButton = CreateButton("Quit", canvasObj.transform, new Vector2(0, -120),
            new Color(0.6f, 0.3f, 0.3f), new Color(0.95f, 0.95f, 0.9f));
        quitButton.onClick.AddListener(QuitGame);
    }

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
        text.fontSize = 36;
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
