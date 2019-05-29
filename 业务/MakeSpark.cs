using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSpark : MonoBehaviour {
    public GameObject Spark;
    // Use this for initialization
    public float Timer = 0.1f;
    private float ColdDown;
	void Start () {
        ColdDown = Timer;
	}
	
	// Update is called once per frame
	void Update () {
		if(ColdDown>0)
        {
            ColdDown -= Time.deltaTime;
        }
        else
        {
            Instantiate(Spark, transform.position, transform.rotation).SetActive(true);
            ColdDown = Timer;
        }
	}
}
