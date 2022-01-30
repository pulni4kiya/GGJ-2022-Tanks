using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks {
    public static GameManager Instance {
        get;
        private set;
    }

    public Camera mainCamera;

    public GameObject playerPrefab;
    public Transform spawnLocation1;
    public Transform spawnLocation2;

	public PlayerInputs playerInputs;

    public GameObject endScreen;
    public GameObject winText;
    public GameObject defeatText;

    public Button mainMenuButton;
    public Button newGameButton;

	public Material wallMaterial;
	public Material groundMaterial;

	public List<Color> colors;

	public Material[] tankMaterials;
	private int tankIndex;

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

		this.colors.Shuffle();

		this.wallMaterial.color = this.colors[0];
	}

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            newGameButton.gameObject.SetActive(true);
            newGameButton.onClick.AddListener(RestartLevel);
        } else {
            newGameButton.gameObject.SetActive(false);
        }

        if (PhotonNetwork.InRoom) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation1.position, Quaternion.identity);
            } else {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnLocation2.position, Quaternion.identity);
            }
        } else if (singlePlayer) {
            var player = Instantiate(playerPrefab, spawnLocation1.position, Quaternion.identity);
            player.GetComponent<TankController>().IsMine = true;

            player = Instantiate(playerPrefab, spawnLocation2.position, Quaternion.identity);
            player.GetComponent<TankController>().IsMine = true;
        }

        mainMenuButton.onClick.AddListener(GoToMenu);
    }

    void OnDestroy() {
        mainMenuButton.onClick.RemoveListener(GoToMenu);

        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) {
            newGameButton.onClick.RemoveListener(RestartLevel);
        }

        Instance = null;
    }

    // Update is called once per frame
    void Update() {

    }

    private void GoToMenu() {
        SceneManager.LoadScene("Menu");
    }

    private void RestartLevel() {
        PhotonNetwork.LoadLevel("RestartLoading");
    }

	public Material GetTankMaterial() {
		return this.tankMaterials[this.tankIndex++];
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

        endScreen.SetActive(true);
        winText.SetActive(true);
        defeatText.SetActive(false);
    }

    public void DeclareDefeat() {
        GameEnded = true;

        endScreen.SetActive(true);
        winText.SetActive(false);
        defeatText.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        newGameButton.gameObject.SetActive(false);
    }
}
