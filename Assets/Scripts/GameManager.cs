using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks {
    public GameObject playerPrefab;
    public Transform spawnLocation1;
    public Transform spawnLocation2;

    private bool singlePlayer = true;

    void Awake() {
        if (!PhotonNetwork.IsConnected && !singlePlayer) {
            Debug.LogError("We can't be in this scene if we're not connected to the network");
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation1.position, Quaternion.identity);
            } else {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation2.position, Quaternion.identity);
            }
        } else if (singlePlayer) {
            var player = Instantiate(playerPrefab, spawnLocation1.position, Quaternion.identity);
            player.GetComponent<PlayerController>().IsMine = true;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("A player has left the room");
    }
}
