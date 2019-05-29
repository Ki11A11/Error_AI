using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerShootBullet : MonoBehaviour {

    public int Damage = 10;
    public float Speed = 10f;
    public float Life = 5f;

    public Vector3 TargetDirect;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(TargetDirect * Time.deltaTime * Speed,Space.World);
        Life -= Time.deltaTime;
        if (Life < 0)
            Destroy(this.gameObject);
	}

    //private void OnCollisionEnter(Collision collision)
    //{

    //    print(collision.transform.name);
    //    if (collision.transform.tag == "UFO")
    //    {

    //            collision.transform.gameObject.GetComponentInParent<EnemyUFO>().GetDamage(Damage);

    //    }
    //    else if (collision.transform.tag == "Cannon")
    //    {
    //        EnemyCannon m_cannon;
    //        m_cannon = collision.transform.gameObject.GetComponentInChildren<EnemyCannon>();
    //        if (m_cannon == null)
    //        {
    //            m_cannon = collision.transform.gameObject.GetComponentInParent<EnemyCannon>();
    //        }

    //            m_cannon.GetDamage(Damage);


    //    }
    //    else if (collision.transform.tag == "Soldier")
    //    {
    //        EnemySoldier m_soldier;
    //        m_soldier = collision.transform.gameObject.GetComponent<EnemySoldier>();
    //        if (m_soldier == null)
    //        {
    //            m_soldier = collision.transform.gameObject.GetComponentInParent<EnemySoldier>();
    //        }


    //            m_soldier.GetDamage(Damage);


    //    }
    //}


    private void OnTriggerEnter(Collider collision)
    {
        print("机枪击中：\t"+collision.transform.name);

        if (collision.transform.tag == "UFO")
        {

            collision.transform.gameObject.GetComponentInParent<EnemyUFO>().GetDamage(Damage);
            Destroy(this.gameObject);
        }
        else if (collision.transform.tag == "Cannon")
        {
            EnemyCannon m_cannon;
            m_cannon = collision.transform.gameObject.GetComponentInChildren<EnemyCannon>();
            if (m_cannon == null)
            {
                m_cannon = collision.transform.gameObject.GetComponentInParent<EnemyCannon>();
            }

            m_cannon.GetDamage(Damage);
            Destroy(this.gameObject);

        }
        else if (collision.transform.tag == "Soldier")
        {
            EnemySoldier m_soldier;
            m_soldier = collision.transform.gameObject.GetComponent<EnemySoldier>();
            if (m_soldier == null)
            {
                m_soldier = collision.transform.gameObject.GetComponentInParent<EnemySoldier>();
            }


            m_soldier.GetDamage(Damage);
            Destroy(this.gameObject);

        }
        
    }
}
