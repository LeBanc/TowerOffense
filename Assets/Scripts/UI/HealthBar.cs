using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Color green;
    public Color orange;
    public Color red;
    public float middleValue = 50f;
    public float maxWidth = 30f;
    public float minWidth = 2f;
    public float height = 5f;

    private Transform source;
    private Image bar;

    public float test = 100f;

    private void Awake()
    {
        bar = GetComponent<Image>();
        if (bar == null) Debug.LogError("[HelthBar] Cannot find Image component!");
    }

    public void Setup(Transform _t, float _maxW)
    {
        source = _t;
        maxWidth = _maxW;
        bar.color = green;
        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void Hide()
    {
        bar.enabled = false;
    }

    public void Show()
    {
        bar.enabled = true;
    }

    public void Remove()
    {
        if(gameObject != null) Destroy(gameObject);
    }

    public void UpdatePosition()
    {
        bar.rectTransform.position = Camera.main.WorldToScreenPoint(source.position + new Vector3(0f, 0f, 2f));
    }

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
