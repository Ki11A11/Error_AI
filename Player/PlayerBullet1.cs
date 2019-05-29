using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet1 : MonoBehaviour {

    public int Damage = 20;

    private bool isDamaged;
	// Use this for initialization
	void Start () {
        isDamaged = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision hit)
    {
        if (isDamaged)
            return;
        print("机枪击中：\t" + hit.transform.name);

        if (hit.transform.tag == "UFO")
        {

            hit.transform.gameObject.GetComponentInParent<EnemyUFO>().GetDamage(Damage);
            isDamaged = true;
        }
        else if (hit.transform.tag == "Cannon")
        {
            EnemyCannon m_cannon;
            m_cannon = hit.transform.gameObject.GetComponentInChildren<EnemyCannon>();
            if (m_cannon == null)
            {
                m_cannon = hit.transform.gameObject.GetComponentInParent<EnemyCannon>();
            }

            m_cannon.GetDamage(Damage);
            isDamaged = true;

        }
        else if (hit.transform.tag == "Soldier")
        {
            EnemySoldier m_soldier;
            m_soldier = hit.transform.gameObject.GetComponent<EnemySoldier>();
            if (m_soldier == null)
            {
                m_soldier = hit.transform.gameObject.GetComponentInParent<EnemySoldier>();
            }


            m_soldier.GetDamage(Damage);
            isDamaged = true;

        }
        else if (hit.transform.tag == "Boss")
        {
            EnemyBoss m_boss;
            m_boss = hit.transform.gameObject.GetComponent<EnemyBoss>();
            if (m_boss == null)
            {
                m_boss = hit.transform.gameObject.GetComponentInParent<EnemyBoss>();
            }



            m_boss.GetDamage(Damage);
        }
    }
}
