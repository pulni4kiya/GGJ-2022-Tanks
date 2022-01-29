using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float timeToLive;
	public float speed = 5f;
	public bool controllable = false;

	private new Rigidbody rigidbody;

	private Vector3 moveDirection;

	private void Start() {
		this.rigidbody = this.GetComponent<Rigidbody>();

		if (controllable) {
			GameManager.Instance.playerInputs.Player.MoveSnake.performed += this.OnMoveSnakeInput;
		}

        UnityEngine.Object.Destroy(gameObject, timeToLive);
    }

	private void FixedUpdate() {
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
}
