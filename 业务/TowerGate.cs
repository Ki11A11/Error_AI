using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGate : MonoBehaviour {

    private bool[] GateCode;
    public float[] GateUp;
    public float[] GateDown;
    public GameObject[] Gate;

    public GameObject m_Camera;
	// Use this for initialization
	void Start () {
        GateCode = new bool[3];
        GateCode[0] = true;
        GateCode[1] = true;
        GateCode[2] = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GateCode[0] = !GateCode[0];
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GateCode[1] = !GateCode[1];
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GateCode[2] = !GateCode[2];
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            m_Camera.SetActive(false);
            this.gameObject.SetActive(false);
        }


        if (GateCode[0])
        {
            Gate[0].transform.localPosition = Vector3.Lerp(Gate[0].transform.localPosition, new Vector3(Gate[0].transform.localPosition.x, GateDown[0], Gate[0].transform.localPosition.z), Time.deltaTime/3);

        }
        else if (!GateCode[0])
        {
            Gate[0].transform.localPosition = Vector3.Lerp(Gate[0].transform.localPosition, new Vector3(Gate[0].transform.localPosition.x, GateUp[0], Gate[0].transform.localPosition.z), Time.deltaTime/3);
        }

        if (GateCode[1])
        {
            Gate[1].transform.localPosition = Vector3.Lerp(Gate[1].transform.localPosition, new Vector3(Gate[1].transform.localPosition.x, GateDown[1], Gate[1].transform.localPosition.z), Time.deltaTime);
        }
        else if (!GateCode[1])
        {
            Gate[1].transform.localPosition = Vector3.Lerp(Gate[1].transform.localPosition, new Vector3(Gate[1].transform.localPosition.x, GateUp[1], Gate[1].transform.localPosition.z), Time.deltaTime);
        }

        if (GateCode[2])
        {
            Gate[2].transform.localPosition = Vector3.Lerp(Gate[2].transform.localPosition, new Vector3(Gate[2].transform.localPosition.x, GateDown[2], Gate[2].transform.localPosition.z), Time.deltaTime);
        }
        else if (!GateCode[2])
        {
            Gate[2].transform.localPosition = Vector3.Lerp(Gate[2].transform.localPosition, new Vector3(Gate[2].transform.localPosition.x, GateUp[2], Gate[2].transform.localPosition.z), Time.deltaTime);
        }



    }
}
