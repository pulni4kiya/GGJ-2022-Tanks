using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform myTransform;

    [SerializeField]
    private GameObject spawnedPickup;

    [SerializeField]
    private bool spawnOnStart;

    private void Start() {
        if (spawnOnStart) {
            Spawn();
        }
    }

    public void Spawn() {
        GameManager.Instance.InstantiateObject(spawnedPickup, myTransform.position, myTransform.rotation);
    }
}
