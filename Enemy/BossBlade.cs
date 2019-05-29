using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class BossBlade : MonoBehaviour {

    public int Damage = 20;
    private FirstPersonControllerNew m_player;
    // Use this for initialization
    void Start () {
        
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<FirstPersonControllerNew>();
    }
    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "MainCharacter")
        {
            m_player.GetDamage(Damage);
            //Destroy(this.gameObject);
        }
    }
}
