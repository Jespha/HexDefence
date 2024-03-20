using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class PooledObjectManager : MonoBehaviour
{
    [SerializeField]
    private ClickManager _clickManager;

    public PooledObject[] _prefab;
    public int[] _poolSize;
    public List<PooledObject>[] _pools;
    private List<PooledObject> _tempObjects = new List<PooledObject>();

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
    }

    public PooledObject Get(PooledObject _pooledObject)
    {
        for (int i = 0; i < _prefab.Length; i++)
        {
            if (_prefab[i] != null && _prefab[i] == _pooledObject)
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

    public void ReturnToPool(PooledObject _pooledObject)
    {
        for (int i = 0; i < _prefab.Length; i++)
        {
            if (_prefab[i] != null && _prefab[i] == _pooledObject)
            {
                for (int j = 0; j < _pools[i].Count; j++)
                {
                    if (_pools[i][j] == _pooledObject)
                    {
                        _pools[i][j].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void InitalizePools()
    {
        ClickType[] _clickTypes = _clickManager.clickTypes;

        Array.Resize(ref _prefab, _prefab.Length + _clickTypes.Length);
        Array.Resize(ref _poolSize, _poolSize.Length + _clickTypes.Length);
        Array.Resize(ref _pools, _prefab.Length);

        int offset = _prefab.Length - _clickTypes.Length;
        for (int i = offset; i < _prefab.Length; i++)
        {
            _prefab[i] = _clickTypes[i - offset].pooledObject;
            _poolSize[i] = _clickTypes[i - offset].poolSize;
            _pools[i] = new List<PooledObject>(_poolSize[i]);
        }

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

    public void AddToPool(PooledObject _pooledObject, int _amount)
    {
        int index = Array.IndexOf(_prefab, _pooledObject);
        if (index != -1)
        {
            // If the object is already in the array, instantiate new ones and add them to the pool
            for (int j = 0; j < _amount; j++)
            {
                PooledObject obj = Instantiate(_prefab[index], this.transform) as PooledObject;
                obj.gameObject.SetActive(false);
                _pools[index].Add(obj);
                _tempObjects.Add(obj); // Keep track of added objects
            }
            _poolSize[index] += _amount; // Increase the pool size
        }
        else
        {
            // If the object is not in the array, add it
            Array.Resize(ref _prefab, _prefab.Length + 1);
            _prefab[_prefab.Length - 1] = _pooledObject;

            // Resize _poolSize and add the new size
            Array.Resize(ref _poolSize, _poolSize.Length + 1);
            _poolSize[_poolSize.Length - 1] = _amount;

            // Resize _pools and add a new list for the new type of PooledObject
            List<PooledObject>[] tempPools = new List<PooledObject>[_pools.Length + 1];
            Array.Copy(_pools, tempPools, _pools.Length);
            tempPools[_pools.Length] = new List<PooledObject>();
            _pools = tempPools;

            AddToPool(_pooledObject, _amount); // Recursive call
        }
    }
    
    public void LevelComplete()
    {
        foreach (PooledObject obj in _tempObjects)
        {
            // Remove the object from the pool
            for (int i = 0; i < _prefab.Length; i++)
            {
                if (_prefab[i] != null && _prefab[i] == obj)
                {
                    _pools[i].Remove(obj);
                }
            }
        }
        // Clear the list of added objects
        _tempObjects.Clear();
    }

}
