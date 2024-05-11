using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarManager : MonoBehaviour
{

    [SerializeField] private PooledObject _hpBar;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private List<PooledObject> _hpBars = new List<PooledObject>();
    [SerializeField] private List<Image> _hpBarImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _hpBarText = new List<TextMeshProUGUI>();
    [SerializeField] private List<GameObject> _activeEnemyObjects = new List<GameObject>();
    
    private void OnEnable()
    {
        StartCoroutine(WaitForEnemyManager());
    }

    private void OnDisable()
    {
        if (_enemyManager != null)
        {
            GameManager.Instance.OnLevelComplete -= ClearHpBars;
            _enemyManager.EnemyAdded -= OnEnemyAdded;
            _enemyManager.EnemyRemoved -= OnEnemyRemoved;
        }
    }

    private IEnumerator WaitForEnemyManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.EnemyManager != null);
        _enemyManager = GameManager.Instance.EnemyManager;
        GameManager.Instance.OnLevelStart -= ClearHpBars;
        _enemyManager.EnemyAdded += OnEnemyAdded;
        _enemyManager.EnemyRemoved += OnEnemyRemoved;
    }

    void Update()
    {
        if (_enemyManager != null)
        {
            for (int i = 0; i < _activeEnemyObjects.Count; i++)
            {
                UpdateHpBar(_activeEnemyObjects[i], i);
            }
        }
    }

    private void OnEnemyAdded(GameObject enemy)
    {
        _activeEnemyObjects.Add(enemy);
        AddHpBar(enemy); 
    }

    private void OnEnemyRemoved(GameObject enemy)
    {
        int index = _activeEnemyObjects.IndexOf(enemy);
        if (index != -1)
        {            

            if (_hpBars[index])
            {
                PooledObjectManager.Instance.ReturnToPool(_hpBars[index]); // TODO: Figure ut why this is not working and i have to use the line below
                _hpBars[index].gameObject.SetActive(false);
                _hpBars.RemoveAt(index);
                _hpBarImages.RemoveAt(index);
                _hpBarText.RemoveAt(index);
            }
            _activeEnemyObjects.RemoveAt(index);
        }
    }

    private void AddHpBar(GameObject enemyObject)
    {
        PooledObject hpBar = Instantiate(_hpBar, transform);
        _hpBars.Add(hpBar);
        foreach (var hpBarImage in hpBar.GetComponentsInChildren<Image>())
        {
            if (hpBarImage.name == "Fill")
                _hpBarImages.Add(hpBarImage);
        }
        TextMeshProUGUI hpBarText = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        hpBarText.text = _enemyManager.ProvideEnemyFloatData(enemyObject, "Health").ToString() + "/" + _enemyManager.ProvideEnemyFloatData(enemyObject, "MaxHealth").ToString();
        _hpBarText.Add(hpBarText);

        // Set the health bar's position to follow the enemy
        hpBar.transform.position = Camera.main.WorldToScreenPoint(enemyObject.transform.position + new Vector3(0, 2, 0));
    }

    private void UpdateHpBar(GameObject enemyObject, int index)
    {
        if (_hpBars[index])
        {
            float enemyHealth = _enemyManager.ProvideEnemyFloatData(enemyObject, "Health");
            float enemyMaxHealth = _enemyManager.ProvideEnemyFloatData(enemyObject, "MaxHealth");

            if (enemyHealth <= 0)
            {
                OnEnemyRemoved(enemyObject);
            } 
            else
            {
                _hpBars[index].transform.position = Camera.main.WorldToScreenPoint(enemyObject.transform.position + new Vector3(0, 2, 0));
                _hpBarImages[index].fillAmount = enemyHealth / enemyMaxHealth;
                _hpBarText[index].text = enemyHealth.ToString() + "/" + enemyMaxHealth.ToString();
            }
        }
    }

    public void ClearHpBars(int level, Level _level)
    {
        _hpBars.Clear();
        _hpBarImages.Clear();
        _hpBarText.Clear();
    }

}


