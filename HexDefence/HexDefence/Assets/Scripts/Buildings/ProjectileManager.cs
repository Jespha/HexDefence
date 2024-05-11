using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{

    public List<PooledObject> activeProjectiles = new List<PooledObject>();
    public List<GameObject> activeProjectilesTarget = new List<GameObject>();
    private ProjectileData[] projectileData = new ProjectileData[0];
    [SerializeField] private GameObject _projectileParent;

    private void OnEnable()
    {
        StartCoroutine(WaitForGameManager());
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelStart -= ClearProjectiles;
        }
    }

    private IEnumerator WaitForGameManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.OnLevelStart += ClearProjectiles;
    }

    public void AddProjectile(HexCell hexcell, GameObject enemy)
    {
        Projectile projectilePrefab = hexcell.HexBuilding.ProjectilePrefab;
        PooledObject projectile = PooledObjectManager.Instance.Get(projectilePrefab.ProjectilePrefab);
        projectile.gameObject.transform.SetParent(_projectileParent.transform);
        
        Vector3 startPosition = hexcell.Position + new Vector3(0, 1, 0);
        projectile.transform.position = startPosition;
        projectile.transform.LookAt(enemy.transform);

        PooledObject launch = PooledObjectManager.Instance.Get(hexcell.HexBuilding.ProjectilePrefab.LaunchVFXPrefab);
        launch.transform.position = startPosition;
        launch.transform.LookAt(enemy.transform);

        ProjectileData[] temp = new ProjectileData[projectileData.Length + 1];
        for (int i = 0; i < projectileData.Length; i++)
        {
            temp[i] = projectileData[i];
        }
        
        temp[temp.Length - 1].projectile = projectile;
        temp[temp.Length - 1].startPosition = hexcell.Position + new Vector3(0, 1, 0);
        temp[temp.Length - 1].endPosition = enemy.transform.position + new Vector3(0, 0.5f, 0);
        temp[temp.Length - 1].speed = hexcell.HexBuilding.ProjectilePrefab.AttackSpeed;
        temp[temp.Length - 1].damage = hexcell.HexBuilding.ProjectilePrefab.AttackDamage;
        temp[temp.Length - 1].launchVFX = launch;
        temp[temp.Length - 1].impactVFX = hexcell.HexBuilding.ProjectilePrefab.ImpactVFXPrefab;
        projectileData = temp;
        activeProjectiles.Add(projectile);
        activeProjectilesTarget.Add(enemy);
    }

    public void ClearProjectiles(int levelNumber, Level level)
    {
        foreach (var projectile in activeProjectiles)
        {
            PooledObjectManager.Instance.ReturnToPool(projectile);
        }
        activeProjectiles.Clear();
    }

    public void Update()
    {
        if (activeProjectiles.Count > 0)
        {
            UpdateProjectiles();
        }
    }

    private void UpdateProjectiles()
    {
        for (int i = 0; i < activeProjectiles.Count; i++)
        {
            if (Vector3.Distance(projectileData[i].projectile.transform.position, projectileData[i].endPosition) > 0.1f)
            {
                projectileData[i].projectile.transform.position = Vector3.MoveTowards(projectileData[i].projectile.transform.position, projectileData[i].endPosition, projectileData[i].speed * Time.deltaTime);
                projectileData[i].projectile.transform.LookAt(projectileData[i].endPosition);
            }
            else
            {
                if (projectileData[i].impactVFX != null)
                {
                    projectileData[i].projectile.transform.LookAt(projectileData[i].endPosition);
                    PooledObject impact = PooledObjectManager.Instance.Get(projectileData[i].impactVFX);
                    impact.transform.position = projectileData[i].endPosition;
                    impact.transform.LookAt(projectileData[i].startPosition);
                }
                GameManager.Instance.EnemyManager.DamageEnemy(activeProjectilesTarget[i], projectileData[i].damage);
                activeProjectilesTarget.RemoveAt(i);
                PooledObjectManager.Instance.ReturnToPool(projectileData[i].projectile);
                activeProjectiles.RemoveAt(i);
                RemoveProjectileData(i);
            }
        }
    }

    private void RemoveProjectileData(int index)
    {
        ProjectileData[] temp = new ProjectileData[projectileData.Length - 1];
        for (int i = 0; i < temp.Length; i++)
        {
            if (i < index)
            {
                temp[i] = projectileData[i];
            }
            else
            {
                temp[i] = projectileData[i + 1];
            }
        }
        projectileData = temp;
    }

}

struct ProjectileData
{
    public PooledObject projectile;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed;
    public float damage;
    public PooledObject launchVFX;
    public PooledObject impactVFX;
}