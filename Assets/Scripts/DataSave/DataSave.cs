using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

/// <summary>
/// DataSave defines the methods to load and save the game data
/// </summary>
[XmlRoot("DataSave")]
public class DataSave
{
    [XmlElement("Coins")]
    public int coins;
    [XmlElement("Day")]
    public int day;
    [XmlElement("Infirmary")]
    public int infirmary;

    [XmlArray("Squads"), XmlArrayItem("SquadSave")]
    public List<SquadSave> squadList = new List<SquadSave>();

    [XmlArray("Soldiers"), XmlArrayItem("SoldierSave")]
    public List<SoldierSave> soldierList = new List<SoldierSave>();

    /// <summary>
    /// Basic constructor of an empty SquadSave (needed to xml saves)
    /// </summary>
    public DataSave()
    {
        // All int are 0 and list are created as new empty lists so there is no need to do anything here
    }

    /// <summary>
    /// Save method creates a new DataSave from game data
    /// </summary>
    private void Save()
    {
        coins = PlayManager.coins;
        day = PlayManager.day;
        infirmary = PlayManager.infirmaryLevel;

        foreach (Squad _squad in PlayManager.squadList)
        {
            squadList.Add(new SquadSave(_squad));
        }

        foreach (Soldier _soldier in PlayManager.soldierList)
        {
            soldierList.Add(new SoldierSave(_soldier));
        }
    }

    /// <summary>
    /// AutoSaveGame method creates a savefile name AutoSave.xml and archive the previous one if it exists
    /// </summary>
    public void AutoSaveGame()
    {
        // Create the DataSave
        Save();

        // Checks if a previous AutoSave file exists and renames it
        if (File.Exists(Path.Combine(Application.persistentDataPath, "AutoSave.xml")))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "AutoSave.old"));
            File.Move(Path.Combine(Application.persistentDataPath, "AutoSave.xml"), Path.Combine(Application.persistentDataPath, "AutoSave.old"));
        }

        // Saves the DataSave into the AutoSave.xml file
        var serializer = new XmlSerializer(typeof(DataSave));
        using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, "AutoSave.xml"), FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    /// <summary>
    /// LoadAutoSavedGame method creates a DataSave from the AutoSave.xml file
    /// </summary>
    /// <returns>DataSave with all game data</returns>
    public static DataSave LoadAutoSavedGame()
    {
        return LoadSavedGame("AutoSave.xml");
    }

    /// <summary>
    /// LoadSavedGame method creates a DataSave from the chosen file
    /// </summary>
    /// <param name="_fileName">Save file to load the data from</param>
    /// <returns>DataSave with all game data</returns>
    public static DataSave LoadSavedGame(string _fileName)
    {
        if(File.Exists(Path.Combine(Application.persistentDataPath, _fileName)))
        {
            var serializer = new XmlSerializer(typeof(DataSave));
            using (var stream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Open))
            {
                return serializer.Deserialize(stream) as DataSave;
            }
        }
        else
        {
            Debug.LogError("[DataSave] The SaveFile doesn't exists!");
            return null;
        }
        
    }
}
