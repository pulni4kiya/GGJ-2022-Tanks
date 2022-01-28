using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviourPun { // IPunObservable
    public float movementSpeed = 10f;
    public GameObject cameraObject;

    public GameObject projectilePrefab;
    public Transform projectileSpawnLocation;
    public float projectileVelocity = 700f;

    private bool? _isMine;
    public bool IsMine {
        get { return _isMine == true || photonView.IsMine; }
        set { _isMine = value; }
    }

    private Rigidbody rigidBody;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();

        if (!IsMine) {
            Destroy(cameraObject);
        }
    }

    void FixedUpdate() {
        if (!IsMine) {
            // rigidBody.position = Vector3.MoveTowards(rigidBody.position, networkPosition, Time.fixedDeltaTime);
            // rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
            return;
        }

        ApplyMovement();
    }

    void Update() {
        if (!IsMine) {
            return;
        }

        ApplyShooting();
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    //     if (stream.IsWriting) {
    //         stream.SendNext(this.rigidBody.position);
    //         stream.SendNext(this.rigidBody.rotation);
    //         stream.SendNext(this.rigidBody.velocity);
    //     } else {
    //         networkPosition = (Vector3) stream.ReceiveNext();
    //         networkRotation = (Quaternion) stream.ReceiveNext();
    //         rigidBody.velocity = (Vector3) stream.ReceiveNext();
    //
    //         float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
    //         networkPosition += (this.rigidBody.velocity * lag);
    //     }
    // }

    private void ApplyMovement() {
        var rawMoveVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A)) rawMoveVector.x -= 1;
        if (Input.GetKey(KeyCode.D)) rawMoveVector.x += 1;
        if (Input.GetKey(KeyCode.W)) rawMoveVector.z += 1;
        if (Input.GetKey(KeyCode.S)) rawMoveVector.z -= 1;

        var moveVector = rawMoveVector.normalized * movementSpeed * Time.deltaTime;

        rigidBody.MovePosition(rigidBody.position + moveVector);
    }

    private void ApplyShooting() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            var projectile = GameManager.Instance.InstantiateObject(
                projectilePrefab,
                projectileSpawnLocation.position,
                projectileSpawnLocation.rotation
            );

            projectile.GetComponent<Rigidbody>().AddRelativeForce(0, 0, projectileVelocity);
        }
    }
}
