using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun {
    public float movementSpeed = 10f;
    public GameObject cameraObject;

    private bool? _isMine;
    public bool IsMine {
        get { return _isMine == true || photonView.IsMine; }
        set { _isMine = value; }
    }

    private CharacterController character;

    // Start is called before the first frame update
    void Start() {
        character = GetComponent<CharacterController>();

        if (!IsMine) {
            Destroy(cameraObject);
        }
    }

    void FixedUpdate() {
        if (!IsMine) {
            return;
        }

        ApplyMovement();
    }

    private void ApplyMovement() {
        var rawMoveVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A)) rawMoveVector.x -= 1;
        if (Input.GetKey(KeyCode.D)) rawMoveVector.x += 1;
        if (Input.GetKey(KeyCode.W)) rawMoveVector.z += 1;
        if (Input.GetKey(KeyCode.S)) rawMoveVector.z -= 1;

        var moveVector = rawMoveVector.normalized * movementSpeed * Time.deltaTime;

        character.Move(moveVector);
    }
}
