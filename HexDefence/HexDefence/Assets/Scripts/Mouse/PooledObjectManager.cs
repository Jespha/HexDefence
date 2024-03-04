using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PooledObjectManager : MonoBehaviour
{
    [SerializeField]
    private ClickManager _clickManager;

    public PooledObject[] _prefab;
    public int[] _poolSize;
    public List<PooledObject>[] _pools;
    
    public static PooledObjectManager Instance;

    void Awake() => Instance = this;

    void Start()
    {
        if (_clickManager == null)
        {
            _clickManager = FindObjectOfType<ClickManager>();

            if (_clickManager == null)
            {
                Debug.Log("ClickPool not found");
                gameObject.SetActive(false);
            }
        }

        InitalizePools();

        for (int j = 0; j < _pools.Length; j++)
        {
            _pools[j] = new List<PooledObject>();
            for (int i = 0; i < _poolSize[j]; i++)
            {
                PooledObject obj = Instantiate(_prefab[j], this.transform) as PooledObject;
                obj.gameObject.SetActive(false);
                _pools[j].Add(obj);
            }
        }
    }

    public PooledObject Get(PooledObject _clickPrefab)
    {
        for (int i = 0; i < _prefab.Length; i++)
        {
            if (_prefab[i] == _clickPrefab)
            {
                for (int j = 0; j < _pools[i].Count; j++)
                {
                    if (_pools[i][j].gameObject.activeInHierarchy == false)
                    {
                        _pools[i][j].gameObject.SetActive(true);
                        return _pools[i][j];
                    }
                    if (
                        (j == (_pools[i].Count - 1))
                        && (_pools[i][j].gameObject.activeInHierarchy == true)
                    )
                    {
                        IncreasePool(i);

                        return _pools[i][j + 1];
                    }
                }
            }
        }
        return null;
    }

    private void IncreasePool(int _specifiedPool)
    {
        PooledObject obj = Instantiate(_prefab[_specifiedPool], this.transform) as PooledObject;
        obj.gameObject.SetActive(true);
        _pools[_specifiedPool].Add(obj);
    }

    public void InitalizePools()
    {
        ClickType[] _clickTypes = _clickManager.clickTypes;

        _prefab = new PooledObject[_clickTypes.Length];
        _poolSize = new int[_clickTypes.Length];
        _pools = new List<PooledObject>[_clickTypes.Length];

        for (int i = 0; i < _clickTypes.Length; i++)
        {
            _prefab[i] = _clickTypes[i].pooledObject;
            _poolSize[i] = _clickTypes[i].poolSize;
            _pools[i] = new List<PooledObject>(_poolSize[i]);
        }
    }
}
