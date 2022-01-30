using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;

public class TankController : MonoBehaviourPun {
    public GameObject projectilePrefab;
    public Transform projectileSpawnLocation;

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

	private GameObject lastProjectile;
	private Color color;

	void Start() {
        rigidBody = GetComponent<Rigidbody>();

        if (!IsMine) {
            GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>().enabled = false;
            return;
        }

        actionEvents.OnFireInput += Shoot;

		var tankMat = GameManager.Instance.GetTankMaterial();
		foreach (var rend in this.GetComponentsInChildren<Renderer>()) {
			rend.material = tankMat;
		}
		this.color = tankMat.color;
    }

    void OnDestroy() {
        actionEvents.OnFireInput -= Shoot;
    }

    private void Shoot() {
        if (GameManager.Instance.GameEnded) {
            return;
        }

		if (this.lastProjectile != null) {
			return;
		}

        var projectile = GameManager.Instance.InstantiateObject(
            projectilePrefab,
            projectileSpawnLocation.position,
            projectileSpawnLocation.rotation
        );

		var bulletController = projectile.GetComponent<BulletController>();
		bulletController.controllable = true;
		bulletController.Init(projectileSpawnLocation.position, this.transform.forward, this.color);

		this.lastProjectile = projectile;
    }

    public void NetTakeDamage(float damage, Vector3 hitLocation) {
        if (PhotonNetwork.InRoom) {
            photonView.RPC("TakeDamage", RpcTarget.All, new object[] { damage, hitLocation });
        } else {
            TakeDamage(damage, hitLocation);
        }
    }

    public void NetHeal(float healAmount) {
        if (PhotonNetwork.InRoom) {
            photonView.RPC("Heal", RpcTarget.All, new object[] { healAmount });
        } else {
            Heal(healAmount);
        }
    }

    [PunRPC]
	public void TakeDamage(float damage, Vector3 hitLocation) {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health > 0) {
            SpawnBoom(hitLocation, 0.5f);
        } else if (health <= 0) {
            SpawnBoom(explosionSpawnLocation.position, 1f);

            if (IsMine) {
                GameManager.Instance.DeclareDefeat();
            } else {
                GameManager.Instance.DeclareVictory();
            }
        }
	}

    [PunRPC]
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

	private void OnTriggerEnter(Collider other) {
        Debug.Log($"OnTriggerEnter tank {IsMine}");

        if (!IsMine) {
            return;
        }

		var pickup = other.GetComponentInParent<IPickup>();
		if (pickup != null) {
			pickup.ApplyPickup(this);
		}
	}
}
