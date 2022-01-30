using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RestartLevelScreen : MonoBehaviour {
    void Start() {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) {
            StartCoroutine(DelayStartGame());
        }
    }

    IEnumerator DelayStartGame() {
        yield return new WaitForSeconds(3);

        PhotonNetwork.LoadLevel("Game");
    }
}
