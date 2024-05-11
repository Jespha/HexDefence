using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

    // Start is called before the first frame update
[CreateAssetMenu(fileName = "ClickType", menuName = "ScriptableObjects/ClickType", order = 3)]
public class ClickType : ScriptableObject
{
    public PooledObject pooledObject;
    public LayerMask layerMask;
    public int poolSize;
}
