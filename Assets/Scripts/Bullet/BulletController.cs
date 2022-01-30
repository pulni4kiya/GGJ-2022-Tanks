using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class BulletController : MonoBehaviourPun {
    public float fadeTime;
	public float speed = 5f;
	public int initialSegments = 3;
	public bool controllable = false;
	public GameObject snakeSegmentPrefab;

	private TrailRenderer trail;

	private int segmentsCount;

	private bool isDead = false;

	private new Rigidbody rigidbody;

	private Vector3 headPosition;
	private Vector3 moveDirection;

	private List<SegmentState> segments = new List<SegmentState>(20);
	private List<Vector3> path = new List<Vector3>();
	private int segmentsToSpawn;

	private float timeSinceLastMove = 0f;

	private MeshCollider collider;
	private Mesh colliderMesh;
	private Vector3[] trailPositions = new Vector3[1000];

	private float Size {
		get {
			return this.snakeSegmentPrefab.GetComponentInChildren<Renderer>().bounds.size.x;
		}
	}

	private void Start() {
		this.collider = this.GetComponentInChildren<MeshCollider>();
		this.colliderMesh = new Mesh();
		this.collider.sharedMesh = this.colliderMesh;

		this.collider.transform.parent = null;
		this.collider.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

		this.rigidbody = this.GetComponent<Rigidbody>();
		this.trail = this.GetComponent<TrailRenderer>();
		this.segmentsToSpawn = this.initialSegments;
		this.segmentsCount = this.initialSegments;

		if (controllable) {
			GameManager.Instance.playerInputs.Player.MoveSnake.performed += this.OnMoveSnakeInput;
		}
    }

	private void Update() {
		this.trail.time = this.segmentsCount * 0.5f / this.speed;

		if (!controllable || GameManager.Instance.GameEnded) {
			return;
		}

		if (isDead) {
			this.FadeToDeath();
			return;
		}

		//this.UpdateSegmentPositions();
	}

	private void FixedUpdate() {
		this.UpdateMeshCollider();

		if (!controllable || GameManager.Instance.GameEnded) {
			return;
		}

		if (isDead) {
			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.angularVelocity = Vector3.zero;
			return;
		}

		if (this.moveDirection != Vector3.zero) {
			this.rigidbody.velocity = this.moveDirection.normalized * this.speed;
			this.rigidbody.MoveRotation(Quaternion.LookRotation(this.moveDirection, Vector3.up));
		}

		//this.UpdateSegmentPositions2();
	}

	private void OnDestroy() {
		if (this.collider) {
			GameObject.Destroy(this.collider.gameObject);
		}

		if (controllable) {
			GameManager.Instance.playerInputs.Player.MoveSnake.performed -= this.OnMoveSnakeInput;
		}
	}

	private void UpdateMeshCollider() {
		var count = this.trail.GetPositions(trailPositions);
		if (count > 6) {
			this.GenerateMesh(this.colliderMesh, trailPositions, 0, count - 4, -1f);
			this.collider.sharedMesh = this.colliderMesh;
		}
	}

	private void UpdateSegmentPositions2() {

	}

	public void NetExtend(int segmentCount) {
		if (PhotonNetwork.InRoom) {
			photonView.RPC("Extend", RpcTarget.All, new object[] { segmentCount });
		} else {
			Extend(segmentCount);
		}
	}

	[PunRPC]
	public void Extend(int segmentCount) {
		this.segmentsToSpawn += segmentCount;
		this.segmentsCount += segmentCount;
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
			tank.NetTakeDamage(damage, collisionLocation);
		}

		this.isDead = true;
		this.fadeTime = this.trail.time;
	}

	private void OnTriggerEnter(Collider other) {
		if (!controllable) {
			return;
		}

		var pickup = other.GetComponentInParent<IPickup>();
		if (pickup != null) {
			pickup.ApplyPickup(this);
		}
	}

	private void GenerateMesh(Mesh mesh, Vector3[] vertices, int start, int count, float yOffset) {
		mesh.Clear();

		var verts = new List<Vector3>();
		for (int i = start; i < start + count; i++) {
			verts.Add(vertices[i]);
			verts.Add(vertices[i] + Vector3.up * yOffset);
		}
		mesh.SetVertices(verts);

		var tris = new List<int>();
		var max = verts.Count - 2;
		for (int i = 0; i < max; i += 2) {
			tris.Add((i + 0) % verts.Count);
			tris.Add((i + 1) % verts.Count);
			tris.Add((i + 2) % verts.Count);
			tris.Add((i + 1) % verts.Count);
			tris.Add((i + 2) % verts.Count);
			tris.Add((i + 3) % verts.Count);
			tris.Add((i + 0) % verts.Count);
			tris.Add((i + 2) % verts.Count);
			tris.Add((i + 1) % verts.Count);
			tris.Add((i + 1) % verts.Count);
			tris.Add((i + 3) % verts.Count);
			tris.Add((i + 2) % verts.Count);
		}
		mesh.SetTriangles(tris, 0);

		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
	}

	private struct SegmentState {
		public Transform transform;
		public SnekVisuals visuals;
	}
}
