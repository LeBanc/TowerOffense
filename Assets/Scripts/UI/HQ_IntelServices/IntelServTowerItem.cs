using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// IntelServTowerItem class is for a Tower item of the Intelligence Services Canvas
/// </summary>
public class IntelServTowerItem : MonoBehaviour
{
    // public UI elements
    public Text towerLevelText;
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

    /// <summary>
    /// Setup method sets all the text field of the IntelServTowerItem
    /// </summary>
    /// <param name="_tower">Tower from which get the data (Tower)</param>
    public void Setup(Tower _tower)
    {
        TowerData _data = _tower.data;

        towerLevelText.text = _data.towerLevel.ToString();
        towerTypeText.text = _data.towerType.ToString();

        Vector3 _delta = _tower.transform.position - PlayManager.hq.transform.position;
        string _posText = ((_delta.z >= 0f) ? "North: " : "South: ") + Mathf.Abs(_delta.z) + " / " + ((_delta.x >= 0f) ? "East: " : "West: ") + Mathf.Abs(_delta.x);
        positionFromHQText.text = _posText;

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
