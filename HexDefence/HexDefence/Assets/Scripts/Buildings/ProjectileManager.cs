using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{

    public List<GameObject> activeProjectiles = new List<GameObject>();
    public List<GameObject> activeProjectilesTarget = new List<GameObject>();
    private ProjectileData[] projectileData = new ProjectileData[0];

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
        GameObject projectile = Instantiate(projectilePrefab.ProjectilePrefab, this.transform.position, Quaternion.identity);
        
        Vector3 startPosition = hexcell.Position + new Vector3(0, 1, 0);
        projectile.transform.position = startPosition;
        projectile.transform.LookAt(enemy.transform);

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
        temp[temp.Length - 1].launchVFX = hexcell.HexBuilding.ProjectilePrefab.LaunchVFXPrefab;
        temp[temp.Length - 1].impactVFX = hexcell.HexBuilding.ProjectilePrefab.ImpactVFXPrefab;
        projectileData = temp;
        activeProjectiles.Add(projectile);
        activeProjectilesTarget.Add(enemy);
    }

    public void ClearProjectiles(int levelNumber, Level level)
    {
        foreach (var projectile in activeProjectiles)
        {
            Destroy(projectile);
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
                    Instantiate(projectileData[i].impactVFX, projectileData[i].endPosition, projectileData[i].projectile.transform.rotation * Quaternion.Euler(0, 180, 0));
                }
                GameManager.Instance.EnemyManager.DamageEnemy(activeProjectilesTarget[i], projectileData[i].damage);
                activeProjectilesTarget.RemoveAt(i);
                Destroy(projectileData[i].projectile);
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
    public GameObject projectile;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed;
    public float damage;
    public GameObject launchVFX;
    public GameObject impactVFX;
}