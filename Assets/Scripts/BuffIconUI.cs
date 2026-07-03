using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIconUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stacksText;

    private void Awake()
    {
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
        }

        if (stacksText == null)
        {
            stacksText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void Setup(BuffDefinitionSO buffDef, int stacks)
    {
        if (buffDef == null)
        {
            Debug.LogWarning("BuffIconUI.Setup called with null BuffDefinitionSO");
            return;
        }

        if (buffDef.buffIcon != null && iconImage != null)
        {
            iconImage.sprite = buffDef.buffIcon;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        // Only show the number if it stacks, or if you are using stacks as duration
        if (stacksText != null)
        {
            if (stacks > 1)
            {
                stacksText.text = stacks.ToString();
                stacksText.gameObject.SetActive(true);
            }
            else
            {
                stacksText.gameObject.SetActive(false);
            }
        }

        // Optional: Tint red if it's a debuff
        if (iconImage != null)
        {
            iconImage.color = buffDef.isDebuff ? new Color(1f, 0.5f, 0.5f) : Color.white;
        }
    }
}