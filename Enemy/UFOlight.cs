using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Characters.FirstPerson;

public class UFOlight : MonoBehaviour {

    public Light m_light1;
    public Light m_light2;
    
    public float DamageTimer = 0.25f;
    private float m_DamageTimer;
    public float AttackEnergy = 25f;

    private FirstPersonControllerNew m_player;


    private float orig_intensity;

    private bool is_Trigger;
    
	// Use this for initialization
	void Start () {
        orig_intensity = m_light1.intensity;
        is_Trigger = false;
        m_DamageTimer = DamageTimer;

        
	}

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<FirstPersonControllerNew>();
    }

    // Update is called once per frame
    void Update () {
        if (m_light1.intensity > orig_intensity && !is_Trigger)
        {
            m_light1.intensity -= Time.deltaTime * 2f;
            m_light2.intensity -= Time.deltaTime * 2f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.transform.name);
        if(other.tag == "MainCharacter")
        {
            //真测到玩家

            is_Trigger = true;
            SendMessageUpwards("SetState", 1);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        

        if (other.tag == "MainCharacter")
        {
            //侦测到玩家
            if (m_light1.intensity < orig_intensity + 10f)
            {
                m_light1.intensity += Time.deltaTime * 5f;
                m_light2.intensity += Time.deltaTime * 5f;
            }
            SendMessageUpwards("SetState",1);
        
        if(m_DamageTimer>0)
            {
                m_DamageTimer -= Time.deltaTime;
            }
        else
            {
                m_DamageTimer = DamageTimer;
                m_player.GetDamage(Mathf.RoundToInt((int)m_light1.intensity));
                m_player.LoseEnergy((int)AttackEnergy);

            }
            

            is_Trigger = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCharacter")
        {
            //玩家跑路了
            is_Trigger = false;
            SendMessageUpwards("SetState", 2);
        }
        
    }
}
