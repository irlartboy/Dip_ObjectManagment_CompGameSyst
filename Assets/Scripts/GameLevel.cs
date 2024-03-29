﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{
    [SerializeField]
    SpawnZone spawnZone;
    [SerializeField]
    PersistableObject[] persistentObjects;

    public static GameLevel Current { get; private set; }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistentObjects.Length);
        for (int i = 0; i < persistentObjects.Length; i++)
        {
            persistentObjects[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            persistentObjects[i].Load(reader);
        }
    }
    //void Start()
    //{
    //    GameM.Instance.SpawnZoneOfLevel = spawnZone;
    // }
    // public Vector3 SpawnPoint
    // {
    //    get
    //    {
    //        return spawnZone.SpawnPoint;
    //    }
    //}
    public void ConfigureSpawn(Shape shape)
    {
        spawnZone.ConfigureSpawn(shape);
    }
    
    void OnEnable()
    {
        Current = this;
        if (persistentObjects == null)
        {
            persistentObjects = new PersistableObject[0];
        }
    }
}
