using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMyself_2 : MonoBehaviour {
    public float TurnSpeed = 40f;

    public enum axis
    {
        up,
        left,
        foward
    }
    public axis m_axis;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        switch(m_axis)
        {
            case axis.foward:
                this.transform.Rotate(Vector3.forward * TurnSpeed * Time.deltaTime, Space.Self);
                break;
            case axis.left:
                this.transform.Rotate(Vector3.left * TurnSpeed * Time.deltaTime, Space.Self);
                break;
            case axis.up:
                this.transform.Rotate(Vector3.up * TurnSpeed * Time.deltaTime, Space.Self);
                break;
        }
        
    }
}
