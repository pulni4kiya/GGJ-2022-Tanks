using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {
        get;
        private set;
    }

    public Camera mainCamera;

    public GameObject playerPrefab;
    public Transform spawnLocation1;
    public Transform spawnLocation2;

	public PlayerInputs playerInputs;

    public GameObject victoryScreen;
    public GameObject defeatScreen;

    public List<Button> mainMenuButtons;

    private bool singlePlayer = true;

    public bool GameEnded {
        get;
        private set;
    } = false;

    public bool IsMaster {
        get {
            return (!PhotonNetwork.InRoom && singlePlayer) || PhotonNetwork.IsMasterClient;
        }
    }

	void Awake() {
		if (!PhotonNetwork.InRoom && !singlePlayer) {
			Debug.LogError("We can't be in this scene if we're not a room");
		}

		Instance = this;

		playerInputs = new PlayerInputs();
		playerInputs.Player.MoveSnake.Enable();
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

        foreach (var button in mainMenuButtons) {
            button.onClick.AddListener(BackToMenu);
        }
    }

    private void BackToMenu() {
        SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update() {

    }

    public GameObject InstantiateObject(
        GameObject original,
        Vector3 position,
        Quaternion rotation
    ) {
        if (PhotonNetwork.InRoom) {
            return PhotonNetwork.Instantiate(original.name, position, rotation);
        } else if (singlePlayer) {
            return Instantiate(original, position, rotation);
        } else {
            Debug.LogError("Could not instantiate object as we're not connected to the server");
            return null;
        }
    }

    public void DestroyObject(GameObject gameObject) {
        if (PhotonNetwork.InRoom) {
            PhotonNetwork.Destroy(gameObject);
        } else if (singlePlayer) {
            Destroy(gameObject);
        } else {
            Debug.LogError("Could not destroy object as we're not connected to the server");
        }
    }

    public void DeclareVictory() {
        GameEnded = true;

        victoryScreen.SetActive(true);
        defeatScreen.SetActive(false);

        PhotonNetwork.LeaveRoom();
    }

    public void DeclareDefeat() {
        GameEnded = true;

        defeatScreen.SetActive(true);
        victoryScreen.SetActive(false);

        PhotonNetwork.LeaveRoom();
    }
}
