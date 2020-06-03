using UnityEngine;

/// <summary>
/// Soldier is the class that represents the soldier with all its data
/// </summary>
public class Soldier : ScriptableObject
{
    // iD number that identify the soldier 
    private int iD;

    // Soldier name
    private string soldierName;

    // Soldier current HP
    private int currentHP;

    // Soldier current XP
    private int currentXP;

    // Image path (name - for data save) and sprite
    private string imagePath;
    private Sprite image;
    
    // Data path (name - for data save) and SoldierData
    private string dataPath;
    private SoldierData data;

    // State boolean (engaged / dead)
    private bool isEngaged = false;
    private bool isDead = false;

    // Bonuses that can be applied to the soldier
    private int[] attackBonuses = new int[3];
    private int[] defenseBonuses = new int[4];
    private int speedBonus;

    // private ??? friendships; => to be implemented

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

    public int MaxXP
    {
        get { return PlayManager.data.experienceMaxAmount[data.soldierLevel]; }
    }

    public int CurrentHP
    {
        set { currentHP = value; }
        get { return currentHP; }
    }

    public int CurrentXP
    {
        set { currentHP = value; }
        get { return currentXP; }
    }

    public int ShortRangeAttack
    {
        get { return data.shortRangeAttack + BonusAtkShortRange; }
    }

    public int MiddleRangeAttack
    {
        get { return data.middleRangeAttack + BonusAtkMidRange; }
    }

    public int LongRangeAttack
    {
        get { return data.longRangeAttack + BonusAtkLongRange; }
    }

    public int ShortRangeDefense
    {
        get { return data.shortRangeDefense + BonusDefShortRange; }
    }

    public int MiddleRangeDefense
    {
        get { return data.middleRangeDefense + BonusDefMidRange; }
    }

    public int LongRangeDefense
    {
        get { return data.longRangeDefense + BonusDefLongRange; }
    }

    public int ExplosivesDefense
    {
        get { return data.explosiveDefense + BonusDefExplosives; }
    }

    public int Speed
    {
        get { return data.speed + BonusSpeed; }
    }

    public int BonusAtkShortRange
    {
        set { attackBonuses[0] = value; }
        get { return attackBonuses[0]; }
    }

    public int BonusAtkMidRange
    {
        set { attackBonuses[1] = value; }
        get { return attackBonuses[1]; }
    }

    public int BonusAtkLongRange
    {
        set { attackBonuses[2] = value; }
        get { return attackBonuses[2]; }
    }
    public int BonusDefShortRange
    {
        set { defenseBonuses[0] = value; }
        get { return defenseBonuses[0]; }
    }

    public int BonusDefMidRange
    {
        set { defenseBonuses[1] = value; }
        get { return defenseBonuses[1]; }
    }

    public int BonusDefLongRange
    {
        set { defenseBonuses[2] = value; }
        get { return defenseBonuses[2]; }
    }
    public int BonusDefExplosives
    {
        set { defenseBonuses[3] = value; }
        get { return defenseBonuses[3]; }
    }

    public int BonusSpeed
    {
        set { speedBonus = value; }
        get { return speedBonus; }
    }

    #endregion

    /// <summary>
    /// Soldier constructor from a SoldierData
    /// </summary>
    /// <param name="_newData">SoldierData (type)</param>
    public Soldier(SoldierData _newData)
    {
        // Gets values from PlayManager
        iD = PlayManager.GetFreeSoldierID();
        soldierName = PlayManager.GetRandomSoldierName();
        image = PlayManager.GetRandomSoldierImage();
        imagePath = image.name;

        // Sets values from SoldierData
        data = _newData;
        dataPath = data.name;
        currentHP = data.maxHP;

        // Sets default value
        currentXP = 0;
    }

    /// <summary>
    /// Soldier constructor from dedicated data (from save file)
    /// </summary>
    /// <param name="_ID">iD number</param>
    /// <param name="_NAME">Name</param>
    /// <param name="_IMAGE">Image name</param>
    /// <param name="_DATA">Data name</param>
    /// <param name="_HP">HP current value</param>
    /// <param name="_XP">XP current value</param>
    public Soldier(int _ID, string _NAME, string _IMAGE, string _DATA, int _HP, int _XP)
    {
        iD = _ID;
        soldierName = _NAME;
        imagePath = _IMAGE;
        image = PlayManager.data.soldierImages.Find(x => x.name.Equals(imagePath));
        dataPath = _DATA;
        data = Resources.Load("SoldierData/" + dataPath) as SoldierData;
        currentHP = _HP;
        currentXP = _XP;
    }

    /// <summary>
    /// Soldier empty constructor
    /// </summary>
    public Soldier()
    {
        iD = 0;
        soldierName = "";
        imagePath = "";
        image = null;
        dataPath = "";
        data = null;
        currentHP = 0;
        currentXP = 0;
    }

    /// <summary>
    /// Evolve method changes the SoldierData
    /// </summary>
    /// <param name="_newData"></param>
    public void Evolve(SoldierData _newData)
    {
        data = _newData;
        dataPath = data.name;
        if (currentHP > MaxHP) currentHP = MaxHP;
    }

    /// <summary>
    /// Rename method changes the Soldier name
    /// </summary>
    /// <param name="_newName"></param>
    public void Rename(string _newName)
    {
        soldierName = _newName;
    }

    /// <summary>
    /// IsEngaged method retruns the isEngaged boolean value
    /// </summary>
    /// <returns>Boolean isEngaged</returns>
    public bool IsEngaged()
    {
        return isEngaged;
    }

    /// <summary>
    /// IsDead method retruns the isDead boolean value
    /// </summary>
    /// <returns>Boolean isDead</returns>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// Engage method sets the isEngaged boolean
    /// </summary>
    /// <param name="engage">Boolean to set isEngaged to</param>
    public void Engage(bool engage)
    {
        isEngaged = engage;
    }

    /// <summary>
    /// Heal method adds HP to current HP
    /// </summary>
    /// <param name="hp">Heal value</param>
    public void Heal(int hp)
    {
        currentHP += hp;
        if (currentHP > MaxHP) currentHP = MaxHP;
    }

    /// <summary>
    /// Die method sets the Soldier as dead
    /// </summary>
    public void Die()
    {
        isDead = true;
    }
}
