using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject 
{
    int shapeId = int.MinValue;
    //Color color;
    Color[] colors;

    [SerializeField]
    MeshRenderer[] meshRenderers;

    // MeshRenderer meshRenderer;

    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

  

    void Awake()
    {
        colors = new Color[meshRenderers.Length];
    }
    // private void Awake()
    // {
    //     meshRenderer = GetComponent<MeshRenderer>();
    //}
    public void SetColor(Color color)
    {
        // this.color = color;
        // GetComponent<MeshRenderer>().material.color = color;
        // meshRenderer.material.color = color;
        // var propertyBlock = new MaterialPropertyBlock();
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }

        //meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }
    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }
    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
    }
    public int ShapeId 
    { 
        get { return shapeId; }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
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
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = material;
        }
        MaterialId = materialId;
    }
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        //writer.Write(color);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                SetColor(reader.ReadColor(), i);
            }
        }
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }
}
