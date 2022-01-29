using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;

public class TankController : MonoBehaviourPun {
    public GameObject projectilePrefab;
    public Transform projectileSpawnLocation;
    public float projectileVelocity = 700f;

    public GameObject explosionPrefab;
    public Transform explosionSpawnLocation;

    public PlayerActionEvents actionEvents;

    public float maxHealth = 100f;
    public float health = 100f;

    private bool? _isMine;
    public bool IsMine {
        get { return _isMine == true || photonView.IsMine; }
        set { _isMine = value; }
    }

    private Rigidbody rigidBody;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();

        if (!IsMine) {
            GetComponent<TankMovement>().enabled = false;
            GetComponent<TankFacing>().enabled = false;
            return;
        }

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

		var bulletController = projectile.GetComponent<BulletController>();
		bulletController.controllable = true;
		bulletController.Init(this.transform.forward);
    }

	internal void TakeDamage(float damage, Vector3 hitLocation) {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health > 0) {
            SpawnBoom(hitLocation);
        } else if (health <= 0) {
            SpawnBoom(explosionSpawnLocation.position);

            // TODO: We dead bro: destroy tank, declare game end
        }
	}

    private void SpawnBoom(Vector3 position) {
        var explosion = Instantiate(
            explosionPrefab,
            position,
            Quaternion.identity,
            parent: transform
        );
        Destroy(explosion, 5);
    }
}
