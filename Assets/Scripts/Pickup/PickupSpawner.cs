using Photon.Pun;
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
        if (spawnOnStart && GameManager.Instance.IsMaster) {
            Spawn();
        }
    }

    public void Spawn() {
        if (spawnedPickup == false) {
            spawnedPickup = GameManager.Instance.InstantiateObject(prefabToSpawn.gameObject, myTransform.position, myTransform.rotation).GetComponent<FoodPickup>();
        }

        if (PhotonNetwork.InLobby) {
            spawnedPickup.photonView.RPC("Appear", RpcTarget.All);
        } else {
            spawnedPickup.Appear();
        }

        spawnedPickup.OnPickupCollected += ReceiveOnPickupCollected;
        isPickupNotYetCollected = true;
    }

    public void ReceiveOnPickupCollected() {
        isPickupNotYetCollected = false;
        OnPickupCollected?.Invoke();
    }

    private void OnDestroy() {
        if (spawnedPickup) {
            spawnedPickup.OnPickupCollected -= ReceiveOnPickupCollected;
        }
    }
}
