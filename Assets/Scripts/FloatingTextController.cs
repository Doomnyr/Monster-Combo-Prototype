using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private UnityEngine.UI.Image iconImage;
    
    [Header("Movement Settings")]
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private Vector2 moveSpeedRangeY = new Vector2(50f, 100f);
    
    [Header("Bounce/Scale Curve")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private float _elapsedTime;
    private Vector2 _currentMoveSpeed;
    private Color _baseColor;

    private void Awake()
    {
        // Automatically looks down into child GameObjects to find the Text Mesh if unassigned!
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Automatically looks down into child GameObjects to find the Image component if unassigned!
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<UnityEngine.UI.Image>();
        }
    }

    public void Setup(string text, Color color, Sprite buffIcon = null)
    {
        textMesh.text = text;
        textMesh.color = color;
        _baseColor = color;

        if (iconImage != null)
        {
            if (buffIcon != null)
            {
                iconImage.sprite = buffIcon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false); // Hide the image container cleanly for normal damage/heals!
            }
        }

        // Determine a clean upward speed
        _currentMoveSpeed = new Vector2(
            0f,
            Random.Range(moveSpeedRangeY.x, moveSpeedRangeY.y)
        );

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

        // 1. Float straight up
        transform.Translate(_currentMoveSpeed * Time.deltaTime);

        // 2. Animate Scale (pop up quickly, then shrink)
        float currentScale = scaleCurve.Evaluate(normalizedTime);
        transform.localScale = Vector3.one * currentScale;

        // 3. Fade Out (Fading both text and the icon simultaneously)
        float currentAlpha = alphaCurve.Evaluate(normalizedTime);
        textMesh.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, currentAlpha);
        
        if (iconImage != null && iconImage.gameObject.activeSelf)
        {
            iconImage.color = new Color(1f, 1f, 1f, currentAlpha);
        }
    }
}