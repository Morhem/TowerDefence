using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    float PhaseTimerSeconds;
    float timerLeft;
    [System.NonSerialized]
    public bool Activated = false;
    [SerializeField]
    WaveScriptableObject[] WavesSO;
    float currentCooldown = 0f;
    int WaveIndex = 0;
    int absoluteWaveNumber = 0;
    Plane plane;
    [System.NonSerialized]
    public bool Started = false;
    bool bossSpawned = false;
    [SerializeField]
    GameObject PhaseBossPrefab;


    TMPro.TMP_Text timerText;
    const float spawnPadding = 150;//В пикселях

    public void Initialize()
    {
        var filter = GameObject.Find("Plane").GetComponent<MeshFilter>();
        Vector3 normal = Vector3.zero;
        if (filter && filter.mesh.normals.Length > 0)
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);
        plane = new Plane(normal, transform.position);

        timerText = GameObject.Find("TimerText").GetComponent<TMPro.TMP_Text>();

        timerLeft = PhaseTimerSeconds;
        WaveIndex = 0;
        int i = 0;
    }

    Vector3 GetRandomPosition()
    {
        // 0 - 3 Top, right, bottom, left

        var side = Random.Range(0, 4);
        Vector3 screenPosition;

        var width = Screen.width;
        var height = Screen.height;


        switch (side)
        {
            case 0: // Top
                screenPosition = new Vector2(Random.Range(0, width), 0 - spawnPadding);
                break;
            case 1: // Right
                screenPosition = new Vector2(width + spawnPadding, Random.Range(0, height));
                break;
            case 2: // Bottom
                screenPosition = new Vector2(Random.Range(0, width), height + spawnPadding);
                break;
            case 3: // Left
                screenPosition = new Vector2(0 - spawnPadding, Random.Range(0, height));
                break;
            default:
                screenPosition = Vector3.zero;
                break;
        }
        Vector3 position = ScreenToPlane(screenPosition);

        NavMeshHit hit;
        NavMesh.SamplePosition(position, out hit, 5, 1);
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }

    Vector3 ScreenToPlane(Vector3 screenPoint)
    {
        var ray = Camera.main.ScreenPointToRay(screenPoint);

        float enter;
        var test = plane.Raycast(ray, out enter);

        var final = ray.GetPoint(enter);
        return final;
    }

    void SpawnMonster(GameObject prefab)
    {
        var monsterInstance = GameObject.Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
        monsterInstance.name = $"{prefab.name} {Random.Range(0, 999)}";
        var mscripts = monsterInstance.GetComponentsInChildren<Monster>();

        foreach (var ms in mscripts)
        {
            MissionController.main.Monsters.Add(ms);
            ms.HP = (int)(ms.HP * MissionController.main.MultiplierHP);
            ms.Gold = (int)(ms.Gold * MissionController.main.MultiplierXP);
        }
    }

    void Update()
    {
        if (!Activated)
        {
            return;
        }
        if (MissionController.main.Pause)
            return;
        timerLeft -= Time.deltaTime;
        if (timerLeft < 0)
            timerLeft = 0;
        timerText.text = ((int)timerLeft).ToString();

        if (timerLeft <= 0)
        {
            if (!MissionController.main.Monsters.Any())
            {
                Activated = false;
                MissionController.main.PhaseComplete();
            }
            return;
        }

        if (!MissionController.main.Monsters.Any() || currentCooldown <= 0f && WaveIndex < WavesSO.Length)
        {
            var wave = WavesSO[WaveIndex];

            MissionController.main.SetWaveCounter(absoluteWaveNumber++);
            
            foreach (var monPrefab in wave.Monsters)
            {
                SpawnMonster(monPrefab);
            }

            if (!bossSpawned && WaveIndex == WavesSO.Length - 1) //Пора спавнить боссов
            {
                if (PhaseBossPrefab != null)
                    SpawnMonster(PhaseBossPrefab);
                if (MissionController.main.AdditionalMinibossPrefab != null)
                    for (int i = 0; i < MissionController.main.AdditionalMinibosses; i++)
                    {
                        SpawnMonster(MissionController.main.AdditionalMinibossPrefab);
                    }
                wave.RepeatsDone = 10000;
                bossSpawned = true;
            }

            wave.RepeatsDone++;
            if (wave.RepeatsDone >= wave.Repeats)
            {
                WaveIndex++;
            }
            currentCooldown = wave.Delay;
            if (WaveIndex >= WavesSO.Length)
                WaveIndex = WavesSO.Length - 1;
        }
        currentCooldown -= Time.deltaTime;
    }
}
