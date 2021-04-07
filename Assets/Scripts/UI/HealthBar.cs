using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HealthBar class is used to display a health bar
/// </summary>
public class HealthBar : MonoBehaviour
{
    // public elements used by prefab
    public Color green;
    public Color orange;
    public Color red;
    public float middleValue = 50f;
    public float maxWidth = 30f;
    public float minWidth = 2f;
    public float height = 5f;

    // Private elements
    private Transform source;
    private Image bar;
    private float offset = 3f;

    // Events
    public delegate void HealthBarEvents(HealthBar _hb);
    public event HealthBarEvents OnRemove;

    /// <summary>
    /// On Awake, find the image component
    /// </summary>
    private void Awake()
    {
        bar = GetComponent<Image>();
        if (bar == null) Debug.LogError("[HealthBar] Cannot find Image component!");
    }

    /// <summary>
    /// Setup method links the HealthBar to a transform and sets a maximum width
    /// </summary>
    /// <param name="_t">Transform</param>
    /// <param name="_maxW">Max width</param>
    public void Setup(Transform _t, float _maxW)
    {
        source = _t;
        if(source.TryGetComponent(out Tower _tower) || source.TryGetComponent(out HQCandidate _hqCandidate))
        {
            offset = 10f;
        }

        maxWidth = _maxW;
        if(bar == null) bar = GetComponent<Image>();
        if (bar == null)
        {
            Debug.LogError("[HealthBar] Cannot find Image component!");
        }
        else
        {
            bar.color = green;
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }

    /// <summary>
    /// Hide method hides the HealthBar
    /// </summary>
    public void Hide()
    {
        if(bar != null) bar.enabled = false;
    }

    /// <summary>
    /// Show method shows the HealthBar
    /// </summary>
    public void Show()
    {
        bar.enabled = true;
    }

    /// <summary>
    /// Remove method destroys the HealthBar and calls the OnRemove event
    /// </summary>
    public void Remove()
    {
        OnRemove?.Invoke(this);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// UpdatePosition method updates the HealthBar position over the source Transform it is linked to
    /// </summary>
    public void UpdatePosition()
    {
        bar.rectTransform.position = Camera.main.WorldToScreenPoint(source.position + new Vector3(0f, 0f, offset));
    }

    /// <summary>
    /// UpdateValue method updates the size and color of the HealthBar
    /// </summary>
    /// <param name="current">Current value</param>
    /// <param name="max">Max value</param>
    public void UpdateValue(int current, int max)
    {
        float _value = 100f * (float)current / (float)max;
        if(_value >= middleValue)
        {
            bar.color = Color.Lerp(orange, green, (_value - middleValue) / middleValue);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (_value * maxWidth)/100f);
        }
        else
        {
            bar.color = Color.Lerp(red, orange, 2 * _value / middleValue);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(minWidth,(_value * maxWidth) / 100f));
        }
    }
}
