using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector2 gameAspect = new Vector2(16,9);

    private Camera cam;

    private int pixelWidth;
    private int pixelHeight;

    public static float width;
    public static float height;

    void Start()
    {
        cam = Camera.main;
        applyAspect();
        pixelWidth = Screen.width;
        pixelHeight = Screen.height;
    }

    void Update()
    {
        height = cam.orthographicSize;
        width = height * cam.aspect;

        if (pixelWidth != Screen.width || pixelHeight != Screen.height)
        {
            applyAspect();
            pixelWidth = Screen.width;
            pixelHeight = Screen.height;
        }
    }

    private void applyAspect()
    {
        float targetAspect = gameAspect.x / gameAspect.y;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = Camera.main;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
