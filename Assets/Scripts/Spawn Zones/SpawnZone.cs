using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnZone : PersistableObject
{

    public abstract Vector3 SpawnPoint { get;  }

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
