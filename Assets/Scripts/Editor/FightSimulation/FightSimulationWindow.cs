using UnityEngine;
using UnityEditor;
using System;
using Boo.Lang;

public class FightSimulationWindow : EditorWindow
{
    float _space = 20f;
    Vector2 scrollPosition;

    List<SquadSimulator> squadsList = new List<SquadSimulator>();
    List<TowerSimulator> towersList = new List<TowerSimulator>();
    List<EnemySimulator> enemyList = new List<EnemySimulator>();

    List<string> results = new List<string>();

    int totalDayTime = 45;
    int initialTravelTime = 10;
    int towerTravelTime = 0;
    int healAmount = 15;

    bool sortByDef = true;
    bool sortByHP = true;

    GUIStyle redText = new GUIStyle();
    GUIStyle greenText = new GUIStyle();
    GUIStyle yellowText = new GUIStyle();

    // Add menu named "My Window" to the Window menu
    [MenuItem("TowerOffense/Fight Simulation Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FightSimulationWindow window = (FightSimulationWindow)EditorWindow.GetWindow(typeof(FightSimulationWindow));
        window.titleContent.text = "Fighting Simulator";

        window.redText.normal.textColor = Color.red;
        window.greenText.normal.textColor = Color.green;
        window.yellowText.normal.textColor = Color.yellow;


        window.AddSquad();
        window.AddTower();

        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Squads Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(300f));
        foreach (SquadSimulator _squad in squadsList)
        {
            DoSquad(_squad);
        }

        if(squadsList.Count < 4)
        {
            GUILayout.Space(5f);
            if (GUILayout.Button("Add Squad", GUILayout.MaxWidth(300f)))
            {
                AddSquad();
            }
        }        
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Towers Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(300f));
        foreach (TowerSimulator _tower in towersList)
        {
            DoTower(_tower);
        }

        GUILayout.Space(5f);
        if (GUILayout.Button("Add Tower"))
        {
            AddTower();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(_space);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("City Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(300f));
        totalDayTime = int.Parse(EditorGUILayout.TextField("Total Day Time:", totalDayTime.ToString(), GUILayout.MaxWidth(180f)));
        initialTravelTime = int.Parse(EditorGUILayout.TextField("Initial Travel Time:", initialTravelTime.ToString(), GUILayout.MaxWidth(180f)));
        towerTravelTime = int.Parse(EditorGUILayout.TextField("Tower Travel Time:", towerTravelTime.ToString(), GUILayout.MaxWidth(180f)));
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("HQ Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(300f));
        healAmount = int.Parse(EditorGUILayout.TextField("Heal Amount At Day End:", healAmount.ToString(), GUILayout.MaxWidth(180f)));
        sortByDef = EditorGUILayout.Toggle("Sort soldiers by defense:", sortByDef, GUILayout.MaxWidth(180f));
        sortByHP = EditorGUILayout.Toggle("Sort soldiers by HP:", sortByHP, GUILayout.MaxWidth(180f));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(_space);

        if (GUILayout.Button("Compute run"))
        {
            ComputeRun();
        }
        DoResults();

    }

    private void AddSquad()
    {
        SquadSimulator _squad = new SquadSimulator();
        _squad.squadName = "Squad" + (squadsList.Count + 1).ToString();
        _squad.soldiers[0] = new SoldierSimulator();
        _squad.soldiers[0].data = Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData;
        _squad.soldiers[1] = new SoldierSimulator();
        _squad.soldiers[1].data = Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData;
        _squad.soldiers[2] = new SoldierSimulator();
        _squad.soldiers[2].data = Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData;
        _squad.soldiers[3] = new SoldierSimulator();
        _squad.soldiers[3].data = Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData;
        squadsList.Add(_squad);
    }

    private void RemoveSquad(SquadSimulator _squad)
    {
        List<SquadSimulator> _list = new List<SquadSimulator>(squadsList);
        _list.Remove(_squad);
        foreach(SquadSimulator _s in _list)
        {
            _s.squadName = "Squad" + (_list.IndexOf(_s) + 1).ToString();
        }
        squadsList = new List<SquadSimulator>(_list);
    }

    private void DoSquad(SquadSimulator _squad)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_squad.squadName, GUILayout.MaxWidth(277f));
        if(GUILayout.Button("-", GUILayout.MaxWidth(20f)))
        {
            RemoveSquad(_squad);
        }
        EditorGUILayout.EndHorizontal();
        _squad.soldiers[0].data = EditorGUILayout.ObjectField(_squad.soldiers[0].data, typeof(SoldierData), false, GUILayout.MaxWidth(300f)) as SoldierData;
        _squad.soldiers[1].data = EditorGUILayout.ObjectField(_squad.soldiers[1].data, typeof(SoldierData), false, GUILayout.MaxWidth(300f)) as SoldierData;
        _squad.soldiers[2].data = EditorGUILayout.ObjectField(_squad.soldiers[2].data, typeof(SoldierData), false, GUILayout.MaxWidth(300f)) as SoldierData;
        _squad.soldiers[3].data = EditorGUILayout.ObjectField(_squad.soldiers[3].data, typeof(SoldierData), false, GUILayout.MaxWidth(300f)) as SoldierData;
    }

    private void AddTower()
    {
        TowerSimulator _tower = new TowerSimulator();
        _tower.towerName = "Tower" + (towersList.Count + 1).ToString();
        _tower.data = Resources.Load("TowerData/0_BasicTower") as TowerData;
        towersList.Add(_tower);
    }

    private void RemoveTower(TowerSimulator _tower)
    {
        List<TowerSimulator> _list = new List<TowerSimulator>(towersList);
        _list.Remove(_tower);
        foreach (TowerSimulator _t in _list)
        {
            _t.towerName = "Tower" + (_list.IndexOf(_t) + 1).ToString();
        }
        towersList = new List<TowerSimulator>(_list);
    }

    private void DoTower(TowerSimulator _tower)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_tower.towerName, GUILayout.MaxWidth(50f));
        _tower.data = EditorGUILayout.ObjectField(_tower.data, typeof(TowerData), false) as TowerData;
        if (GUILayout.Button("-", GUILayout.MaxWidth(20f)))
        {
            RemoveTower(_tower);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DoResults()
    {
        if (results.Count > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(600));
            GUILayout.BeginVertical();
            for(int i=0;i < results.Count;i++)
            {
                if(results[i].StartsWith("*"))
                {
                    EditorGUILayout.LabelField(results[i].Substring(1), yellowText, GUILayout.MaxWidth(600f));
                }
                else if(results[i].StartsWith("-"))
                {
                    EditorGUILayout.LabelField(results[i].Substring(1), redText, GUILayout.MaxWidth(600f));
                }
                else if(results[i].StartsWith("+"))
                {
                    EditorGUILayout.LabelField(results[i].Substring(1), greenText, GUILayout.MaxWidth(600f));
                }
                else
                {
                    EditorGUILayout.LabelField(results[i], GUILayout.MaxWidth(600f));
                }                
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }

    private void ComputeRun()
    {
        results.Clear();
        if (squadsList.Count == 0)
        {
            results.Add("-There must be at least on squad!");
            return;
        }

        if (towersList.Count == 0)
        {
            results.Add("-There must be at least on tower!");
            return;
        }

        foreach(SquadSimulator _s in squadsList)
        {
            if(_s.soldiers[0].data == null || _s.soldiers[1].data == null || _s.soldiers[2].data == null || _s.soldiers[3].data == null )
            {
                results.Add("-All squads must be full (4 soldiers data)!");
                return;
            }
        }

        int globalCounter = 0;
        int dayCount = 1;

        bool allTowersDestroyed = false;
        bool allSoldiersDead = false;

        foreach (SquadSimulator _s in squadsList)
        {
            _s.soldiers[0].soldierHP = _s.soldiers[0].data.maxHP;
            _s.soldiers[1].soldierHP = _s.soldiers[1].data.maxHP;
            _s.soldiers[2].soldierHP = _s.soldiers[2].data.maxHP;
            _s.soldiers[3].soldierHP = _s.soldiers[3].data.maxHP;
        }

        foreach (TowerSimulator _t in towersList)
        {
            _t.towerHP = _t.data.maxHP;
        }

        enemyList.Clear();
        SortSoldiers(dayCount);

        while (!allSoldiersDead && !allTowersDestroyed)
        {
            globalCounter++;
            if(globalCounter >= totalDayTime + initialTravelTime)
            {
                results.Add("*Day " + dayCount + ": " + globalCounter + "s - Squads are back in HQ");
                // End of day => heal and reorganize soldiers
                // Reset globalCounter and increment dayCount
                globalCounter = 0;
                dayCount++;
                foreach(SquadSimulator _s in squadsList)
                {
                    foreach(SoldierSimulator _soldier in _s.soldiers)
                    {
                        _soldier.soldierHP += healAmount;
                        if (_soldier.soldierHP > _soldier.data.maxHP) _soldier.soldierHP = _soldier.data.maxHP;
                    }
                }

                enemyList.Clear();
                SortSoldiers(dayCount);

            }
            else if(globalCounter >= totalDayTime)
            {
                if (globalCounter == totalDayTime)
                {
                    results.Add("*Day " + dayCount + ": " + globalCounter + "s - Retreat");
                }

                // Retreat => No more attack on or from towers, only from/to enemy soldiers
                // Attack from soldiers
                foreach (SquadSimulator _s in squadsList)
                {
                    for(int i = 0; i < _s.soldiers.Length; i++)
                    {
                        if (_s.soldiers[i].soldierHP > 0 && ((globalCounter - initialTravelTime) % (int)_s.soldiers[i].data.shootingDelay) == 0)
                        {
                            int enemyIndex = 0;
                            foreach (EnemySimulator _e in enemyList)
                            {
                                if (_e.enemyHP > 0) break;
                                enemyIndex++;
                            }
                            if (enemyIndex < enemyList.Count)
                            {
                                EnemySimulator chosenEnemy = enemyList[enemyIndex];
                                int damage = Mathf.Max(_s.soldiers[i].data.shortRangeAttack - chosenEnemy.defense,
                                    _s.soldiers[i].data.middleRangeAttack - chosenEnemy.defense,
                                    _s.soldiers[i].data.longRangeAttack, 0);
                                chosenEnemy.enemyHP -= damage;
                                results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _s.squadName + " Soldier" + (i+1) +" deals " + damage + " damage(s) to " + chosenEnemy.enemyName + " (HP: " + chosenEnemy.enemyHP + ")");
                                if (chosenEnemy.enemyHP <= 0) results.Add("+Day " + dayCount + ": " + globalCounter + "s - " + chosenEnemy.enemyName + " is dead");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                // Attack from enemy soldiers
                foreach (EnemySimulator _e in enemyList)
                {
                    if (_e.enemyHP > 0 && ((globalCounter - initialTravelTime) % 3) == 0)
                    {
                        int squadIndex = 0;
                        int soldierIndex = 0;
                        foreach (SquadSimulator _s in squadsList)
                        {
                            for( soldierIndex=0; soldierIndex < _s.soldiers.Length; soldierIndex++)
                            {
                                if (_s.soldiers[soldierIndex].soldierHP > 0) break;
                            }

                            if(soldierIndex < _s.soldiers.Length)
                            {
                                break;
                            }
                            else
                            {
                                squadIndex++;
                            }                            
                        }
                        if (squadIndex < squadsList.Count)
                        {
                            SquadSimulator chosenSquad = squadsList[squadIndex];
                            int _srd = chosenSquad.soldiers[soldierIndex].data.shortRangeDefense;
                            int _mrd = chosenSquad.soldiers[soldierIndex].data.middleRangeDefense;
                            int _lrd = chosenSquad.soldiers[soldierIndex].data.longRangeDefense;

                            int damage = Mathf.Max(_e.attack - _srd, _e.attack - _mrd, 0);

                            chosenSquad.soldiers[soldierIndex].soldierHP -= damage;
                            if (chosenSquad.soldiers[soldierIndex].soldierHP < 0) chosenSquad.soldiers[soldierIndex].soldierHP = 0;
                            results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _e.enemyName + " deals " + damage + " damage(s) to " + chosenSquad.squadName + " - Soldier" + (soldierIndex +1) + " (HP: " + chosenSquad.soldiers[soldierIndex].soldierHP + ")");
                            if (chosenSquad.soldiers[soldierIndex].soldierHP <= 0)
                            {
                                results.Add("Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - Soldier" + (soldierIndex + 1) + " is wounded");
                                if (soldierIndex == 4) results.Add("-Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - All soldiers of squad are dead!");
                            }
                        }
                        else
                        {
                            allSoldiersDead = true;
                            break;
                        }
                    }
                }
            }
            else if(globalCounter >= initialTravelTime)
            {
                // Attack from soldiers
                foreach (SquadSimulator _s in squadsList)
                {
                    for(int i=0; i < _s.soldiers.Length; i++)
                    {
                        if (_s.soldiers[i].soldierHP > 0 && ((globalCounter - initialTravelTime) % (int)_s.soldiers[i].data.shootingDelay) == 0)
                        {
                            int towerIndex = 0;
                            foreach (TowerSimulator _t in towersList)
                            {
                                if (_t.towerHP > 0) break;
                                towerIndex++;
                            }
                            if (towerIndex < towersList.Count)
                            {
                                TowerSimulator chosenTower = towersList[towerIndex];
                                int damage = Mathf.Max(_s.soldiers[i].data.shortRangeAttack - chosenTower.data.shortRangeDefense,
                                    _s.soldiers[i].data.middleRangeAttack - chosenTower.data.middleRangeDefense,
                                    _s.soldiers[i].data.longRangeAttack - chosenTower.data.longRangeDefense, 0);
                                chosenTower.towerHP -= damage;
                                results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _s.squadName + " Soldier" + (i+1) + " deals " + damage + " damage(s) to " + chosenTower.towerName + " (HP: " + chosenTower.towerHP + ")");
                                if(chosenTower.towerHP <= 0) results.Add("+Day " + dayCount + ": " + globalCounter + "s - " + chosenTower.towerName + " is destroyed");
                            }
                            else
                            {
                                allTowersDestroyed = true;
                                break;
                            }
                        }
                    }
                }

                // Attack from towers
                foreach(TowerSimulator _t in towersList)
                {
                    if (_t.towerHP > 0 && ((globalCounter - initialTravelTime) % (int)_t.data.shootingDelay) == 0)
                    {
                        int squadIndex = 0;
                        int soldierIndex = 0;
                        foreach (SquadSimulator _s in squadsList)
                        {
                            for (soldierIndex = 0; soldierIndex < _s.soldiers.Length; soldierIndex++)
                            {
                                if (_s.soldiers[soldierIndex].soldierHP > 0) break;
                            }

                            if (soldierIndex < _s.soldiers.Length)
                            {
                                break;
                            }
                            else
                            {
                                squadIndex++;
                            }
                        }
                        if (squadIndex < squadsList.Count)
                        {
                            SquadSimulator chosenSquad = squadsList[squadIndex];
                            int _srd = chosenSquad.soldiers[soldierIndex].data.shortRangeDefense;
                            int _mrd = chosenSquad.soldiers[soldierIndex].data.middleRangeDefense;
                            int _lrd = chosenSquad.soldiers[soldierIndex].data.longRangeDefense;

                            int damage = Mathf.Max(_t.data.shortRangeAttack - _srd,
                                _t.data.middleRangeAttack - _mrd,
                                _t.data.longRangeAttack - _lrd, 0);

                            chosenSquad.soldiers[soldierIndex].soldierHP -= damage;
                            if (chosenSquad.soldiers[soldierIndex].soldierHP < 0) chosenSquad.soldiers[soldierIndex].soldierHP = 0;
                            results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _t.towerName + " deals " + damage + " damage(s) to " + chosenSquad.squadName + " - Soldier" + (soldierIndex +1) + " (HP: " + chosenSquad.soldiers[soldierIndex].soldierHP + ")");
                            if (chosenSquad.soldiers[soldierIndex].soldierHP <= 0)
                            {
                                results.Add("-Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - Soldier" + (soldierIndex + 1) + " is wounded");
                                if(soldierIndex == 4) results.Add("-Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - All soldiers of squad are dead!");
                            }
                        }
                        else
                        {
                            allSoldiersDead = true;
                            break;
                        }
                    }
                }

                // Spawn from towers
                foreach (TowerSimulator _t in towersList)
                {
                    if (_t.towerHP > 0 && _t.data.spawnSoldier && ((globalCounter - initialTravelTime) % (int)_t.data.spawnDelay) == 0)
                    {
                        EnemySimulator _enemy = new EnemySimulator();
                        _enemy.enemyName = "EnemySoldierFrom" + _t.towerName;
                        _enemy.enemyHP = _t.data.maxHP / 5;
                        _enemy.attack = Mathf.FloorToInt(Mathf.Max(1, Mathf.Max(_t.data.shortRangeAttack, _t.data.middleRangeAttack, _t.data.longRangeAttack) / 10));
                        _enemy.defense = Mathf.FloorToInt(Mathf.Max(0, (Mathf.Max(_t.data.shortRangeDefense, _t.data.middleRangeDefense, _t.data.longRangeDefense) / 2)));
                        enemyList.Add(_enemy);
                        results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _t.towerName + " has spaw a new enemy soldier");
                    }
                }

                // Attack from enemy soldiers
                foreach (EnemySimulator _e in enemyList)
                {
                    if (_e.enemyHP > 0 && ((globalCounter - initialTravelTime) % 3) == 0)
                    {
                        int squadIndex = 0;
                        int soldierIndex = 0;
                        foreach (SquadSimulator _s in squadsList)
                        {
                            for (soldierIndex = 0; soldierIndex < _s.soldiers.Length; soldierIndex++)
                            {
                                if (_s.soldiers[soldierIndex].soldierHP > 0) break;
                            }

                            if (soldierIndex < _s.soldiers.Length)
                            {
                                break;
                            }
                            else
                            {
                                squadIndex++;
                            }
                        }
                        if (squadIndex < squadsList.Count)
                        {
                            SquadSimulator chosenSquad = squadsList[squadIndex];
                            int _srd = chosenSquad.soldiers[soldierIndex].data.shortRangeDefense;
                            int _mrd = chosenSquad.soldiers[soldierIndex].data.middleRangeDefense;
                            int _lrd = chosenSquad.soldiers[soldierIndex].data.longRangeDefense;

                            int damage = Mathf.Max(_e.attack - _srd, _e.attack - _mrd, 0);

                            chosenSquad.soldiers[soldierIndex].soldierHP -= damage;
                            if (chosenSquad.soldiers[soldierIndex].soldierHP < 0) chosenSquad.soldiers[soldierIndex].soldierHP = 0;
                            results.Add("Day " + dayCount + ": " + globalCounter + "s - " + _e.enemyName + " deals " + damage + " damage(s) to " + chosenSquad.squadName + " - Soldier" + (soldierIndex + 1) + " (HP: " + chosenSquad.soldiers[soldierIndex].soldierHP + ")");
                            if (chosenSquad.soldiers[soldierIndex].soldierHP <= 0)
                            {
                                results.Add("-Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - Soldier" + (soldierIndex + 1) + " is wounded");
                                if (soldierIndex == 4) results.Add("-Day " + dayCount + ": " + globalCounter + "s - " + chosenSquad.squadName + " - All soldiers of squad are dead!");
                            }
                        }
                        else
                        {
                            allSoldiersDead = true;
                            break;
                        }
                    }
                }
            }

            // Check if all soldiers are dead
            if (allSoldiersDead) results.Add("-Day " + dayCount + ": " + globalCounter + "s - All soldiers are dead!");

            // Check if all towers are destroyed
            if (allTowersDestroyed) results.Add("+Day " + dayCount + ": " + globalCounter + "s - All towers are destroyed!");
        }
    }

    private void SortSoldiers(int _day)
    {
        foreach (SquadSimulator _s in squadsList)
        {
            if (sortByDef && sortByHP)
            {
                Array.Sort(_s.soldiers, SoldierSimulator.SortByDefThenHP);
            }
            else if (sortByDef)
            {
                Array.Sort(_s.soldiers, SoldierSimulator.SortByDef);
            }
            else if (sortByHP)
            {
                Array.Sort(_s.soldiers, SoldierSimulator.SortByHP);
            }

            for (int i = 0; i < _s.soldiers.Length; i++)
            {
                results.Add("*Day " + _day + ": " + _s.squadName + " - Soldier" + (i+1) + " type is " + _s.soldiers[i].data.name + " (HP: " + _s.soldiers[i].soldierHP + ")");
            }
        }
    }
}

[Serializable]
public class TowerSimulator
{
    public string towerName = "";
    public TowerData data;
    public int towerHP;
}

[Serializable]
public class SquadSimulator
{
    public string squadName = "";
    public SoldierSimulator[] soldiers = new SoldierSimulator[4];
}

[Serializable]
public class SoldierSimulator
{
    public SoldierData data;
    public int soldierHP;

    /// <summary>
    /// SortByHP method compares two soldiers and returns an int depending of soldiers'HP
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByHP(SoldierSimulator x, SoldierSimulator y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return y.soldierHP.CompareTo(x.soldierHP);
    }

    /// <summary>
    /// SortByDef method compares two soldiers and returns an int depending of soldiers'defense (max of ranges)
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByDef(SoldierSimulator x, SoldierSimulator y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return Mathf.Max(y.data.shortRangeDefense,y.data.middleRangeDefense, y.data.longRangeDefense).CompareTo(Mathf.Max(x.data.shortRangeDefense, x.data.middleRangeDefense, x.data.longRangeDefense));
    }

    /// <summary>
    /// SortByDefThenHP method compares two soldiers and returns an int depending of soldiers'defense (max of ranges) and soldiers'HP
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByDefThenHP(SoldierSimulator x, SoldierSimulator y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        switch (Mathf.Max(y.data.shortRangeDefense, y.data.middleRangeDefense, y.data.longRangeDefense).CompareTo(Mathf.Max(x.data.shortRangeDefense, x.data.middleRangeDefense, x.data.longRangeDefense)))
        {
            case -1:
                return -1;
            case 1:
                return 1;
            case 0:
                return y.soldierHP.CompareTo(x.soldierHP);
            default:
                return 0;
        }
    }
}

[Serializable]
public class EnemySimulator
{
    public string enemyName = "";
    public int enemyHP;
    public int attack;
    public int defense;
}