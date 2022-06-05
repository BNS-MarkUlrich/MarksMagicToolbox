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
    [SerializeField] private UnityEvent onWaveEnd = new UnityEvent();

    private bool _canStartWave;
    private bool _canCheckCreatures;

    private int _totalCreatureAmount;
    private List<GameObject> _deadCreatures = new List<GameObject>();
    
    public int GetWave => wave;

    private void Start()
    {
        StartCoroutine(StarWaveTimer(wave));
    }

    public void StartWave(int wave)
    {
        SpawnCreatures(wave);
        onStartWave?.Invoke();
        _canStartWave = false;
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
            
            _totalCreatureAmount += creatureAmount;

            var spawnLocations = creature.spawnLocations;
            var spawnLocationCount = spawnLocations.Count;

            var hasSpawnLocations = spawnLocationCount > 0;
            if (!hasSpawnLocations)
            {
                Debug.LogError(creature.creature.name + " has no spawn locations! Setting to default...");
                AddDefaultSpawn(creature);
            }

            StartCoroutine(SpawnInterval(creature, currentCreature, spawnLocations, spawnLocationCount));
        }
        _canCheckCreatures = true;
    }

    private IEnumerator SpawnInterval(WaveCreatures creature, GameObject currentCreature, List<Transform> spawnLocations, int spawnLocationCount)
    {
        var creatureAmount = creature.creatureAmount;
        var spawnInterval = creature.spawnInterval;
        for (var i = 0; i < creatureAmount;)
        {
            var randomSpawnIndex = Random.Range(0, spawnLocationCount);
            Instantiate(currentCreature, spawnLocations[randomSpawnIndex].position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
            i++;
        }
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
        if (_canCheckCreatures) CheckCreatures();
        
        if (!_canStartWave) return;
        StartWave(wave);
    }

    private IEnumerator StarWaveTimer(int wave)
    {
        var currentWave = waveConfigurations[wave];
        var waveTimer = currentWave.waveTimer;
        yield return new WaitForSeconds(waveTimer);
        
        _canStartWave = true;
        _deadCreatures.Clear();
    }

    private void CheckCreatures()
    {
        var allObjects = FindObjectsOfType<GameObject>();
        
        var perishedCreatures = allObjects.Where(creature => creature.gameObject.HasTag("Enemy") && creature.gameObject.HasTag("Dead")).ToList();
        
        foreach (var creature in perishedCreatures)
        {
            var containsCreature = _deadCreatures.Contains(creature);
            if (containsCreature) continue;
            _deadCreatures.Add(creature);
        }

        var endWave = _totalCreatureAmount - _deadCreatures.Count <= 0;
        if(!endWave) return;
        WaveReset();
    }

    private void WaveReset()
    {
        onWaveEnd?.Invoke();
        _canCheckCreatures = false;
        _canStartWave = false;

        if (LastWave()) return;
        ++wave;
        
        _totalCreatureAmount = 0;
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
    public float spawnInterval;
}