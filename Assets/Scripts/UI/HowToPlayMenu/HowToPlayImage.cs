using UnityEngine;
using UnityEngine.UI;

public class HowToPlayImage : MonoBehaviour
{
    private Image mainImage;

    // Start is called before the first frame update
    void Start()
    {
        mainImage = GetComponent<Image>();
        HowToPlayButton.OnButtonSelected += UpdateImage;
    }

    private void OnDestroy()
    {
        HowToPlayButton.OnButtonSelected += UpdateImage;
    }

    private void UpdateImage(Sprite _sprite)
    {
        mainImage.sprite = _sprite;
    }

}
