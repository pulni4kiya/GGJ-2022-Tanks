using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] Vector2 scaleMinMax;
    [SerializeField] Vector2 zMinMax;
    [SerializeField] Vector2 xMinMax;


    void Update()
    {
        this.transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < xMinMax.x){
            transform.position = new Vector3(xMinMax.y, transform.position.y, Random.Range(zMinMax.x, zMinMax.y));
            transform.localScale = Vector3.one * Random.Range(scaleMinMax.x, scaleMinMax.y);
        }

    }


    void ResetPosition(){
        //this.transform.Translate(Vector3.)
    }
}
