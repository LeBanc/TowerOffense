using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : Singleton<PlayManager>
{
    public GameData gameData;

    public static GameData data;

    // Start is called before the first frame update
    void Start()
    {
        data = gameData;
    }

    public static float LongRange
    {
        get{return data.longRange ;}
    }

    public static float MiddleRange
    {
        get { return data.middleRange; }
    }

    public static float ShortRange
    {
        get { return data.shortRange; }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
