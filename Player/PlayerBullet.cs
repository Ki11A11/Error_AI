using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {


    public float Speed = 50f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(this.transform.forward * Speed * Time.deltaTime);
	}


    private void OnTriggerEnter(Collider other)
    {
        
    }


}
