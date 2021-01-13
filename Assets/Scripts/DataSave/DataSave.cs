using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System;

/// <summary>
/// DataSave defines the methods to load and save the game data
/// </summary>
[XmlRoot("DataSave")]
public class DataSave
{

    // Save file data (name & time)
    [XmlElement("SaveName")]
    public string saveName;
    [XmlElement("Date")]
    public DateTime date;

    // Global HQ data
    [XmlElement("Coins")]
    public int coins;
    [XmlElement("Day")]
    public int day;

    // Facilities data
    [XmlElement("Infirmary")]
    public int infirmary;
    [XmlElement("Radio")]
    public int radio;

    // HQ save data
    [XmlElement("HQ")]
    public HQSave hqSave;

    // Squads save data
    [XmlArray("Squads"), XmlArrayItem("SquadSave")]
    public List<SquadSave> squadList = new List<SquadSave>();

    // Soldiers save data
    [XmlArray("Soldiers"), XmlArrayItem("SoldierSave")]
    public List<SoldierSave> soldierList = new List<SoldierSave>();

    // Towers save data
    [XmlArray("Towers"), XmlArrayItem("TowerSave")]
    public List<TowerSave> towerList = new List<TowerSave>();

    // Turrets save data
    [XmlArray("Turrets"), XmlArrayItem("TurretSave")]
    public List<TurretSave> turretList = new List<TurretSave>();

    // Turret bases save data
    [XmlArray("TurretBases"), XmlArrayItem("TurretBaseSave")]
    public List<TurretBaseSave> turretBaseList = new List<TurretBaseSave>();

    // HQ Candidates save data
    [XmlArray("HQCandidates"), XmlArrayItem("HQCandidateSave")]
    public List<HQCandidateSave> hqCandidateList = new List<HQCandidateSave>();

    // Buildings save data
    [XmlArray("Buildings"), XmlArrayItem("BuildingSave")]
    public List<BuildingSave> buildingList = new List<BuildingSave>();

    /// <summary>
    /// Basic constructor of an empty SquadSave (needed to xml saves)
    /// </summary>
    public DataSave()
    {
        // All int are 0 and list are created as new empty lists so there is no need to do anything here
    }

    /// <summary>
    /// Save method inits the DataSave with the input name and the current game data
    /// </summary>
    /// <param name="_saveName">Name (string) of the save file</param>
    private void Save(string _saveName)
    {
        // Save Name & Time
        saveName = _saveName;
        date = DateTime.Now;

        // Save single data
        coins = PlayManager.coins;
        day = PlayManager.day;
        infirmary = PlayManager.infirmaryLevel;
        radio = PlayManager.radioLevel;

        hqSave = new HQSave(PlayManager.hq);

        // Save list data
        foreach (Squad _squad in PlayManager.squadList)
        {
            squadList.Add(new SquadSave(_squad));
        }

        foreach (Soldier _soldier in PlayManager.soldierList)
        {
            soldierList.Add(new SoldierSave(_soldier));
        }

        foreach (Tower _tower in PlayManager.towerList)
        {
            towerList.Add(new TowerSave(_tower));
        }

        foreach (Turret _turret in PlayManager.turretList)
        {
            turretList.Add(new TurretSave(_turret));
        }

        foreach (TurretBase _turretBase in PlayManager.turretBaseList)
        {
            turretBaseList.Add(new TurretBaseSave(_turretBase));
        }

        foreach (HQCandidate _candidate in PlayManager.hqCandidateList)
        {
            hqCandidateList.Add(new HQCandidateSave(_candidate));
        }

        foreach (GameObject _building in PlayManager.buildingList)
        {
            buildingList.Add(new BuildingSave(_building));
        }
    }

    /// <summary>
    /// SaveGame method creates a savefile with the _saveName parameter name
    /// </summary>
    /// <param name="_saveName">Name of the save file (string)</param>
    public void SaveGame(string _saveName)
    {
        // Create the DataSave
        Save(_saveName);
        _saveName = _saveName.Replace(' ', '_');

        // Saves the DataSave into a save file
        var serializer = new XmlSerializer(typeof(DataSave));
        using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, string.Concat(_saveName,".save")), FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    /// <summary>
    /// AutoSaveGame method creates a savefile named AutoSave.xml and archive the previous one if it exists
    /// </summary>
    public void AutoSaveGame()
    {
        // Create the DataSave
        Save("AutoSave");

        // Checks if a previous AutoSave file exists and renames it
        if (File.Exists(Path.Combine(Application.persistentDataPath, "AutoSave.save")))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "AutoSave1.save"));
            File.Move(Path.Combine(Application.persistentDataPath, "AutoSave.save"), Path.Combine(Application.persistentDataPath, "AutoSave1.save"));
        }

        // Saves the DataSave into the AutoSave.xml file
        var serializer = new XmlSerializer(typeof(DataSave));
        using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, "AutoSave.save"), FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    /// <summary>
    /// LoadAutoSavedGame method creates a DataSave from the AutoSave.xml file
    /// </summary>
    public static void LoadAutoSavedGame()
    {
        LoadSavedGame("AutoSave.save");
    }

    /// <summary>
    /// LoadSavedGame method creates a DataSave from the chosen file and loads it into the PlayManager
    /// </summary>
    /// <param name="_fileName">Save file to load the data from</param>
    public static void LoadSavedGame(string _fileName)
    {
        // Check if file exists
        if(File.Exists(Path.Combine(Application.persistentDataPath, _fileName)))
        {
            // DeSerialize the file into data and load them in the game
            var serializer = new XmlSerializer(typeof(DataSave));
            using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Open))
            {
                LoadData(serializer.Deserialize(stream) as DataSave);
            }
        }
        else
        {
            Debug.LogError("[DataSave] The SaveFile doesn't exists!");
        }        
    }

    /// <summary>
    /// GetFileData method returns a FileData struct from a save file
    /// </summary>
    /// <param name="_fileName">Name of the file (string)</param>
    /// <returns>FileData struct with usable file data if the file exists, ("",DateTime.MinValue,0) otherwise</returns>
    public static FileData GetFileData(string _fileName)
    {
        // Checks if file exists
        if (File.Exists(Path.Combine(Application.persistentDataPath, _fileName)))
        {
            // If it exists, returns a FileData from the file data
            var serializer = new XmlSerializer(typeof(DataSave));
            using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Open))
            {
                DataSave _data = serializer.Deserialize(stream) as DataSave;
                FileData _fileData = new FileData(_data.saveName, _data.date, _data.day);
                return _fileData;
            }
        }
        else
        {
            // If it doesn't exist, return an empty FileData
            Debug.LogError("[DataSave] The SaveFile doesn't exists!");
            FileData _fileData = new FileData("", DateTime.MinValue, 0);
            return _fileData;
        }
    }

    /// <summary>
    /// FileData struct is used to return the usable file data in one struct
    /// </summary>
    public struct FileData
    {
        public FileData(string name, DateTime dateTime, int dayCount)
        {
            SaveName = name;
            Date = dateTime;
            Day = dayCount;
        }

        // Name of the save file
        public string SaveName { get; }
        // Date & Time of the save file
        public DateTime Date { get; }
        // Days (game data) past
        public int Day { get; }
    }

    /// <summary>
    /// SortByDate method is used to sort FileData by their date/time
    /// </summary>
    /// <param name="x">First FileData to compare</param>
    /// <param name="y">Second FileData to compare</param>
    /// <returns>-1 if x<y, 0 if x==y and 1 if x>y</returns>
    public static int SortByDate(FileData x, FileData y)
    {
        if (x.Date == DateTime.MinValue)
        {
            if (y.Date == DateTime.MinValue) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y.Date == DateTime.MinValue) return -1; // y is null so x is lower than y

        return (-1)*x.Date.CompareTo(y.Date);
    }

    /// <summary>
    /// LoadSavedGame method loads a DataSave into the PlayManager
    /// </summary>
    /// <param name="_save">DataSave to load</param>
    public static void LoadData(DataSave _save)
    {
        // Global data
        PlayManager.coins = _save.coins;
        PlayManager.day = _save.day;
        PlayManager.infirmaryLevel = _save.infirmary;
        PlayManager.radioLevel = _save.radio;

        // Load Soldiers
        PlayManager.soldierList.Clear();
        PlayManager.soldierIDList.Clear();
        foreach (SoldierSave _soldierSave in _save.soldierList)
        {
            Soldier _soldier = _soldierSave.Load();
            PlayManager.soldierList.Add(_soldier);
            PlayManager.soldierIDList.Add(_soldier.ID);
        }
        PlayManager.nextSoldierID = PlayManager.soldierList.Count;

        // Load Squads
        PlayManager.squadList.Clear();
        foreach (SquadSave _squadSave in _save.squadList)
        {
            Squad _squad = _squadSave.Load();
            PlayManager.squadList.Add(_squad);
        }
        PlayManager.nextSquadID = PlayManager.squadList.Count;

        // Load Towers
        foreach (Tower _tower in PlayManager.towerList)
        {
            GameObject.Destroy(_tower.gameObject);
        }
        PlayManager.towerList.Clear();
        foreach (TowerSave _towerSave in _save.towerList)
        {
            GameObject _instance = GameObject.Instantiate(PlayManager.data.towerPrefab, GameObject.Find("Towers").transform);
            Tower _tower = _towerSave.Load(_instance);
            PlayManager.towerList.Add(_tower);
        }

        // Load Turrets
        foreach (Turret _turret in PlayManager.turretList)
        {
            GameObject.Destroy(_turret.gameObject);
        }
        PlayManager.turretList.Clear();
        foreach (TurretSave _turretSave in _save.turretList)
        {
            GameObject _instance = GameObject.Instantiate(PlayManager.data.turretPrefab, GameObject.Find("Turrets").transform);
            Turret _turret = _turretSave.Load(_instance);
            PlayManager.turretList.Add(_turret);
        }

        // Load Turret bases
        foreach (TurretBase _turretBase in PlayManager.turretBaseList)
        {
            GameObject.Destroy(_turretBase.gameObject);
        }
        PlayManager.turretBaseList.Clear();
        foreach (TurretBaseSave _turretBaseSave in _save.turretBaseList)
        {
            GameObject _instance = GameObject.Instantiate(PlayManager.data.turretBasePrefab, GameObject.Find("Turrets").transform);
            TurretBase _turretBase = _turretBaseSave.Load(_instance);
            PlayManager.turretBaseList.Add(_turretBase);
        }

        // Load HQCandidates
        foreach (HQCandidate _candidate in PlayManager.hqCandidateList)
        {
            GameObject.Destroy(_candidate.gameObject);
        }
        PlayManager.hqCandidateList.Clear();
        foreach (HQCandidateSave _candidateSave in _save.hqCandidateList)
        {
            GameObject _instance = GameObject.Instantiate(PlayManager.data.hqCandidatePrefab, GameObject.Find("HQCandidates").transform);
            HQCandidate _candidate = _candidateSave.Load(_instance);
            PlayManager.hqCandidateList.Add(_candidate);
        }

        // Load Buildings
        foreach (GameObject _building in PlayManager.buildingList)
        {
            GameObject.Destroy(_building);
        }
        PlayManager.buildingList.Clear();
        foreach(BuildingSave _buildingSave in _save.buildingList)
        {
            PlayManager.buildingList.Add(_buildingSave.Load(GameObject.Instantiate(PlayManager.data.buildingPrefab, GameObject.Find("Buildings").transform)));
        }

        // Load HQ
        Vector3 _hqPos = _save.hqSave.Load();
        PlayManager.hq.transform.position = _hqPos;
        PlayManager.hqPos = GridAdjustment.GetGridCoordinates(new Vector3(_hqPos.x, 0f, _hqPos.z));
        PlayManager.hq.SetSpawnPoints();

        PlayManager.soldierNav.BuildNavMesh();
        PlayManager.squadNav.BuildNavMesh();
    }
}


