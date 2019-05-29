using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySoldier : MonoBehaviour
{
    public float _upward;
    public float _left;
    public int MaxHp = 100;
    public int MaxSheild = 100;
    public int m_Attack = 15;
    public float DetectRadius = 30f; //发现玩家的范围
    public float MaxAngleCanSee = 80f; //发现玩家的角度
    public float Speed = 1f; //移动速度
    public float PatrolTurnSpeed = 150f; //巡逻时战立的转体速度，用三角函数实现所以诡异
    public float PatrolTurnRange = 2f; //巡逻站立范围
    public float LookTurnSpeed = 4f; //其他时候的转体速度

    public float AttackSym = 8f; //degree高于此值切换为攻击
    public float IdleSym = 0f; //低于切赋闲
    public float AlertSym = 4f; //高于则警觉

    public float PatrolTimer = 5f; //路点巡逻中途停顿时间
    private float m_PatrolTimer; //实时保存剩余停顿时间

    public float AttackColddown = 3f; //攻击冷却
    private float AttackTimer; //实时保存当前攻击冷却
    private bool is_Attacked;

    public float ReloadColddown = 3f;
    private float ReloadTimer;

    public int BulletAmount = 7;
    private int n_Bullet;

    public float Degree; //警觉度
    private float theta; //左右摆动当前角度

    public Vector3 PlayerLastPosition; //玩家最后位置，调用detect更新
    private Vector3 OrigPosition; //原始位置
    private Quaternion OrigTurn; //原始角度

    public State m_State = State.Idle;

    private int n_Hp;
    private int n_Sheild;

    private NavMeshAgent m_navagent;
    private GameObject m_player;
    public GameObject m_assistant;
    public GameObject Laser; //子弹
    public GameObject DeadExplode;

    public Transform[] PatrolSpot; //路点
    private int n_Spot; //目标路点

    public bool is_Dead;

    public LayerMask mask;

    public enum State
    {
        Idle,
        Alert,
        Patrol,
        Attack,
        Ensure,
        Default
    }

    // Use this for initialization
    void Start()
    {
        n_Hp = MaxHp;
        n_Sheild = MaxSheild;

        AttackTimer = AttackColddown;
        ReloadTimer = ReloadColddown;
        n_Bullet = 0;
        theta = 0f;
        Degree = IdleSym;
        n_Spot = 0;
        OrigPosition = transform.position;
        OrigTurn = transform.rotation;
        m_PatrolTimer = PatrolTimer;

        is_Dead = false;
        is_Attacked = false;
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").gameObject;
        m_navagent = this.gameObject.GetComponent<NavMeshAgent>();
        m_navagent.speed = Speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(transform.position, transform.forward * 5f);
        //Debug.DrawLine(transform.position, m_player.transform.position);
        if (is_Dead)
        {
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Degree = 0;
            m_State = State.Idle;
            SendMessageUpwards("FavorChange", -1);
            Destroy(this.transform.parent.gameObject);
        }


        switch (m_State)
        {
            case State.Idle:
            {
                Idle();

                if (DetectPlayer())
                {
                    //应该预置一个警觉值
                    m_State = State.Ensure;
                }

                break;
            }

            case State.Ensure:
            {
                if (DetectPlayer())
                {
                    m_navagent.SetDestination(transform.position);
                    LookAtPlayer();
                    Degree += Time.deltaTime * 5f;
                    if (Degree >= AttackSym)
                    {
                        m_State = State.Attack;
                    }
                }
                else
                {
                    m_navagent.SetDestination(PlayerLastPosition);
                    Degree -= Time.deltaTime;
                    if (Degree <= IdleSym)
                    {
                        m_State = State.Patrol;
                    }

                    if (Degree >= AlertSym)
                    {
                        m_State = State.Alert;
                    }
                }

                break;
            }
            case State.Alert:
            {
//                    print("巡逻兵" + transform.name + "alert");
                m_navagent.SetDestination(PlayerLastPosition);
                if (m_navagent.remainingDistance < m_navagent.stoppingDistance + 0.1f)
                {
                    LookAround();
                }

                if (DetectPlayer())
                {
                    LookAtPlayer();
                    Degree += Time.deltaTime * 10f;
                    if (Degree >= AttackSym || is_Attacked)
                    {
                        Degree = AttackSym;
                        m_State = State.Attack;
                    }
                }
                else
                {
                    Degree -= Time.deltaTime;
                    if (Degree <= IdleSym)
                    {
                        m_State = State.Patrol;
                    }
                }

                break;
            }
            case State.Attack:
            {
                is_Attacked = true;
                Attack();
                if (!DetectPlayer())
                {
                    m_State = State.Alert;
                }

                break;
            }

            case State.Patrol:
            {
                Patrol();

                if (DetectPlayer())
                {
//                        print("true");
                    m_State = State.Ensure;
                }

                break;
            }
        }
    }

    bool DetectPlayer() //在范围和角度内检测玩家，可以检测中间障碍
    {
        float angle = Vector3.Angle(transform.forward, m_player.transform.position - transform.position);
        float dis = Vector3.Distance(m_player.transform.position, transform.position);
        if (dis < DetectRadius && angle < MaxAngleCanSee)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, m_player.transform.position - transform.position, out hit, 6000f,
                mask.value))
            {
                if (hit.collider.gameObject.tag == "MainCharacter")
                {
                    PlayerLastPosition = m_player.transform.position;
                    return true;
                }
            }
        }

        return false;
    }

    void LookAtPlayer() //转向玩家
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(m_player.transform.position - transform.position), Time.deltaTime * LookTurnSpeed);
    }

    void Attack() //实例化激光攻击
    {
        LookAtPlayer();

        if (ReloadTimer > 0)
        {
            ReloadTimer -= Time.deltaTime;
        }
        else
        {
            if (AttackTimer > 0)
            {
                AttackTimer -= Time.deltaTime;
            }
            else
            {
                n_Bullet++;
                if (n_Bullet > BulletAmount)
                {
                    n_Bullet = 0;
                    ReloadTimer = ReloadColddown;
                }

                GameObject m_Laser = Instantiate(Laser, transform.position + Vector3.up * 0.5f + transform.forward * 2,
                    transform.rotation) as GameObject;
                _left = Random.Range(-1.5f, 1.5f);
                m_Laser.transform.LookAt(m_player.transform.position + Vector3.up * _upward + Vector3.left * _left);

                m_Laser.GetComponent<Rigidbody>().AddForce(m_Laser.transform.forward * 2000f);
                //m_Laser.GetComponent<ProjectileScript>().impactNormal = .normal;
                m_Laser.SetActive(true);

                //print("attack");
                AttackTimer = AttackColddown;
            }
        }


        if (Vector3.Distance(transform.position, m_player.transform.position) > DetectRadius / 2) //战斗时不能离玩家太远或太近
        {
            if (this.isActiveAndEnabled)
                m_navagent.SetDestination(PlayerLastPosition);
        }
        else if (Vector3.Distance(transform.position, m_player.transform.position) < DetectRadius / 3)
        {
            if (this.isActiveAndEnabled)
                m_navagent.SetDestination(transform.position);
        }
    }

    void Patrol() //路点巡逻
    {
        if (PatrolSpot[n_Spot] == null)
        {
            m_State = State.Idle;
            return;
        }

        m_navagent.SetDestination(PatrolSpot[n_Spot].position);

        if (Vector3.Distance(PatrolSpot[n_Spot].position, transform.position) <= 1f)
        {
            if (m_PatrolTimer > 0)
            {
                m_PatrolTimer -= Time.deltaTime;
                LookAround();
                return;
            }
            else
            {
                m_PatrolTimer = PatrolTimer;
            }

            n_Spot++;
            n_Spot = n_Spot % (PatrolSpot.Length);
            theta = 0f;
        }

        //print(n_Spot);
    }

    void LookAround() //左右摆头
    {
        theta += Time.deltaTime;

        transform.Rotate(Vector3.down * Mathf.Sin(PatrolTurnSpeed * theta * Time.deltaTime) * PatrolTurnRange,
            Space.World);
    }

    void Idle()
    {
        theta = 0; //用于转体的角度归零
        Degree = 0; //警觉度归零
        if (Vector3.Distance(transform.position, OrigPosition) > 0.5f) //回到初始位置
        {
            m_navagent.enabled = true;
            m_navagent.SetDestination(OrigPosition);
        }
        else
        {
            transform.rotation =
                Quaternion.Slerp(transform.rotation, OrigTurn, Time.deltaTime * LookTurnSpeed); //旋转回原来角度
        }
    }


    public void GetDamage(int Damage)
    {
        m_State = State.Attack;
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