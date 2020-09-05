using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; // Координаты точек для спавна
    [HideInInspector] public bool[] isPointInUse; // хранит инф. об использовании точек спавна

    [SerializeField] private GameObject enemy; // префаб врага
    [SerializeField] private GameObject antiEnemy; // префаб "антикрота"
    [SerializeField] private GameObject cat, sheep, duck; //префабы объектов-бонусов
    public Transform startPointBonus;
    private GameObject[] bonuseGroup; // массив для хранения объектов-бонусов
    private GameObject[] antiEnemyGroup; // массив для хранения антикротов
    private GameObject[] enemyGroup; // массив для хранения всех врагов на сцене
    [SerializeField] private Transform enemyContainer; // Родитель(контейнер) всех врагов
    [SerializeField] private int antiEnemyQuantity; // необходимое колиечство антикротов
    [SerializeField] private int enemyQuantity; // необходимое количество врагов всего
    [SerializeField] private int enemyOnScene; // необходимое количество врагов на сцене
    [Header("Врагов не меньше")] public int minQuantity = 2;
    [Header("Врагов не больше")] public int maxQuantity = 4;
    [HideInInspector] public bool permisionForSpawn = true; // Контролирует спавн врагов
    [HideInInspector] public bool permisionForBonus = true; // Контролирует спавн бонусов
    static public SpawnController instance;
    private void Awake()
    {
        if(SpawnController.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        SpawnController.instance = this;
    }
    private void Start()
    {
        // Инициализация массива хранения состояния точек
        isPointInUse = new bool[spawnPoints.Length];
        for (int i = 0; i < isPointInUse.Length; i++)
        {
            isPointInUse[i] = false;
        }
        // Инициализация массива всех врагов на сцене
        enemyGroup = new GameObject[enemyQuantity];
        for (int i = 0; i < enemyGroup.Length; i++)
        {
            enemyGroup[i] = Instantiate(enemy, enemyContainer);
            enemyGroup[i].SetActive(false);
        }
        antiEnemyGroup = new GameObject[antiEnemyQuantity];
        for(int i = 0; i < antiEnemyGroup.Length; i++)
        {
            antiEnemyGroup[i] = Instantiate(antiEnemy, enemyContainer);
            antiEnemyGroup[i].SetActive(false);
        }
        // Инициализация объектов-бонусов
        bonuseGroup = new GameObject[9];
        for(int i = 0; i < bonuseGroup.Length; i++)
        {
            if (i < 3)
                bonuseGroup[i] = Instantiate(cat, enemyContainer);
            if(i >= 3 && i < 6)
                bonuseGroup[i] = Instantiate(sheep, enemyContainer);
            if(i >= 6)
                bonuseGroup[i] = Instantiate(duck, enemyContainer);
            bonuseGroup[i].SetActive(false);
        }
        Initialization();
    }
    private void FixedUpdate()
    {
        // Регулировка врагов на доске
        int activeQuantity = QuantityCheck(enemyGroup);
        if (activeQuantity < minQuantity)
            AddEnemy(new Vector3 (0, 0, 0));
        if (activeQuantity > maxQuantity)
            permisionForSpawn = false;
        if (activeQuantity >= minQuantity &&
            activeQuantity <= maxQuantity)
            permisionForSpawn = true;
        PointRelief();
    }
    /// <summary>
    /// Метод создания начального количества врагов
    /// </summary>
    private void Initialization()
    {
        for(int i = 0; i < enemyOnScene; i++)
        {
            AddEnemy(new Vector3(0, 0, 0));
        }
    }
    /// <summary>
    /// Добавляет врагов на карту
    /// </summary>
    public void AddEnemy(Vector3 lastPosition)
    {
        Vector3 position;
        int indexData;
        do
        {
            int index = Random.Range(0, spawnPoints.Length);
            if (isPointInUse[index])
                continue;
            if(!isPointInUse[index] &&
                lastPosition != spawnPoints[index].position)
            {
                position = spawnPoints[index].position;
                isPointInUse[index] = true;
                indexData = index;
                break;
            }
        } while (true);
        int chance = Random.Range(0, 100);
        if (chance > 20)
        {
            int randomEnemy = Random.Range(0, enemyGroup.Length);
            for (int i = randomEnemy; i < enemyGroup.Length; i++)
            {
                if (enemyGroup[i].activeSelf)
                    continue;
                if (!enemyGroup[i].activeSelf)
                {
                    enemyGroup[i].transform.position = position;
                    enemyGroup[i].SetActive(true);
                    enemyGroup[i].GetComponent<Enemy>().indexOfPosition = indexData;
                    break;
                }
            }
        }
        if (chance <= 20)
        {
            int randomAntiEnemy = Random.Range(0, antiEnemyGroup.Length);
            for (int i = randomAntiEnemy; i < antiEnemyGroup.Length; i++)
            {
                if (antiEnemyGroup[i].activeSelf)
                    continue;
                if (!antiEnemyGroup[i].activeSelf)
                {
                    antiEnemyGroup[i].transform.position = position;
                    antiEnemyGroup[i].SetActive(true);
                    antiEnemyGroup[i].GetComponent<Enemy>().indexOfPosition = indexData;
                    break;
                }
            }
        }
        if (permisionForBonus)
            if (Random.Range(0, 101) <= 10) 
               StartCoroutine(AddBonus());
    }
    /// <summary>
    /// Освобождает ранее используемую точку для спавна
    /// </summary>
    public void PointRelief()
    {
        for (int i = 0; i < isPointInUse.Length; i++)
        {
            isPointInUse[i] = false;
        }
        for (int i = 0; i < enemyGroup.Length; i++)
        {
            if (enemyGroup[i].activeSelf)
            {
                isPointInUse[enemyGroup[i].GetComponent<Enemy>().indexOfPosition] = true;
            }
        }
        for (int i = 0; i < antiEnemyGroup.Length; i++)
        {
            if (antiEnemyGroup[i].activeSelf)
            {
                isPointInUse[antiEnemyGroup[i].GetComponent<Enemy>().indexOfPosition] = true;
            }
        }
    }
    /// <summary>
    /// Проверка количества врагов на доске
    /// </summary>
    /// <param name="enemyGroup">Массив врагов</param>
    /// <returns></returns>
    public int QuantityCheck(GameObject[] enemyGroup)
    {
        int activeEnemies = 0;
        for (int i = 0; i < enemyGroup.Length; i++)
        {
            if (enemyGroup[i].activeSelf)
                activeEnemies++;
        }
        return activeEnemies;
    }
    /// <summary>
    /// Добавление нового бонус-объекта на сцену
    /// </summary>
    /// <returns></returns>
    IEnumerator AddBonus()
    {
        permisionForBonus = false;
        for (int i = 0; i < bonuseGroup.Length; i++)
        {
            int index = Random.Range(0, bonuseGroup.Length);
            if (!bonuseGroup[index].activeSelf)
            {
                bonuseGroup[index].transform.position = startPointBonus.position;
                bonuseGroup[index].SetActive(true);
                break;
            }
        }
        yield return new WaitForSeconds(2f);
        permisionForBonus = true;
    }
}
