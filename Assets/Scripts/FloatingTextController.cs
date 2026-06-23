using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    
    [Header("Movement Settings")]
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private Vector2 moveSpeedRangeY = new Vector2(50f, 100f);
    [SerializeField] private Vector2 moveSpeedRangeX = new Vector2(-30f, 30f);
    
    [Header("Bounce/Scale Curve")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private float _elapsedTime;
    private Vector2 _currentMoveSpeed;
    private Color _baseColor;

    private void Awake()
    {
        // Automatically looks down into child GameObjects to find the Text Mesh!
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        _baseColor = color;

        // Choose a random upward bounce and slight random sideways drift
        _currentMoveSpeed = new Vector2(
            Random.Range(0, moveSpeedRangeX.y),
            Random.Range(0, moveSpeedRangeY.y)
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

        // 1. Drift and Gravity
        //transform.Translate(_currentMoveSpeed * Time.deltaTime);
        //_currentMoveSpeed.y -= 150f * Time.deltaTime; // Apply slight gravity to curve the arc!

        // 2. Animate Scale (pop up quickly, then shrink)
        float currentScale = scaleCurve.Evaluate(normalizedTime);
        transform.localScale = Vector3.one * currentScale;

        // 3. Fade Out
        float currentAlpha = alphaCurve.Evaluate(normalizedTime);
        textMesh.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, currentAlpha);
    }
}