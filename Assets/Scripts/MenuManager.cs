using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks {
    public Text statusLabel;
    public Button connectButton;

    // Start is called before the first frame update
    void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;

        connectButton.enabled = false;
        connectButton.onClick.AddListener(TryToJoinRandomRoom);

        statusLabel.text = "Connecting to server...";

        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
        } else {
            OnConnectedToMaster();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master server");

        statusLabel.text = "Connected to server, please join room";
        connectButton.enabled = true;
    }

    private void TryToJoinRandomRoom() {
        statusLabel.text = "Joining room...";
        PhotonNetwork.JoinRandomRoom();
        connectButton.enabled = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log($"Could not join a random room: {returnCode} {message}");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogError("Could not create room");

        statusLabel.text = "Could not join or create a room :(";
        connectButton.enabled = true;
    }

    private void CreateRoom() {
        PhotonNetwork.CreateRoom(null, roomOptions: new RoomOptions {
            MaxPlayers = 2
        });
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined a room");

        var room = PhotonNetwork.CurrentRoom;

        if (room.PlayerCount == 1) {
            statusLabel.text = "In lobby. Waiting for second player";
        } else if (room.PlayerCount == 2) {
            statusLabel.text = "In lobby. Starting game...";
        } else {
            statusLabel.text = "More than two players in room";
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
