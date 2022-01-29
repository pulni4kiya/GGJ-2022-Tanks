using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnekVisuals : MonoBehaviour
{
	public GameObject head;
	public GameObject body;

	public void ShowHead(Vector3 direction) {
		this.body.SetActive(false);
		this.head.SetActive(true);
		this.head.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
	}

	public void ShowBody() {
		this.body.SetActive(true);
		this.head.SetActive(false);
	}
}
