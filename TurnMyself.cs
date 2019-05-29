using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMyself : MonoBehaviour {

    float Theta;
    public float Range = 1f;
    public float Speed = 0.5f;
    public float TurnSpeed = 10f;
	// Use this for initialization
	void Start () {
        Theta = 0;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(Vector3.up * TurnSpeed *Time.deltaTime, Space.Self);
        transform.Translate(new Vector3(0, Mathf.Cos(Theta), 0) * Time.deltaTime * Range);
        Theta += Time.deltaTime * Speed;
    }
}
