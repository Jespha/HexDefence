using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public ClickPool pool;

    [SerializeField] private PooledObject _clickPrefab;
    // [SerializeField] private ClickPrefabs clickPrefabs;

    private void OnEnable()
    {
       MouseController.Instance.OnLeftMouseClick += OnLeftMouseClick;
       MouseController.Instance.OnRightMouseClick += OnRightMouseClick;
    }

    private void OnDisable()
    {
       MouseController.Instance.OnLeftMouseClick -= OnLeftMouseClick;
       MouseController.Instance.OnRightMouseClick -= OnRightMouseClick;
    }



    private void OnLeftMouseClick(RaycastHit hit)
    {
        float localX = hit.point.x - transform.position.x;
        float localZ = hit.point.z - transform.position.z;

        PooledObject obj = pool.Get(_clickPrefab);
        obj.transform.position =  hit.point;
        Debug.Log("Lclick");
    }

    private void OnRightMouseClick(RaycastHit hit)
    {
        // Debug.Log("L HitObject: " + hit.transform.name + " at position " + hit.point);
        float localX = hit.point.x - transform.position.x;
        float localZ = hit.point.z - transform.position.z;
        Debug.Log("Rclick");
    }

}
