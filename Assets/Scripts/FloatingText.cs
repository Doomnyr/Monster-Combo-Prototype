using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private UnityEngine.UI.Image iconImage;
    
    [Header("UI Drifting Speed")]
    [Tooltip("Speed to float upward in Canvas local pixels per second")]
    [SerializeField] private float driftSpeedY = 60f;
    [SerializeField] private float lifetime = 2.0f;
    
    [Header("Curves")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private RectTransform _rectTransform;
    private float _elapsedTime;
    private Color _baseColor;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Auto-locate missing references in children
        if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (iconImage == null) iconImage = GetComponentInChildren<UnityEngine.UI.Image>();
    }

    public void Setup(string text, Color color, Sprite iconSprite = null)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
            _baseColor = color;
            Debug.Log("Set up floating combat text.");
        }

        if (iconImage != null)
        {
            if (iconSprite != null)
            {
                iconImage.sprite = iconSprite;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false); // Hide the icon container for normal damage/heals
            }
        }

        // Reset local coordinates to center of parent spawn point
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        _elapsedTime = 0f;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float normalizedTime = _elapsedTime / lifetime;

        if (normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
            return;
        }

        // 1. Move upward safely in Canvas Pixel Units (No Worldspace Translate issues!)
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition += new Vector2(0f, driftSpeedY * Time.deltaTime);
        }

        // 2. Animate Scale (pop up quickly, then shrink)
        float currentScale = scaleCurve.Evaluate(normalizedTime);
        transform.localScale = Vector3.one * currentScale;

        // 3. Fade Out Text and Icon
        float currentAlpha = alphaCurve.Evaluate(normalizedTime);
        if (textMesh != null)
        {
            textMesh.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, currentAlpha);
        }
        
        if (iconImage != null && iconImage.gameObject.activeSelf)
        {
            iconImage.color = new Color(1f, 1f, 1f, currentAlpha);
        }
    }
}