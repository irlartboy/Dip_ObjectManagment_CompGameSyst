using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameM : PersistableObject
{
    [SerializeField] ShapeFactory shapeFactory;

    [SerializeField] Slider creationSpeedSlider;
    [SerializeField] Slider destructionSpeedSlider;

    public string savePath;
    public List<Shape> shapes;
    public PersistentStorage storage;
    const int saveVersion = 4;
    float creationProgress;
    float destructionProgress;
    public int levelCount;
    int loadedLevelBuildIndex;
    [SerializeField] bool reseedOnLoad;
    Random.State mainRandomState;
    // public SpawnZone spawnZone;
  //  public SpawnZone SpawnZoneOfLevel { get; set; }

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    //public static GameM Instance { get; private set; }

   // void OnEnable()
    //{
   //     Instance = this;
    //}
    void Start()
    {
        mainRandomState = Random.state;
        //Instance = this;
        shapes = new List<Shape>();
        if (Application.isEditor)
        {
            // Scene loadedLevel = SceneManager.GetSceneByName("Level 1");
            // if (loadedLevel.isLoaded)
            // {
            //    SceneManager.SetActiveScene(loadedLevel);
            //     return;
            // }
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level_"))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }
        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            // Instantiate(prefab);
            CreateShape();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            DestroyShape();
        }
        else if (Input.GetKey(KeyCode.N))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
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
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }
        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }
    
    
    IEnumerator LoadLevel(int levelBuildIndex)
    {
        // SceneManager.LoadScene("Level_1", LoadSceneMode.Additive);
        // yield return null;
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }
   
    void CreateShape()
    {
        // PersistableObject o = Instantiate(prefab);
        Shape instance = shapeFactory.GetRandom();
        //Transform t = instance.transform;
        // t.localPosition = Random.insideUnitSphere * 5f;
        // t.localPosition = GameLevel.Current.SpawnPoint;
        // t.localRotation = Random.rotation;
        // t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        //instance.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f, valueMin: 0.25f, valueMax: 1f,alphaMin: 1f, alphaMax: 1f));
        //instance.AngularVelocity = Random.onUnitSphere * Random.Range(0f,90f);
        // instance.Velocity = Random.onUnitSphere * Random.Range(0f, 2f);
        GameLevel.Current.ConfigureSpawn(instance);
        shapes.Add(instance);
    }
    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            // Destroy(shapes[index].gameObject);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }
    void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
       // CreationSpeed = 0;
        creationSpeedSlider.value = CreationSpeed = 0;
       // DestructionSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;
        for (int i = 0; i < shapes.Count; i++)
        {
            //Destroy(shapes[i].gameObject);
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }
    public override void Save(GameDataWriter writer)
    {
        //  writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(destructionProgress);
        writer.Write(DestructionSpeed);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
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
        StartCoroutine(LoadGame(reader));

    }
    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3)
        {
            // Random.state = reader.ReadRandomState();
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }
        // StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3)
        {
            GameLevel.Current.Load(reader);
        }
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
