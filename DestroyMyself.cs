using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMyself : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("_DestroyMyself", 5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void _DestroyMyself()
    {
        Destroy(this.gameObject);
    }
}
