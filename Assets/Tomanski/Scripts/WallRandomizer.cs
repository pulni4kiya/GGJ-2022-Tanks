using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRandomizer : MonoBehaviour
{
    [SerializeField]
    private float randomScale = 0.2f;
    [SerializeField]
    private float randomRotation = 1.2f;
    [SerializeField]
    private float randomHeight = 0.5f;
    void Start()
    {
        this.transform.localScale = Vector3.one * Random.Range(1f, 1f + randomScale);
        this.transform.Rotate( Vector3.up * Random.Range(-randomRotation, randomRotation) );
        this.transform.Translate(Vector3.up * Random.Range(0, randomHeight));
    }
}
