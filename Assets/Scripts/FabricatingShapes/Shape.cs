using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject 
{
    int shapeId = int.MinValue;
    Color color;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetColor(Color color)
    {
        this.color = color;
        // GetComponent<MeshRenderer>().material.color = color;
        meshRenderer.material.color = color;
    }
    public int ShapeId 
    { 
        get { return shapeId; }
        set
        {
            if (shapeId == 0)
            {
                ShapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }
    public int MaterialId { get; private set; } 
    public void SetMaterial (Material material, int materialId)
    {
        //GetComponent<MeshRenderer>().material = material;
        meshRenderer.material.color = color;
        MaterialId = materialId;
    }
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor(): Color.white);
    }
}
