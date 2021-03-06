﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// SoldierNamesData are the list of soldiers name
/// </summary>
[CreateAssetMenu(fileName = "SoldierNames", menuName = "Tower Offense/Soldier names Data", order = 103)]
public class SoldierNamesData : ScriptableObject
{
    // CSV file assets
    public TextAsset lastNamesFile;
    public TextAsset maleFirstNamesFile;
    public TextAsset femaleFirstNamesFile;

    // Name lists
    public List<string> lastNamesList = new List<string>();
    public List<string> maleFirstNamesList = new List<string>();
    public List<string> femaleFirstNamesList = new List<string>();

    // CSV separator
    private char lineSeparator = '\n'; // It defines line seperate character
    private char fieldSeparator = ','; // It defines field seperate chracter

    /// <summary>
    /// LoadLastNames method loads the soldier's last names from the CSV file
    /// </summary>
    public void LoadLastNames()
    {
#if UnityEditor
        lastNamesList.Clear();
        if(lastNamesFile != null)
        {
            // Parse CSV
            string[] records = lastNamesFile.text.Split(lineSeparator);
            foreach (string record in records)
            {
                string[] fields = record.Split(fieldSeparator);
                foreach (string field in fields)
                {
                    lastNamesList.Add(field);
                }
            }
        }
#endif
    }

    /// <summary>
    /// SaveLastNames method saves the soldier's last names to the CSV file
    /// </summary>
    public void SaveLastNames()
    {
#if UnityEditor
        if(lastNamesFile != null)
        {
            string filePath = AssetDatabase.GetAssetPath(lastNamesFile);
            StreamWriter writer = new StreamWriter(filePath);

            foreach (string _s in lastNamesList)
            {
                writer.Write(_s);
            }

            writer.Flush();
            writer.Close();
        }
#endif
    }

    /// <summary>
    /// LoadMaleNames method loads the soldier's male first names from the CSV file
    /// </summary>
    public void LoadMaleNames()
    {
#if UnityEditor
        maleFirstNamesList.Clear();
        if (maleFirstNamesFile != null)
        {
            // Parse CSV
            string[] records = maleFirstNamesFile.text.Split(lineSeparator);
            foreach (string record in records)
            {
                string[] fields = record.Split(fieldSeparator);
                foreach (string field in fields)
                {
                    maleFirstNamesList.Add(field);
                }
            }
        }
#endif
    }

    /// <summary>
    /// SaveMaleNames method saves the soldier's male first names to the CSV file
    /// </summary>
    public void SaveMaleNames()
    {
#if UnityEditor
        if (maleFirstNamesFile != null)
        {
            string filePath = AssetDatabase.GetAssetPath(maleFirstNamesFile);
            StreamWriter writer = new StreamWriter(filePath);

            foreach (string _s in maleFirstNamesList)
            {
                writer.Write(_s);
            }

            writer.Flush();
            writer.Close();
        }
#endif
    }

    /// <summary>
    /// LoadFemaleNames method loads the soldier's female first names from the CSV file
    /// </summary>
    public void LoadFemaleNames()
    {
#if UnityEditor
        femaleFirstNamesList.Clear();
        if (femaleFirstNamesFile != null)
        {
            // Parse CSV
            string[] records = femaleFirstNamesFile.text.Split(lineSeparator);
            foreach (string record in records)
            {
                string[] fields = record.Split(fieldSeparator);
                foreach (string field in fields)
                {
                    femaleFirstNamesList.Add(field);
                }
            }
        }
#endif
    }

    /// <summary>
    /// SaveFemaleNames method saves the soldier's female first names to the CSV file
    /// </summary>
    public void SaveFemaleNames()
    {
#if UnityEditor
        if (femaleFirstNamesFile != null)
        {
            string filePath = AssetDatabase.GetAssetPath(femaleFirstNamesFile);
            StreamWriter writer = new StreamWriter(filePath);

            foreach (string _s in femaleFirstNamesList)
            {
                writer.Write(_s);
            }

            writer.Flush();
            writer.Close();
        }
#endif
    }
}
