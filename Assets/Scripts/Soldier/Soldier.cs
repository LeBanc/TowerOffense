using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Soldier : MonoBehaviour
{
    [SerializeField]
    private int iD;
    [SerializeField]
    private string soldierName;
    [SerializeField]
    private Sprite image;
    [SerializeField]
    private SoldierData data;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private int currentXP;

    private bool isEngaged = false;
    private bool isDead = false;

    // private ??? friendships;

    #region Properties access
    public int ID
    {
        get { return iD; }
    }

    public string Name
    {
        get { return soldierName; }
    }

    public Sprite Image
    {
        get { return image; }
    }

    public SoldierData Data
    {
        get { return data; }
    }

    public int MaxHP
    {
        get { return data.maxHP; }
    }

    public int CurrentHP
    {
        set { currentHP = value; }
        get { return currentHP; }
    }

    public int CurrentXP
    {
        set { currentHP = value; }
        get { return currentHP; }
    }

    public int ShortRangeAttack
    {
        get { return data.shortRangeAttack; }
    }

    public int MiddleRangeAttack
    {
        get { return data.middleRangeAttack; }
    }

    public int LongRangeAttack
    {
        get { return data.longRangeAttack; }
    }

    public int ShortRangeDefense
    {
        get { return data.shortRangeAttack; }
    }

    public int MiddleRangeDefense
    {
        get { return data.middleRangeAttack; }
    }

    public int LongRangeDefense
    {
        get { return data.longRangeAttack; }
    }

    #endregion

    public Soldier(SoldierData _newData)
    {
        iD = PlayManager.GetFreeSoldierID();
        soldierName = PlayManager.GetRandomSoldierName();
        image = PlayManager.GetRandomSoldierImage();
        data = _newData;
        currentHP = data.maxHP;
        currentXP = 0;
    }

    public void Evolve(SoldierData _newData)
    {
        data = _newData;
        if (currentHP > MaxHP) currentHP = MaxHP;
    }

    public void Rename(string _newName)
    {
        soldierName = _newName;
    }

    public bool IsEngaged()
    {
        return isEngaged;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Engage(bool engage)
    {
        isEngaged = engage;
    }

    public void Heal(int hp)
    {
        currentHP += hp;
        if (currentHP > MaxHP) currentHP = MaxHP;
    }

    public void Die()
    {
        isDead = true;
    }
}
