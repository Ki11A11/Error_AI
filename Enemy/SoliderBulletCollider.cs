using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SoliderBulletCollider : MonoBehaviour {

    private FirstPersonControllerNew m_player;
    public int Damage = 15;
    // Use this for initialization
    void Start () {
        m_player = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<FirstPersonControllerNew>();
    }
	
	// Update is called once per frame
	void Update () {
        
	}


    //private void OnCollisionEnter(Collision collision)
    //{
    //    print("3333");
    //    print(collision.transform.name);
    //    m_player.GetDamage(Damage);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    print(collision.transform.name);
    //    m_player.GetDamage(Damage);
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    print(collision.transform.name);
    //    m_player.GetDamage(Damage);
    //}

    private void OnCollisionEnter(Collision other)
    {
        //print("巡逻兵子弹命中物体:\t" + other.transform.name);
        if (other.transform.tag == "MainCharacter")
        {
            m_player.GetDamage(Damage);
            Destroy(this.gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    print("巡逻兵子弹命中物体:\t"+other.name);
    //    if(other.tag == "MainCharacter")
    //    {
    //        m_player.GetDamage(Damage);
    //        Destroy(this.gameObject);
    //    }
    //}

}
