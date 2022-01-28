using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TankController : MonoBehaviourPun {
    public GameObject projectilePrefab;
    public Transform projectileSpawnLocation;
    public float projectileVelocity = 700f;

    public PlayerActionEvents actionEvents;

    private bool? _isMine;
    public bool IsMine {
        get { return _isMine == true || photonView.IsMine; }
        set { _isMine = value; }
    }

    private Rigidbody rigidBody;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();

        actionEvents.OnFireInput += Shoot;
    }

    void OnDestroy() {
        actionEvents.OnFireInput -= Shoot;
    }

    private void Shoot() {
        var projectile = GameManager.Instance.InstantiateObject(
            projectilePrefab,
            projectileSpawnLocation.position,
            projectileSpawnLocation.rotation
        );

        projectile.GetComponent<Rigidbody>().AddRelativeForce(0, 0, projectileVelocity);
    }
}
