using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviourPunCallbacks {
    public static GameManager Instance {
        get;
        private set;
    }

    public GameObject playerPrefab;
    public Transform spawnLocation1;
    public Transform spawnLocation2;

	public PlayerInputs playerInputs;

    private bool singlePlayer = true;

	void Awake() {
		if (!PhotonNetwork.InRoom && !singlePlayer) {
			Debug.LogError("We can't be in this scene if we're not a room");
		}

		Instance = this;

		playerInputs = new PlayerInputs();
		playerInputs.Player.MoveSnake.Enable();

		// DontDestroyOnLoad(this);
	}

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.InRoom) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation1.position, Quaternion.identity);
            } else {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation2.position, Quaternion.identity);
            }
        } else if (singlePlayer) {
            var player = Instantiate(playerPrefab, spawnLocation1.position, Quaternion.identity);
            player.GetComponent<TankController>().IsMine = true;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public GameObject InstantiateObject(GameObject original, Vector3 position, Quaternion rotation) {
        if (PhotonNetwork.InRoom) {
            return PhotonNetwork.Instantiate(original.name, position, rotation);
        } else if (singlePlayer) {
            return Instantiate(original, position, rotation);
        } else {
            Debug.LogError("Could not instantiate object as we're not connected to the server");
            return null;
        }
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("A player has left the room");
    }
}
