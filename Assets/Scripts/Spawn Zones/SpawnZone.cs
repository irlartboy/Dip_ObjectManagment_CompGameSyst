using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnZone : PersistableObject
{
   // [SerializeField]
   // SpawnMovementDirection spawnMovementDirection;
    //[SerializeField] float spawnSpeedMin, spawnSpeedMax;
   // [SerializeField]
   // FloatRange spawnSpeed;
    public abstract Vector3 SpawnPoint { get;  }


    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum MovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random
        }

        public MovementDirection movementDirection;
        public FloatRange speed;
    }

    [SerializeField]
    SpawnConfiguration spawnConfig;
    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shape.SetColor(Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
        saturationMin: 0.5f, saturationMax: 1f,
        valueMin: 0.25f, valueMax: 1f,
        alphaMin: 1f, alphaMax: 1f
        ));
        shape.AngularVelocity = Random.onUnitSphere * Random.Range(0f, 90f);
        Vector3 direction;
        switch (spawnConfig.movementDirection)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.MovementDirection.Outward:
                direction = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = transform.forward;
                break;
        }
        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange;
    }

    //[SerializeField]
    //bool surfaceOnly;
    //void OnDrawGizmos()
    // {
    // Gizmos.color = Color.cyan;
    // Gizmos.matrix = transform.localToWorldMatrix;
    // Gizmos.DrawWireSphere(Vector3.zero, 1f);
    // }

    //{
    //get
    // {
    // return Random.insideUnitSphere * 5f + transform.position;
    // return transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);
    //}
    //}

}
//public enum SpawnMovementDirection
//{
    //Forward,
    //Upward,
    //Outward,
    //Random
//}


