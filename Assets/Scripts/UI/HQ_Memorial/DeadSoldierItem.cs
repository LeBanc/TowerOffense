using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeadSoldierItem : MonoBehaviour
{
    public SoldierImage soldierImage;
    public Text soldierName;
    public Text dayOfDeath;
    public Image mourningImage;

    /// <summary>
    /// Setup methods sets the item to display soldier data and returns a bool if the mourning still affects a soldier
    /// </summary>
    /// <param name="_soldier">Dead soldier to display (Soldier)</param>
    /// <returns>True if mourning active, false otherwise (bool)</returns>
    public bool Setup(Soldier _soldier)
    {
        soldierImage.Setup(_soldier, true);
        soldierName.text = PlayManager.data.ranks[_soldier.Data.soldierLevel] + " " + _soldier.Name;
        dayOfDeath.text = _soldier.DayOfDeath.ToString();

        int _max = _soldier.Friendship.Values.Max();
        int _daysToMourn = 2;
        if (_max >= PlayManager.data.friendshipLevels[4].threshold)
        {
            _daysToMourn += 4;
        }
        else if (_max >= PlayManager.data.friendshipLevels[3].threshold)
        {
            _daysToMourn += 3;
        }
        else if (_max >= PlayManager.data.friendshipLevels[2].threshold)
        {
            _daysToMourn += 2;
        }
        else if (_max >= PlayManager.data.friendshipLevels[1].threshold)
        {
            _daysToMourn += 1;
        }

        // Check if mourning is still active
        bool _returnValue = ((PlayManager.day - _soldier.DayOfDeath) <= _daysToMourn);
        mourningImage.enabled = _returnValue;
        return _returnValue;
    }
}
