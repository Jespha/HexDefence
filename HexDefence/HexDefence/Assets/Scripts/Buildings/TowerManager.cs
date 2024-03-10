using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerManager : MonoBehaviour
{
    [SerializeField]EnemyManager _enemyManager;
    [SerializeField] private ProjectileManager _projectileManager;
    TowerData[] Towers = new TowerData[0]; 
    List<Collider> _colliders = new List<Collider>();

    private void Start ()
    {
        if (_enemyManager == null)
        {
            _enemyManager = FindObjectOfType<EnemyManager>();
            if (_enemyManager == null)
            {
                Debug.Log("EnemyManager not found");
                gameObject.SetActive(false);
            }
        }
    }

    public void AddTower(HexCell hexCell, GameObject towerPrefab)
    {
        TowerData[] temp = new TowerData[Towers.Length + 1];
        for (int i = 0; i < Towers.Length; i++)
        {
            temp[i] = Towers[i];
        }
        temp[temp.Length - 1].hexCell = hexCell;
        temp[temp.Length - 1].towerPrefab = towerPrefab;
        temp[temp.Length - 1].position = hexCell.Position;
        temp[temp.Length - 1].attackType = hexCell.HexBuilding.AttackType;
        temp[temp.Length - 1].TypeCollider = hexCell.HexBuilding.TypeCollider;
        temp[temp.Length - 1].colliderSize = hexCell.HexBuilding.ColliderSize;
        temp[temp.Length - 1].colliderPosition = hexCell.HexBuilding.ColliderPosition;
        temp[temp.Length - 1].attackSpeed = hexCell.HexBuilding.AttackSpeed;
        temp[temp.Length - 1].attackRange = hexCell.HexBuilding.AttackRange;
        temp[temp.Length - 1].attackDamage = hexCell.HexBuilding.AttackDamage;
        temp[temp.Length - 1].attackPriority = hexCell.HexBuilding.TargetPriority;
        temp[temp.Length - 1].lastAttackTime = 0;
        Towers = temp;
        _colliders.Add(AddColliderToTower(towerPrefab, Towers[Towers.Length - 1]));
    }

    private Collider AddColliderToTower(GameObject towerPrefab, TowerData towerData)
    {
        SphereCollider sc = null;
        BoxCollider bc = null;
        var collider = towerData.TypeCollider;
        switch (collider)
        {
            case TypeCollider.SphereCollider:
                sc = towerPrefab.AddComponent<SphereCollider>();
                sc.radius = towerData.colliderSize.x;
                sc.center = towerData.colliderPosition;
                sc.isTrigger = true;
                return sc;

            case TypeCollider.BoxCollider:
                bc = towerPrefab.AddComponent<BoxCollider>();
                bc.size = towerData.colliderSize;
                bc.center = towerData.colliderPosition;
                bc.isTrigger = true;
                return bc;

        }

        return null;
    }

    public void Update()
    {
        if (GameManager.Instance.GamePhase == GamePhase.Defend)
        {
            for (int i = 0; i < Towers.Length; i++)
            {
                for (int j = 0; j < _enemyManager.activeEnemies.Count; j++)
                {
                    if (Vector3.Distance(Towers[i].position, _enemyManager.activeEnemies[j].transform.position) <= Towers[i].attackRange)
                    {
                        if (Towers[i].lastAttackTime + Towers[i].attackSpeed > Time.time)
                            continue; // Skip to the next iteration of the loop
                        Towers[i].lastAttackTime = Time.time;
                        switch (Towers[i].attackType)
                        {
                            case AttackType.Projectile:
                                projectileLogic(Towers[i].hexCell, _enemyManager.activeEnemies[j]);
                                break;
                            case AttackType.Area:
                                areaLogic(Towers[i].hexCell);
                                break;
                            case AttackType.HitScan:
                                hitScanLogic(Towers[i].hexCell);
                                break;
                            case AttackType.GenerateCurrency:
                                generateCurrencyLogic(Towers[i].hexCell);
                                break;
                        }
                    }
                }
            }
        }
    }


    private void projectileLogic(HexCell hexCell, GameObject enemy)
    {
        _projectileManager.AddProjectile(hexCell, enemy);
    }

    private void areaLogic(HexCell hexCell)
    {
        // Area logic
    }

    private void hitScanLogic(HexCell hexCell)
    {
        // HitScan logic
    }

    private void generateCurrencyLogic(HexCell hexCell)
    {
        // GenerateCurrency logic
    }



}

struct TowerData
{
    public HexCell hexCell;
    public GameObject towerPrefab;
    public Vector3 position;
    public AttackType attackType;
    public TypeCollider TypeCollider;
    public Vector3 colliderSize;
    public Vector3 colliderPosition;
    public float attackSpeed;
    public float attackRange;
    public float attackDamage;
    public float lastAttackTime;
    public AttackPriority attackPriority;
}
