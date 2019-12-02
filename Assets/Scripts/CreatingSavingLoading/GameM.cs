using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameM : PersistableObject
{
    public ShapeFactory shapeFactory;
    public string savePath;
    public List<Shape> shapes;
    public PersistentStorage storage;
    const int saveVersion = 1;

    void Awake()
    {
        shapes = new List<Shape>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Instantiate(prefab);
            CreateShape();
        }
        else if (Input.GetKey(KeyCode.N))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    void CreateShape()
    {
        // PersistableObject o = Instantiate(prefab);
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f, valueMin: 0.25f, valueMax: 1f,alphaMin: 1f, alphaMax: 1f));
        shapes.Add(instance);
    }
    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }
    public override void Save(GameDataWriter writer)
    {
        //  writer.Write(-saveVersion);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            // PersistableObject o = Instantiate(prefab);
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
    /* void Save()
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
     } */
}
