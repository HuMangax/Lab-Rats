using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Serializable]
    public struct LevelEntry
    {
        public string name;
        public string sceneName;
        public bool locked;
    }

    public LevelEntry[] levels = new[]
    {
        new LevelEntry { name = "Level 1", sceneName = "Level_1", locked = false },
        new LevelEntry { name = "Level 2", sceneName = "Level_2", locked = false },
        new LevelEntry { name = "Level 3", sceneName = "", locked = true },
        new LevelEntry { name = "Level 4", sceneName = "", locked = true },
        new LevelEntry { name = "Level 5", sceneName = "", locked = true },
        new LevelEntry { name = "Level 6", sceneName = "", locked = true },
    };

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

        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.12f, 0.12f, 0.15f, 1f);
        StretchToFill(bg.GetComponent<RectTransform>());

        GameObject header = CreateUIElement("Header", canvasObj.transform);
        Text headerText = header.AddComponent<Text>();
        headerText.text = "SELECT LEVEL";
        headerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        headerText.fontSize = 60;
        headerText.alignment = TextAnchor.MiddleCenter;
        headerText.color = new Color(0.9f, 0.85f, 0.7f);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.5f, 1f);
        headerRect.anchorMax = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0, -80);
        headerRect.sizeDelta = new Vector2(600, 100);

        int columns = 3;
        float cellWidth = 280f;
        float cellHeight = 160f;
        float spacingX = 40f;
        float spacingY = 40f;
        float gridWidth = columns * cellWidth + (columns - 1) * spacingX;
        float startX = -gridWidth / 2f + cellWidth / 2f;
        float startY = 100f;

        for (int i = 0; i < levels.Length; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float x = startX + col * (cellWidth + spacingX);
            float y = startY - row * (cellHeight + spacingY);

            CreateLevelButton(levels[i], canvasObj.transform, new Vector2(x, y),
                new Vector2(cellWidth, cellHeight), i);
        }

        Button backBtn = CreateSimpleButton("Back", canvasObj.transform,
            new Vector2(0, -340), new Vector2(200, 60),
            new Color(0.5f, 0.45f, 0.4f), new Color(0.95f, 0.95f, 0.9f), 30);
        backBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    void CreateLevelButton(LevelEntry level, Transform parent, Vector2 position,
        Vector2 size, int index)
    {
        Color bgColor = level.locked
            ? new Color(0.3f, 0.3f, 0.3f)
            : new Color(0.25f, 0.45f, 0.55f);
        Color textColor = level.locked
            ? new Color(0.5f, 0.5f, 0.5f)
            : new Color(0.95f, 0.95f, 0.9f);

        GameObject buttonObj = CreateUIElement("Level_" + index, parent);
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = buttonObj.AddComponent<Button>();

        if (level.locked)
        {
            btn.interactable = false;
        }
        else
        {
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(bgColor.r + 0.15f, bgColor.g + 0.15f, bgColor.b + 0.15f);
            colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f);
            btn.colors = colors;

            string sceneName = level.sceneName;
            btn.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
        }

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        GameObject nameObj = CreateUIElement("Name", buttonObj.transform);
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = level.name;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 32;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = textColor;
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.15f);
        nameRect.anchorMax = new Vector2(1, 0.85f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = Vector2.zero;

        if (level.locked)
        {
            GameObject lockObj = CreateUIElement("Lock", buttonObj.transform);
            Text lockText = lockObj.AddComponent<Text>();
            lockText.text = "LOCKED";
            lockText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            lockText.fontSize = 16;
            lockText.alignment = TextAnchor.LowerCenter;
            lockText.color = new Color(0.6f, 0.4f, 0.4f);
            RectTransform lockRect = lockObj.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(0, 0);
            lockRect.anchorMax = new Vector2(1, 0.3f);
            lockRect.sizeDelta = Vector2.zero;
            lockRect.anchoredPosition = Vector2.zero;
        }
    }

    Button CreateSimpleButton(string label, Transform parent, Vector2 position,
        Vector2 size, Color bgColor, Color textColor, int fontSize)
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
        rect.sizeDelta = size;

        GameObject textObj = CreateUIElement("Text", buttonObj.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
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
