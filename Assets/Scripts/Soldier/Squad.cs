using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class Squad : MonoBehaviour
{
    public enum PreferedRange
    {
        ShortRange,
        MiddleRange,
        LongRange
    }

    public enum PositionChoice
    {
        MaximizeAttack,
        MaximizeDefense,
        PlayerChoice
    }

    public bool isEngaged = false;

    private int iD;
    [SerializeField]
    private Color color;
    [SerializeField]
    private SquadData squadType;
    [SerializeField]
    private Soldier[] soldierList = new Soldier[4];
    [SerializeField]
    private PreferedRange range;
    [SerializeField]
    private PositionChoice posChoice;


    #region Properties access
    public int ID
    {
        get { return iD; }
    }

    public Color Color
    {
        get { return color; }
    }

    public SquadData SquadType
    {
        get { return squadType; }
    }

    public PositionChoice PosChoice
    {
        set { posChoice = value; }
        get { return posChoice; }
    }

    public PreferedRange PrefRange
    {
        get { return range; }
    }

    public Soldier[] Soldiers
    {
        get { return soldierList; }
    }
    #endregion

    public void Setup()
    {
        iD = PlayManager.GetFreeSquadID();
        color = PlayManager.GetSquadColor(iD);
        squadType = PlayManager.defaultSquadType;
        posChoice = PositionChoice.MaximizeAttack;
        range = PreferedRange.MiddleRange;
    }

    public void ChangeSquadType(SquadData _newType)
    {
        squadType = _newType;
    }

    public SquadUnit InstanciateSquadUnit(Vector3 spawPoint)
    {
        GameObject _go = Instantiate(PlayManager.SquadPrefab, GridAdjustment.GetGridCoordinates(spawPoint), Quaternion.identity);
        SquadUnit _su = _go.GetComponent<SquadUnit>();
        _su.Setup(this);
        FindObjectOfType<CityCanvas>().AddSquad(this, _su);
        return _su;
    }

    public void UpdatePosChoice(PositionChoice _newChoice)
    {
        posChoice = _newChoice;
        switch(posChoice)
        {
            case PositionChoice.MaximizeAttack:
                range = GetMaxAttackRange();
                break;
            case PositionChoice.MaximizeDefense:
                range = GetMaxDefenseRange();
                break;
            case PositionChoice.PlayerChoice:
                break;
            default:
                break;
        }
    }

    private PreferedRange GetMaxAttackRange()
    {
        if (soldierList[0] == null || soldierList[1] == null || soldierList[2] == null || soldierList[3] == null) return PreferedRange.MiddleRange;

        int _shortRange = soldierList[0].ShortRangeAttack + soldierList[1].ShortRangeAttack + soldierList[2].ShortRangeAttack + soldierList[3].ShortRangeAttack;
        int _middleRange = soldierList[0].MiddleRangeAttack + soldierList[1].MiddleRangeAttack + soldierList[2].MiddleRangeAttack + soldierList[3].MiddleRangeAttack;
        int _longRange = soldierList[0].LongRangeAttack + soldierList[1].LongRangeAttack + soldierList[2].LongRangeAttack + soldierList[3].LongRangeAttack;

        if (_longRange >= _middleRange && _longRange >= _shortRange) return PreferedRange.LongRange;
        else if (_middleRange >= _shortRange) return PreferedRange.MiddleRange;
        else return PreferedRange.ShortRange;
    }

    private PreferedRange GetMaxDefenseRange()
    {
        if (soldierList[0] == null || soldierList[1] == null || soldierList[2] == null || soldierList[3] == null) return PreferedRange.LongRange;

        int _shortRange = soldierList[0].ShortRangeDefense + soldierList[1].ShortRangeDefense + soldierList[2].ShortRangeDefense + soldierList[3].ShortRangeDefense;
        int _middleRange = soldierList[0].MiddleRangeDefense + soldierList[1].MiddleRangeDefense + soldierList[2].MiddleRangeDefense + soldierList[3].MiddleRangeDefense;
        int _longRange = soldierList[0].LongRangeDefense + soldierList[1].LongRangeDefense + soldierList[2].LongRangeDefense + soldierList[3].LongRangeDefense;

        if (_longRange >= _middleRange && _longRange >= _shortRange) return PreferedRange.LongRange;
        else if (_middleRange >= _shortRange) return PreferedRange.MiddleRange;
        else return PreferedRange.ShortRange;
    }
}
