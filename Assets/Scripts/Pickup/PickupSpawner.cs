using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform myTransform;

    [SerializeField]
    private FoodPickup prefabToSpawn;

    [SerializeField]
    private FoodPickup spawnedPickup;

    [SerializeField]
    private bool isPickupNotYetCollected;
    public bool IsPickupNotYetCollected { get { return isPickupNotYetCollected; } }

    [SerializeField]
    private bool spawnOnStart;

    public System.Action OnPickupCollected;

    private void Start() {
        if (spawnOnStart) {
            Spawn();
        }
    }

    public void Spawn() {
        if (spawnedPickup == false) {
            spawnedPickup = Instantiate(prefabToSpawn, myTransform.position, myTransform.rotation, myTransform);
        }
        
        spawnedPickup.Appear();
        spawnedPickup.OnPickupCollected += ReceiveOnPickupCollected;
        isPickupNotYetCollected = true;
    }

    public void ReceiveOnPickupCollected() {
        isPickupNotYetCollected = false;
        OnPickupCollected?.Invoke();
    }

    private void OnDestroy() {
        spawnedPickup.OnPickupCollected -= ReceiveOnPickupCollected;
    }
}
