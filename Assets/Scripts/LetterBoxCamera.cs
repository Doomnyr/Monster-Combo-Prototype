using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    public Vector2 targetAspect = new Vector2(16, 9);

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float targetRatio = targetAspect.x / targetAspect.y;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetRatio;

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