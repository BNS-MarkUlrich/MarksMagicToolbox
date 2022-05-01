using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private int wave; // [SerializeField] = debugging
    [SerializeField] private List<WaveConfiguration> waveConfigurations = new List<WaveConfiguration>();

    [SerializeField] private UnityEvent onStartWave = new UnityEvent();

    private bool canStartWave;
    private bool canCheckCreatures;

    private int totalCreatureAmount;
    private List<Health> deadCreatures = new List<Health>();
    
    public int GetWave => wave;

    private void Start()
    {
        StartCoroutine(StarWaveTimer(wave));
    }

    public void StartWave(int wave)
    {
        SpawnCreatures(wave);
        onStartWave?.Invoke();
        canStartWave = false;
    }

    private void SpawnCreatures(int wave)
    {
        var currentWave = waveConfigurations[wave];
        print("Starting: " + currentWave.waveName + "...");

        foreach (var creature in currentWave.creatures)
        {
            var currentCreature = creature.creature;
            var creatureAmount = creature.creatureAmount;
            
            var isCreatureNull = currentCreature == null;
            if (isCreatureNull) continue;
            
            currentCreature.AddTag("Enemy");
            
            totalCreatureAmount += creatureAmount;

            var spawnLocations = creature.spawnLocations;
            var spawnLocationCount = spawnLocations.Count;

            var hasSpawnLocations = spawnLocationCount > 0;
            if (!hasSpawnLocations)
            {
                Debug.LogError(creature.creature.name + " has no spawn locations! Setting to default...");
                AddDefaultSpawn(creature);
            }

            for (var i = 0; i < creatureAmount; i++)
            {
                var randomSpawnIndex = Random.Range(0, spawnLocationCount);
                Instantiate(currentCreature, spawnLocations[randomSpawnIndex].position, Quaternion.identity);
            }
        }
        canCheckCreatures = true;
    }

    private void AddDefaultSpawn(WaveCreatures creature)
    {
        var hasDefaultSpawn = FindObjectOfType<GameObject>().name == "DefaultSpawn";
        if (hasDefaultSpawn) return;
        
        var defaultSpawn = new GameObject();
        defaultSpawn.transform.position = Vector3.zero;
        defaultSpawn.name = "DefaultSpawn";

        creature.spawnLocations.Add(defaultSpawn.transform);
    }
    
    private void FixedUpdate()
    {
        if (canCheckCreatures) CheckCreatures();
        
        if (!canStartWave) return;
        StartWave(wave);
    }

    private IEnumerator StarWaveTimer(int wave)
    {
        var currentWave = waveConfigurations[wave];
        var waveTimer = currentWave.waveTimer;
        yield return new WaitForSeconds(waveTimer);
        
        canStartWave = true;
        deadCreatures.Clear();
    }

    private void CheckCreatures()
    {
        var allObjects = FindObjectsOfType<Health>();
        
        var perishedCreatures = allObjects.Where(creature => creature.gameObject.HasTag("Enemy") && creature.gameObject.HasTag("Dead")).ToList();
        
        foreach (var creature in perishedCreatures)
        {
            var containsCreature = deadCreatures.Contains(creature);
            if (containsCreature) continue;
            deadCreatures.Add(creature);
        }

        var endWave = totalCreatureAmount - deadCreatures.Count <= 0;
        if(!endWave) return;
        WaveReset();
    }

    private void WaveReset()
    {
        canCheckCreatures = false;
        canStartWave = false;

        if (LastWave()) return;
        ++wave;
        
        totalCreatureAmount = 0;
        StartCoroutine(StarWaveTimer(wave));
    }

    private bool LastWave()
    {
        var totalWaves = waveConfigurations.Count;

        var isLastWave = wave >= --totalWaves;
        return isLastWave;
    }
}

[System.Serializable]
public struct WaveConfiguration
{
    public string waveName;
    
    [Header("Wave Settings")]
    public float waveTimer;
    public List<WaveCreatures> creatures;
}

[System.Serializable]
public struct WaveCreatures
{
    public GameObject creature;
    public int creatureAmount;
    public List<Transform> spawnLocations;
}