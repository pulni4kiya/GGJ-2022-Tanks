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
        if (spawnOnStart && GameManager.Instance.IsMaster) {
            SpawnRandomAmount();
        }
    }

    public void SpawnRandomAmount() {
        int randomAmount = Mathf.RoundToInt(Random.Range(minPercentageToSpawn, maxPercentageToSpawn) * spawners.Count);
        SpawnPickups(randomAmount);
    }

    private void SpawnPickups(int amount) {
        Debug.Assert(amount < spawners.Count, $"Attempting to spawn more pickups than the available spawners.", this);

        List<PickupSpawner> shuffledSpawners = new List<PickupSpawner>(spawners);
        shuffledSpawners.Shuffle();

        for (int i = 0; i < amount; i++) {
            if (shuffledSpawners[i].IsPickupNotYetCollected == false) {
                shuffledSpawners[i].Spawn();
                shuffledSpawners[i].OnPickupCollected += OnPickupCollected;
            }
        }
    }

    private void OnPickupCollected() {
        if (GetAreAllPickupsCollected()) {
            SpawnRandomAmount();
        }
    }

    private bool GetAreAllPickupsCollected() {
        for (int i = 0; i < spawners.Count; i++) {
            if (spawners[i].IsPickupNotYetCollected == true) {
                return false;
            }
        }

        return true;
    }

    private void OnDestroy() {
        for (int i = 0; i < spawners.Count; i++) {
            spawners[i].OnPickupCollected -= OnPickupCollected;
        }
    }
}
