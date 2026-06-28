using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIconUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stacksText;

    public void Setup(BuffDefinitionSO buffDef, int stacks)
    {
        if (buffDef.buffIcon != null)
        {
            iconImage.sprite = buffDef.buffIcon;
            iconImage.enabled = true;
        }

        // Only show the number if it stacks, or if you are using stacks as duration
        if (stacks > 1)
        {
            stacksText.text = stacks.ToString();
            stacksText.gameObject.SetActive(true);
        }
        else
        {
            stacksText.gameObject.SetActive(false);
        }

        // Optional: Tint red if it's a debuff
        if (buffDef.isDebuff)
        {
            iconImage.color = new Color(1f, 0.5f, 0.5f); // Light red tint
        }
        else
        {
            iconImage.color = Color.white;
        }
    }
}