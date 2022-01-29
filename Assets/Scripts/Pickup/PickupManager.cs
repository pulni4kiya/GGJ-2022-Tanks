using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)]
    private float minPercentageToSpawn;
    [SerializeField][Range(0f, 1f)]
    private float maxPercentageToSpawn;

    [SerializeField]
    private bool spawnOnStart;

    [SerializeField]
    private List<PickupSpawner> spawners;

    private void Start() {
        if (spawnOnStart) {
            SpawnRandomAmount();
        }
    }

    public void SpawnRandomAmount() {
        int randomAmount = Mathf.RoundToInt(Random.Range(minPercentageToSpawn, maxPercentageToSpawn) * spawners.Count);
        SpawnPickups(randomAmount);
    }

    public void SpawnPickups(int amount) {
        Debug.Assert(amount < spawners.Count, $"Attempting to spawn more pickups than the available spawners.", this);

        List<PickupSpawner> shuffledSpawners = new List<PickupSpawner>(spawners);
        shuffledSpawners.Shuffle();

        for (int i = 0; i < amount; i++) {
            spawners[i].Spawn();
        }
    }
}
