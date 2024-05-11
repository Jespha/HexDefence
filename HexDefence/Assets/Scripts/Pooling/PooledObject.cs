using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{   
    [SerializeField] private float timeUntilReturn = 2.5f;

    void OnEnable ()
    {
        if (timeUntilReturn > 0)
        StartCoroutine(ReturnToPool(timeUntilReturn));
    }


    public IEnumerator ReturnToPool(float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        gameObject.SetActive(false);
        // _pooledObject.ReturnToPool();
        // Debug.Log("Deactivate object " + transform.parent.name);
    }

}
