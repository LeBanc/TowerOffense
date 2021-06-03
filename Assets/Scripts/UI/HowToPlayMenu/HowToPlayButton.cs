using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HowToPlayButton : Button
{
    public string title;
    public string subtitle;
    public Sprite helpImage;

    public delegate void HowToPlayButtonEventHandler(Sprite _sprite);
    public static event HowToPlayButtonEventHandler OnButtonSelected;

    protected override void Start()
    {
        base.Start();
        transform.GetChild(0).GetComponent<Text>().text = title;
        transform.GetChild(1).GetComponent<Text>().text = subtitle;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnButtonSelected?.Invoke(helpImage);
    }

    public void UpdateText()
    {
        transform.GetChild(0).GetComponent<Text>().text = title;
        transform.GetChild(1).GetComponent<Text>().text = subtitle;
    }
}
