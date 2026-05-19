using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    private int maxValue = 1;

    public void SetMax(int max)
    {
        maxValue = Mathf.Max(1, max);
        Set(max);
    }

    public void Set(int current)
    {
        if (fillImage == null) return;
        float v = Mathf.Clamp01((float)current / (float)maxValue);
        fillImage.fillAmount = v;
    }
}
