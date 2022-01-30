using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NetworkGameManager : MonoBehaviourPunCallbacks {
    public GameManager gameManager;

    void Start() {
        PhotonNetwork.SerializationRate = 30;
    }

    public override void OnLeftRoom() {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) {
            SceneManager.LoadScene("Menu");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (gameManager.GameEnded) {
            return;
        }

        Debug.Log("A player has left the room");
        gameManager.DeclareVictory();
    }
}
