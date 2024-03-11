using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HpBarManager : MonoBehaviour
{

    [SerializeField] private PooledObject _hpBar;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private List<PooledObject> _hpBars = new List<PooledObject>();
    [SerializeField] private List<Image> _hpBarImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _hpBarText = new List<TextMeshProUGUI>();

    private void OnEnable()
    {
        WaitForEnemyManager();
    }

    private void OnDisable()
    {
        if (_enemyManager != null)
        {
            GameManager.Instance.OnLevelComplete -= ClearHpBars;
        }
    }

    private IEnumerator WaitForEnemyManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.EnemyManager != null);
        GameManager.Instance.EnemyManager = _enemyManager;
        GameManager.Instance.OnLevelStart -= ClearHpBars;
    }

    void Update()
    {
        if (_enemyManager != null)
        {
            for (int i = 0; i < _enemyManager.activeEnemies.Count; i++)
            {
                if (_hpBars.Count < _enemyManager.activeEnemies.Count)
                {
                    AddHpBar(_enemyManager.activeEnemies[i]);
                }
                else
                {
                    UpdateHpBar(_enemyManager.activeEnemies[i]);
                }
            }
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
    }

    private void UpdateHpBar(GameObject enemyObject)
    {
        for (int i = 0; i < _hpBars.Count; i++)
        {
            if (_hpBars[i] && i < _enemyManager.activeEnemies.Count && i < _hpBarImages.Count && i < _hpBarText.Count)
            {
                float enemyHealth = _enemyManager.ProvideEnemyFloatData(enemyObject, "Health");
                float enemyMaxHealth = _enemyManager.ProvideEnemyFloatData(enemyObject, "MaxHealth");

                if (enemyHealth <= 0)
                {
                    // Deactivate the health bar if the enemy's health is 0
                    _hpBars[i].gameObject.SetActive(false);
                }
                else
                {
                    _hpBars[i].transform.position = Camera.main.WorldToScreenPoint(_enemyManager.activeEnemies[i].transform.position + new Vector3(0, 2, 0));
                    _hpBarImages[i].fillAmount = enemyHealth / enemyMaxHealth;
                    _hpBarText[i].text = enemyHealth.ToString() + "/" + enemyMaxHealth.ToString();
                }
            }
        }
    }
            //     PooledObjectManager.Instance.ReturnToPool(_hpBars[i]);

    public void ClearHpBars(int level, Level _level)
    {
        _hpBars.Clear();
        _hpBarImages.Clear();
        _hpBarText.Clear();
    }

}


