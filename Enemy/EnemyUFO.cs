using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUFO : MonoBehaviour {

    public int MaxHp = 100;
    public int MaxSheild = 100;
    public int m_Attack = 15;

    private int n_Hp;
    private int n_Sheild;

    private float Theta;

    public float ColdDown = 1.0f;

    public float MoveSpeed = 3f;

    private float orig_y;

    public GameObject DeadExplode;
    private GameObject m_player;
    public GameObject m_assistant;
    //public GameObject Bullet;
    //public Transform PushBullet;

    public Transform[] PatrolSpot;
    public int n_Spot;
    private float m_PatrolTimer;
    public float PatrolTimer;

    public State m_state;
    public float Degree;
    public float AttackSym = 8f;

    private bool is_Dead;


    public enum State
    {
        Patrol,
        Attack,
        Alert
    }



    private Vector3 PlayerLastPosition;

    // Use this for initialization
    void Start()
    {
        Degree = 0f;
        n_Hp = MaxHp;
        n_Sheild = MaxSheild;
        m_state = State.Patrol;

        orig_y = this.transform.position.y;
        
        is_Dead = false;
    }
    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit DownRay;
        //if (Physics.Raycast(transform.position + Vector3.down*1.5f, Vector3.down, out DownRay))
        //{
        //    print("distance:" + DownRay.distance);
            
        //}


        if (!m_player)
        {
            m_player = GameObject.FindGameObjectWithTag("Player").gameObject;
        }
            

        if (is_Dead)
        {
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Degree = 0;
            m_state = State.Patrol;
            SendMessageUpwards("FavorChange", -1);
            Destroy(this.transform.gameObject);
        }
        //upAndDown(1f, 0.5f);
        //CatchPlayer();
        //Attack();
        //Patrol();
        switch (m_state)
        {
            case State.Patrol:
                {
                    
                    Patrol();
                    break;
                }
            case State.Alert:
                {
                    Degree -= Time.deltaTime;
                    if(Degree<=0)
                    {
                        m_state = State.Patrol;
                    }
                    Alert();
                    break;
                }
            case State.Attack:
                {
                    Degree = AttackSym;
                    Attack();
                    break;
                }
        }

    }


    void upAndDown(float Speed, float Range)
    {
        transform.Translate(new Vector3(0, Mathf.Cos(Theta), 0) * Time.deltaTime * Range);
        Theta += Time.deltaTime * Speed;

    }

    void Attack()
    {
        CatchPlayer();

        //todo
        
    }

    void Patrol()
    {
        

        //transform.position = Vector3.Slerp(transform.position, new Vector3(PatrolSpot[n_Spot].position.x, orig_y, PatrolSpot[n_Spot].position.z), Time.deltaTime * 0.2f);
        if(PatrolSpot[0] == null)
        {
            //print("UFO don't have patrol spot!");
            return;
        }
        if (Vector2.Distance( new Vector2(PatrolSpot[n_Spot].position.x, PatrolSpot[n_Spot].position.z), new Vector2( transform.position.x,transform.position.z)) <= 1f)
        {

            if (m_PatrolTimer > 0)
            {
                m_PatrolTimer -= Time.deltaTime;

                

                return;
            }
            else
            {
                m_PatrolTimer = PatrolTimer;
            }
            n_Spot++;
            n_Spot = n_Spot % (PatrolSpot.Length);
            
        }
        else
        {
            transform.Translate(Vector3.Normalize(new Vector3(PatrolSpot[n_Spot].position.x, orig_y, PatrolSpot[n_Spot].position.z) - transform.position) * Time.deltaTime * MoveSpeed);
        }

        
    }

    void Alert()
    {
        transform.Translate(Vector3.Normalize(new Vector3(PlayerLastPosition.x, orig_y+2f, PlayerLastPosition.z) - transform.position) * Time.deltaTime * MoveSpeed);
    }

    void CatchPlayer()
    {

        //transform.position = Vector3.Slerp(transform.position, new Vector3(m_player.transform.position.x,orig_y,m_player.transform.position.z), Time.deltaTime * 0.2f);
        transform.Translate(Vector3.Normalize(new Vector3(m_player.transform.position.x, orig_y+2f, m_player.transform.position.z) - transform.position) * Time.deltaTime * MoveSpeed);
        PlayerLastPosition = m_player.transform.position;
    }

    public void SetState(int index)
    {
        switch(index)
        {
            case 0:
                {
                    m_state = State.Patrol;
                    break;
                }
            case 1:
                {
                    m_state = State.Attack;
                    break;
                }
            case 2:
                {
                    m_state = State.Alert;
                    break;
                }

                    
        }
    }

    public void GetDamage(int Damage)
    {
        m_state = State.Attack;
        PlayerLastPosition = m_player.transform.position;
        Degree = 50f;

        if (n_Sheild >= Damage)
        {
            n_Sheild -= Damage;
        }
        else
        {
            n_Hp -= (Damage - n_Sheild);
            n_Sheild = 0;
        }

        if (n_Hp <= 0)
        {
            is_Dead = true;
        }
    }
}
