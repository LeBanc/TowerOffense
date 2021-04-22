using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// IntelServTowerItem class is for a Tower item of the Intelligence Services Canvas
/// </summary>
public class IntelServTowerItem : MonoBehaviour
{
    // public UI elements
    public Text towerTypeText;
    public Text positionFromHQText;
    public Text shortAtkText;
    public Text middleAtkText;
    public Text longAtkText;
    public Text exploAtkText;
    public Text shortDefText;
    public Text middleDefText;
    public Text longDefText;
    public Text exploDefText;

    public Image towerLevelImage;
    public Image towerTypeImage;


    // public Sprite references
    public Sprite level0;
    public Sprite level1;
    public Sprite level2;
    public Sprite level3;
    public Sprite level4;

    public Sprite shortRange;
    public Sprite middleRange;
    public Sprite longRange;
    public Sprite explosives;


    /// <summary>
    /// Setup method sets all the text field of the IntelServTowerItem
    /// </summary>
    /// <param name="_tower">Tower from which get the data (Tower)</param>
    public void Setup(Tower _tower)
    {
        TowerData _data = _tower.data;

        // Set the tower level image
        switch(_data.towerLevel)
        {
            case 0:
                towerLevelImage.sprite = level0;
                break;
            case 1:
                towerLevelImage.sprite = level1;
                break;
            case 2:
                towerLevelImage.sprite = level2;
                break;
            case 3:
                towerLevelImage.sprite = level3;
                break;
            case 4:
                towerLevelImage.sprite = level4;
                break;
            default:
                towerLevelImage.sprite = level0;
                break;
        }

        // Get tower type and set image
        towerTypeText.text = _data.towerType.ToString();
        switch(_data.towerType)
        {
            case "Basic Tower":
                towerTypeImage.sprite = middleRange;
                break;
            case "Short Range Tower":
                towerTypeImage.sprite = shortRange;
                break;
            case "Middle Range Tower":
                towerTypeImage.sprite = middleRange;
                break;
            case "Long Range Tower":
                towerTypeImage.sprite = longRange;
                break;
            case "Enemy HQ":
                towerTypeImage.sprite = level4;
                break;
            default:
                towerTypeImage.sprite = middleRange;
                break;
        }

        // Display tower position from HQ
        Vector3 _delta = _tower.transform.position - PlayManager.hq.transform.position;
        string _posText = ((_delta.z >= 0f) ? "North: " : "South: ") + Mathf.Abs(_delta.z) + " / " + ((_delta.x >= 0f) ? "East: " : "West: ") + Mathf.Abs(_delta.x);
        positionFromHQText.text = _posText;

        // Display tower data
        shortAtkText.text = _data.shortRangeAttack.ToString();
        middleAtkText.text = _data.middleRangeAttack.ToString();
        longAtkText.text = _data.longRangeAttack.ToString();
        exploAtkText.text = _data.explosiveAttack.ToString();

        shortDefText.text = _data.shortRangeDefense.ToString();
        middleDefText.text = _data.middleRangeDefense.ToString();
        longDefText.text = _data.longRangeDefense.ToString();
        exploDefText.text = _data.explosiveDefense.ToString();
    }
}
