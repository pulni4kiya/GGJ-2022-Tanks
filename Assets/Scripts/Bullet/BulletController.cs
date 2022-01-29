using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float timeToLive;
	public float speed = 5f;
	public bool controllable = false;
	public TrailRenderer trail;

	private float size;
	private bool isDead = false;

	private new Rigidbody rigidbody;

	private Vector3 moveDirection;

	private void Start() {
		this.rigidbody = this.GetComponent<Rigidbody>();

		if (controllable) {
			GameManager.Instance.playerInputs.Player.MoveSnake.performed += this.OnMoveSnakeInput;
		}
    }

	private void Update() {
		this.timeToLive -= Time.deltaTime;
		if (this.timeToLive < 0f) {
			GameObject.Destroy(this.gameObject);
		}
	}

	private void FixedUpdate() {
		if (isDead) {
			this.rigidbody.velocity = Vector3.zero;
			return;
		}

		if (this.moveDirection != Vector3.zero) {
			this.rigidbody.velocity = this.moveDirection.normalized * this.speed;
			this.rigidbody.MoveRotation(Quaternion.LookRotation(this.moveDirection, Vector3.up));
		}
	}

	private void OnMoveSnakeInput(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
		var inputDir = obj.ReadValue<Vector2>();
		var dir = new Vector3(inputDir.x, 0f, inputDir.y);

		if (dir != -this.moveDirection) {
			this.moveDirection = dir;
		}
	}

	public void Init(Vector3 forward) {
		this.moveDirection = forward;
	}

	private void OnCollisionEnter(Collision collision) {
		if (isDead) {
			return;
		}

		var tank = collision.collider.GetComponent<TankController>();

		if (tank != null) {
			var damage = Mathf.Pow(this.size, 1.5f);
			tank.TakeDamage(damage);
		}

		this.isDead = true;
		this.timeToLive = trail.time;
	}
}
