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
                towerLevelImage.sprite = PlayManager.data.rankImages[0];
                break;
            case 1:
                towerLevelImage.sprite = PlayManager.data.rankImages[1];
                break;
            case 2:
                towerLevelImage.sprite = PlayManager.data.rankImages[2];
                break;
            case 3:
                towerLevelImage.sprite = PlayManager.data.rankImages[3];
                break;
            case 4:
                towerLevelImage.sprite = PlayManager.data.rankImages[4];
                break;
            default:
                towerLevelImage.sprite = PlayManager.data.rankImages[0];
                break;
        }

        // Get tower type and set image
        towerTypeText.text = _data.towerType.ToString();
        towerTypeImage.sprite = _data.towerTypeSprite;

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
