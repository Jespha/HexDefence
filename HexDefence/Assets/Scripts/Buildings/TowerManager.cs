using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField]EnemyManager _enemyManager;
    [SerializeField] private ProjectileManager _projectileManager;
    TowerData[] Towers = new TowerData[0]; 
    List<Collider> _colliders = new List<Collider>();
    public List<HexBuilding> HexBuildings = new List<HexBuilding>(); // Live copy of base HexBuilding
    public static TowerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of TowerManager detected. Deleting one.");
            Destroy(this);
        }
    }

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

    public void UnlockTower(HexBuilding newHexBuilding)
    {
        HexBuilding hexBuilding = newHexBuilding.Clone();
        HexBuildings.Add(hexBuilding);
    }

    public void ClearHexBuildings()
    {
        HexBuildings.Clear();
    }

    public void AddTower(HexCell hexCell, PooledObject towerPrefab)
    {
        TowerData[] temp = new TowerData[Towers.Length + 1];
        for (int i = 0; i < Towers.Length; i++)
        {
            temp[i] = Towers[i];
        }
        temp[temp.Length - 1].hexCell = hexCell;
        temp[temp.Length - 1].towerPrefab = towerPrefab;
        if(hexCell.HexBuilding.AimPart != null)
        {
            GameObject aimPartInstance = Instantiate(hexCell.HexBuilding.AimPart, hexCell.Position, Quaternion.identity);
            Vector3 originalScale = aimPartInstance.transform.localScale;
            aimPartInstance.transform.parent = towerPrefab.transform;
            aimPartInstance.transform.localScale = originalScale; 
            temp[temp.Length - 1].AimPart = aimPartInstance;
        }
        temp[temp.Length - 1].position = hexCell.Position;
        temp[temp.Length - 1].attackType = hexCell.HexBuilding.AttackType;
        temp[temp.Length - 1].TypeCollider = hexCell.HexBuilding.TypeCollider;
        temp[temp.Length - 1].colliderSize = hexCell.HexBuilding.ColliderSize;
        temp[temp.Length - 1].colliderPosition = hexCell.HexBuilding.ColliderPosition;
        temp[temp.Length - 1].attackDamage = hexCell.HexBuilding.AttackDamage;
        temp[temp.Length - 1].attackSpeed = hexCell.HexBuilding.AttackSpeed;
        temp[temp.Length - 1].attackRange = hexCell.HexBuilding.AttackRange;
        temp[temp.Length - 1].attackPriority = hexCell.HexBuilding.TargetPriority;
        temp[temp.Length - 1].lastAttackTime = 0;

        List<int> newUpgradeList = new List<int>(); // Local upgrads ( 0 Damage, 1 AttackSpeed, 2 Range) 
        for (int i = 0; i < 3; i++)
        {
            newUpgradeList.Add(1);
        }
        temp[temp.Length - 1].localUpgrades = newUpgradeList;
        Towers = temp;
        // _colliders.Add(AddColliderToTower(towerPrefab, Towers[Towers.Length - 1]));
    }

    public void UpgradeLocalTower(int index, int upgradeIndex)
    {
        Towers[index].localUpgrades[upgradeIndex]++;
    }

    public void ApplyUpgradeToTowerManagerHexBuildings(Upgrade upgrade)
    {
        HexBuilding _building = upgrade._building;
        int _hexBuildingIndex;

        for (int i = 0; i < HexBuildings.Count; i++)
        {
            if (HexBuildings[i] == _building)
            {
                _hexBuildingIndex = i;
                HexBuildings[i].UpgradeStats(upgrade.AttackDamageUpgrade, upgrade.AttackSpeedUpgrade, upgrade.AttackRangeUpgrade);
                ChangeTowerDataValues(HexBuildings[i], upgrade);
            }
        }
        ApplyUpgradeToTowers(upgrade);
    }
    
    private void ApplyUpgradeToTowers(Upgrade upgrade)
    {
        HexBuilding _hexBuildingPrefab = null;
        foreach (var hexBuilding in HexBuildings)
        {
            if (hexBuilding == upgrade._building)
            {
                _hexBuildingPrefab = hexBuilding;
            }
        }

        for (int i = 0; i < Towers.Length; i++)
        {
            if (Towers[i].towerPrefab.name == upgrade._building.Name)
            {
                Towers[i].attackDamage = _hexBuildingPrefab.AttackDamage;
                Towers[i].attackSpeed = _hexBuildingPrefab.AttackSpeed;
                Towers[i].attackRange = _hexBuildingPrefab.AttackRange;
            }
        }
    }
    
    public void ChangeTowerDataValues(HexBuilding hexBuilding, Upgrade upgrade)
    {
        for (int i = 0; i < Towers.Length; i++)
        {
            TowerData towerData = Towers[i];
            if (towerData.towerPrefab == hexBuilding.Prefab)
            {
                towerData.attackDamage = hexBuilding.AttackDamage;
                towerData.attackSpeed = hexBuilding.AttackSpeed;
                towerData.attackRange = hexBuilding.AttackRange;
                Towers[i] = towerData;
            }
        }
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
        if (GameManager.Instance != null && _enemyManager != null)
        {
            if (GameManager.Instance.GamePhase == GamePhase.Defend && _enemyManager != null && _enemyManager.activeEnemies != null && _enemyManager.activeEnemies.Count > 0)            
            {
                for (int i = 0; i < Towers.Length; i++)
                {
                    // Temporary copy of the keys in the dictionary
                    var enemies = new List<GameObject>(_enemyManager.activeEnemies.Keys);

                    foreach (var enemy in enemies)
                    {
                        // Check if the enemy is still in the dictionary
                        if (!_enemyManager.activeEnemies.ContainsKey(enemy))
                            continue;

                        if (Vector3.Distance(Towers[i].position, enemy.transform.position) <= Towers[i].attackRange)
                        {
                            if (Towers[i].AimPart != null)
                                Towers[i].AimPart.transform.LookAt(enemy.transform);
                                //TODO: ADD GLOBAL SPEED UPGRADS TO ATTACK SPEED
                                
                            if (Towers[i].lastAttackTime + Towers[i].attackSpeed - (Towers[i].attackSpeed*(Towers[i].localUpgrades[1]/50)) > Time.time)
                                continue; 
                            Towers[i].lastAttackTime = Time.time;
                            switch (Towers[i].attackType)
                            {
                                case AttackType.Projectile:
                                    projectileLogic(Towers[i].hexCell, enemy, Towers[i].AimPart);
                                    break;
                                case AttackType.Area:
                                    areaLogic(Towers[i].hexCell);
                                    break;
                                case AttackType.Beam:
                                    hitScanLogic(Towers[i].hexCell);
                                    break;
                                case AttackType.Economy:
                                    generateCurrencyLogic(Towers[i].hexCell);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }


    private void projectileLogic(HexCell hexCell, GameObject enemy, GameObject aimPart)
    {
        _projectileManager.AddProjectile(hexCell, enemy);
        if (aimPart == null)
            return;
        AnimationCurve _curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0.5f));
        StartCoroutine(AnimationCoroutine.SetScaleVec3Coroutine(aimPart.transform, new Vector3(1, 1, 0.5f), new Vector3(1, 1, 1), _curve, 0.3f));
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
    public PooledObject towerPrefab;
    public GameObject AimPart;
    public Vector3 position;
    public AttackType attackType;
    public TypeCollider TypeCollider;
    public Vector3 colliderSize;
    public Vector3 colliderPosition;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float lastAttackTime;
    public AttackPriority attackPriority;
    public List<int> localUpgrades;
}
