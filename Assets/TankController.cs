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
		bulletController.Init(projectileSpawnLocation.position, this.transform.forward);
    }

    public void ControlledTakeDamage(float damage, Vector3 hitLocation) {
        photonView.RPC("TakeDamage", RpcTarget.All, new object[] { damage, hitLocation });
    }

    [PunRPC]
	public void TakeDamage(float damage, Vector3 hitLocation) {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health > 0) {
            SpawnBoom(hitLocation, 0.5f);
        } else if (health <= 0) {
            SpawnBoom(explosionSpawnLocation.position, 1f);

            // TODO: We dead bro: destroy tank, declare game end
        }
	}

	public void Heal(float healAmount) {
		health = Mathf.Clamp(health + healAmount, 0, maxHealth);
	}

	private void SpawnBoom(Vector3 position, float scale) {
        var explosion = Instantiate(
            explosionPrefab,
            position,
            Quaternion.identity,
            parent: transform
        );
        explosion.transform.localScale *= scale;
        Destroy(explosion, 5);
    }

    // TODO: Networking for this?
	private void OnTriggerEnter(Collider other) {
		var pickup = other.GetComponentInParent<IPickup>();
		if (pickup != null) {
			pickup.ApplyPickup(this);
		}
	}
}
