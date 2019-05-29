using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class Bullet : MonoBehaviour {

    public float Life = 3f;
    public int Damage = 15;
    public float Speed = 5f;
    public Vector3 m_target;

    private FirstPersonControllerNew m_player;
    // Use this for initialization


    // Use this for initialization
    void Start () {
        m_target = Vector3.zero;
        m_player = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<FirstPersonControllerNew>();
        
    }
	
	// Update is called once per frame
	void Update () {
        Life -= Time.deltaTime;


        //
        //Move();
        transform.Translate(transform.InverseTransformDirection(this.transform.forward) * Speed * Time.deltaTime);
        if (Life <= 0f)
        {
            Destroy(this.gameObject);
        }
	}




    public void Move()
    {
        if(m_target == Vector3.zero)
        {
            
            transform.Translate(0, 0, Speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, m_target, Time.deltaTime * Speed);
        }
    }

    public void MoveTo(Vector3 go)
    {
        m_target = go;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.transform.name);
        if (collision.transform.tag == "MainCharacter")
        {
            m_player.GetDamage(Damage);
        }
            
    }


}
