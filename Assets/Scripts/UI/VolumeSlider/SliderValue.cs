using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SliderValue component display the value from a slider in a Text
/// </summary>
[RequireComponent(typeof(Text))]
public class SliderValue : MonoBehaviour
{
    // Slider from which get the value
    public Slider slider;
    // Coefficient to multiply the value with
    public int coeff = 1;

    private Text fieldValue;

    /// <summary>
    /// At Start, fetches the Text, subscribe to slider value change and update the Text
    /// </summary>
    private void Start()
    {
        fieldValue = GetComponent<Text>();
        if(slider != null)
        {
            slider.onValueChanged.AddListener(UpdateValue);
            fieldValue.text = ((int)(slider.value * coeff)).ToString();
        }
    }

    /// <summary>
    /// OnDestroy, unsubscribe to slider event
    /// </summary>
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateValue);
    }

    /// <summary>
    /// UpdateValue method update the Text value with value*coeff
    /// </summary>
    /// <param name="_val">New value to display (float)</param>
    public void UpdateValue(float _val)
    {
        fieldValue.text = ((int)(_val * coeff)).ToString();
    }
}
