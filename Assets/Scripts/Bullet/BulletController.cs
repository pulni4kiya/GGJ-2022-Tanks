using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float fadeTime;
	public float speed = 5f;
	public int initialSegments = 3;
	public bool controllable = false;
	public GameObject snakeSegmentPrefab;

	private int segmentsCount;

	private bool isDead = false;

	private new Rigidbody rigidbody;

	private Vector3 headPosition;
	private Vector3 moveDirection;

	private List<SegmentState> segments = new List<SegmentState>(20);
	private int segmentsToSpawn;

	private float timeSinceLastMove = 0f;

	private float Size {
		get {
			return this.snakeSegmentPrefab.GetComponentInChildren<Renderer>().bounds.size.x;
		}
	}

	private void Start() {
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.segmentsToSpawn = this.initialSegments;

		if (controllable) {
			GameManager.Instance.playerInputs.Player.MoveSnake.performed += this.OnMoveSnakeInput;
		}
    }

	private void Update() {
		if (!controllable || GameManager.Instance.GameEnded) {
			return;
		}

		if (isDead) {
			this.FadeToDeath();
			return;
		}

		this.UpdateSegmentPositions();
	}

	public void Extend(int segmentCount) {
		this.segmentsToSpawn += segmentCount;
	}

	private void FadeToDeath() {
		this.fadeTime -= Time.deltaTime;
		if (this.fadeTime < 0f) {
			this.DestroySnake();
		}
	}

	private void DestroySnake() {
		foreach (var s in this.segments) {
			GameManager.Instance.DestroyObject(s.transform.gameObject);
		}
		GameManager.Instance.DestroyObject(this.gameObject);
	}

	private void UpdateSegmentPositions() {
		var moveTime = this.Size / this.speed;

		this.timeSinceLastMove += Time.deltaTime;
		if (this.timeSinceLastMove > moveTime) {
			this.timeSinceLastMove -= moveTime;

			this.headPosition += this.moveDirection.normalized * this.Size;

			if (this.segments.Count > 0) {
				this.segments.Last().visuals.ShowBody();
			}
			if (this.segmentsToSpawn > 0) {
				this.segmentsToSpawn--;
				this.SpawnSegment();
			} else {

				var seg = this.segments[0];
				this.segments.RemoveAt(0);
				this.segments.Add(seg);
				seg.transform.position = this.headPosition;
			}
			this.segments.Last().visuals.ShowHead(this.moveDirection);
		}
	}

	private void SpawnSegment() {
		var segment = GameManager.Instance.InstantiateObject(
			this.snakeSegmentPrefab,
			this.headPosition,
			Quaternion.identity
		);
		this.segmentsCount++;
		this.segments.Add(new SegmentState() {
			transform = segment.transform,
			visuals = segment.GetComponent<SnekVisuals>()
		});
	}

	private void FixedUpdate() {
		if (!controllable || GameManager.Instance.GameEnded) {
			return;
		}

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

	public void Init(Vector3 position, Vector3 forward) {
		this.moveDirection = forward;
		this.headPosition = position;
	}

	private void OnCollisionEnter(Collision collision) {
		if (isDead || !controllable) {
			return;
		}

		var collisionLocation = collision.contacts[0].point;
		var tank = collision.collider.GetComponent<TankController>();

		if (tank != null) {
			var damage = Mathf.Pow(this.segmentsCount, 1.5f);
			tank.ControlledTakeDamage(damage, collisionLocation);
		}

		this.isDead = true;
	}

	private void OnTriggerEnter(Collider other) {
		var pickup = other.GetComponentInParent<IPickup>();
		if (pickup != null) {
			pickup.ApplyPickup(this);
		}
	}

	private struct SegmentState {
		public Transform transform;
		public SnekVisuals visuals;
	}
}
