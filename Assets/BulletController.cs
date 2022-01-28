using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    public float timeToLive;

    void Start() {
        Object.Destroy(gameObject, timeToLive);
    }
}
