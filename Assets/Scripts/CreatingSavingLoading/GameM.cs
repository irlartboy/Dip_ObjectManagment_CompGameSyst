using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameM : MonoBehaviour
{
    public Transform prefab;
    public string savePath;
    public List<Transform> objects;

    void Awake()
    {
        objects = new List<Transform>();
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Instantiate(prefab);
            CreateObject();
        }
        else if (Input.GetKey(KeyCode.N))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    void CreateObject()
    {
        Transform t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(t);
    }
    void BeginNewGame()
    {
        for (int i = 0; i < object.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }
    void Save()
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(objects.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);

            }
        }
        if (writer != null)
        {
            ((IDisposable)writer).Dispose();
        }
    }
    void Load()
    {
        BeginNewGame();
        using (var writer = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();
                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }
}
