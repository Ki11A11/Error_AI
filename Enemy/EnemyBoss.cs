using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBoss : MonoBehaviour
{
    public enum PostureState
    {
        Stand,
        Fold,
        Unfold,
        Lie,
        Stretch,
        Contract,
    }

    public enum AttackState
    {
        Stop,
        Laser,
        Shoot,
        Blade
    }

    public enum MoveState
    {
        Idle,
        Watch,
        Catch,
        Reset
    }

    public int MaxHp;
    public int MaxSheild;
    private int n_Hp;
    private int n_Sheild;

    public float TurnSpeed = 100f;
    public float TurnMyselfSpeed = 300f;
    public float MoveSpeed;
    public float UpwardOffset;
    public float DownOffset = 3.6f;


    public GameObject[] m_Laser;
    public GameObject m_Bullet;
    public GameObject[] m_Blade;
    public GameObject DeadExplode;


    private GameObject m_player;

    private Vector3 Orig_position;
    private Quaternion Orig_rotation;

    //public PostureState m_Posture = PostureState.Stand;
    public AttackState m_Attack = AttackState.Stop;
    //public MoveState m_Move = MoveState.Idle;


    public int ShootNum = 20;
    private int ShootCount;
    public float ShootColddown = 0.1f;
    private float ShootColdTimer;
    public float ShootGap = 3f;
    private float ShootGapTimer;


    public float _left;
    public float _upward;
    public float StretchLengthBlade = 6;
    public float StretchLengthLaser = 4;
    private bool is_stretch;
    private bool is_stand;
    private bool is_dead;

    public float StretchTime = 3f;
    private float StretchTimer;

    public bool KillAll;

    // Use this for initialization
    void Start()
    {
        Orig_position = transform.position;
        Orig_rotation = transform.rotation;

        ShootCount = 0;
        ShootColdTimer = 0;
        ShootGapTimer = 0;

        n_Hp = MaxHp;
        n_Sheild = MaxSheild;

        is_stretch = false;
        is_stand = true;
        is_dead = false;
        KillAll = false;
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").gameObject;
        StretchTimer = StretchTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (n_Hp < 500)
            m_Attack = AttackState.Blade;
        else if (n_Sheild < 500)
            m_Attack = AttackState.Laser;
        else if (n_Sheild < 1000)
            m_Attack = AttackState.Shoot;

        if (is_dead)
        {
            Instantiate(DeadExplode, transform.position, transform.rotation).SetActive(true);
            Instantiate(DeadExplode, transform.position, transform.rotation).SetActive(true);
            Instantiate(DeadExplode, transform.position, transform.rotation).SetActive(true);
            Destroy(this.transform.gameObject);
        }

        switch (m_Attack)
        {
            case AttackState.Stop:
            {
                LookAtPlayer();
                break;
            }
            case AttackState.Laser:
            {
                if (Stretch(StretchLengthLaser))
                    is_stretch = true;
                if (!is_stand && Stand())
                    is_stand = true;


                Watch();
                TurnMySelf();


                Fold();
                for (int i = 0; i < m_Laser.Length; i++)
                {
                    m_Laser[i].gameObject.SetActive(true);
                    m_Laser[i].transform.rotation = transform.rotation;
                }

                float angle = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z),
                    new Vector3((m_player.transform.position - transform.position).x, 0,
                        (m_player.transform.position - transform.position).z));

                if (ShootGapTimer < 0 && angle < 20)
                {
                    if (ShootColdTimer < 0)
                    {
                        ShootColdTimer = ShootColddown;
                        ShootCount++;
                        if (ShootCount >= ShootNum)
                        {
                            ShootGapTimer = ShootGap;
                            ShootCount = 0;
                        }

                        GameObject m_Laser = Instantiate(m_Bullet, transform.position + transform.forward * 1,
                            transform.rotation) as GameObject;
                        _left = Random.Range(-1.5f, 1.5f);
                        m_Laser.transform.LookAt(m_player.transform.position + Vector3.up * _upward +
                                                 Vector3.left * _left);
                        m_Laser.SetActive(true);
                        m_Laser.GetComponent<Rigidbody>().AddForce(m_Laser.transform.forward * 2000f);
                    }
                    else
                    {
                        ShootColdTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    ShootGapTimer -= Time.deltaTime;
                }

                break;
            }
            case AttackState.Shoot:
            {
                Watch(); //y
                Stand(); //xz
                UnFold();
                Contract();
                Front();
                float angle = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z),
                    new Vector3((m_player.transform.position - transform.position).x, 0,
                        (m_player.transform.position - transform.position).z));

                if (ShootGapTimer < 0 && angle < 20)
                {
                    if (ShootColdTimer < 0)
                    {
                        ShootColdTimer = ShootColddown;
                        ShootCount++;
                        if (ShootCount >= ShootNum)
                        {
                            ShootGapTimer = ShootGap;
                            ShootCount = 0;
                        }

                        GameObject m_Laser = Instantiate(m_Bullet, transform.position + transform.forward * 1,
                            transform.rotation) as GameObject;
                        _left = Random.Range(-1.5f, 1.5f);
                        m_Laser.transform.LookAt(m_player.transform.position + Vector3.up * _upward +
                                                 Vector3.left * _left);
                        m_Laser.SetActive(true);
                        m_Laser.GetComponent<Rigidbody>().AddForce(m_Laser.transform.forward * 2000f);
                    }
                    else
                    {
                        ShootColdTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    ShootGapTimer -= Time.deltaTime;
                }

                break;
            }
            case AttackState.Blade:
            {
                UnFold();
                Lie();
                //Catch();
                is_stand = false;
                for (int i = 0; i < m_Laser.Length; i++)
                {
                    m_Laser[i].gameObject.SetActive(true);
                    m_Laser[i].transform.rotation = transform.rotation;
                }

                if (!is_stretch)
                {
                    if (Stretch(StretchLengthBlade))
                    {
                        StretchTimer -= Time.deltaTime;
                    }


                    if (StretchTimer < 0)
                    {
                        is_stretch = true;
                        StretchTimer = StretchTime;
                    }
                }
                else
                {
                    if (Contract())
                        is_stretch = false;
                }

                //Catch();
                TurnMySelf();

                if (ShootGapTimer < 0)
                {
                    if (ShootColdTimer < 0)
                    {
                        ShootColdTimer = ShootColddown;
                        ShootCount++;
                        if (ShootCount >= ShootNum)
                        {
                            ShootGapTimer = ShootGap;
                            ShootCount = 0;
                        }

                        GameObject m_Laser =
                            Instantiate(m_Bullet, transform.position + Vector3.up * 2,
                                transform.rotation) as GameObject;
                        _left = Random.Range(-1.5f, 1.5f);
                        m_Laser.transform.LookAt(m_player.transform.position + Vector3.up * _upward +
                                                 Vector3.left * _left);
                        m_Laser.SetActive(true);
                        m_Laser.GetComponent<Rigidbody>().AddForce(m_Laser.transform.forward * 2000f);
                    }
                    else
                    {
                        ShootColdTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    ShootGapTimer -= Time.deltaTime;
                }

                break;
            }
        }

        #region useless

        //switch (m_Move)
        //{
        //    case MoveState.Idle:
        //        {
        //            break;
        //        }
        //    case MoveState.Watch:
        //        {
        //            LookAtPlayer();
        //            break;
        //        }
        //    case MoveState.Catch:
        //        {
        //            MoveTo(new Vector3(m_player.transform.position.x, m_player.transform.position.y + UpwardOffset, m_player.transform.position.z));
        //            break;
        //        }
        //    case MoveState.Reset:
        //        {
        //            MoveTo(Orig_position);
        //            break;
        //        }
        //}

        //switch (m_Posture)
        //{
        //    case PostureState.Stand:
        //        {
        //            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Orig_position.y, transform.position.z), Time.deltaTime);
        //            TurnTo(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z));
        //            break;
        //        }
        //    case PostureState.Fold:
        //        {
        //            //m_Blade[0].transform.localEulerAngles = new Vector3(-90, 0, 0);
        //            //m_Blade[1].transform.localEulerAngles = new Vector3(-270, 0, 180);
        //            //m_Blade[2].transform.localEulerAngles = new Vector3(-180, 90, 270);
        //            //m_Blade[3].transform.localEulerAngles = new Vector3(-180, -90, 90);

        //            m_Blade[0].transform.localRotation = Quaternion.Lerp(m_Blade[0].transform.localRotation, Quaternion.Euler(new Vector3(-90, 0, 0)), Time.deltaTime);
        //            m_Blade[1].transform.localRotation = Quaternion.Lerp(m_Blade[1].transform.localRotation, Quaternion.Euler(new Vector3(-270, 0, 180)), Time.deltaTime);
        //            m_Blade[2].transform.localRotation = Quaternion.Lerp(m_Blade[2].transform.localRotation, Quaternion.Euler(new Vector3(-180, 90, 270)), Time.deltaTime);
        //            m_Blade[3].transform.localRotation = Quaternion.Lerp(m_Blade[3].transform.localRotation, Quaternion.Euler(new Vector3(-180, -90, 90)), Time.deltaTime);

        //            break;
        //        }
        //    case PostureState.Unfold:
        //        {
        //            m_Blade[0].transform.localRotation = Quaternion.Lerp(m_Blade[0].transform.localRotation, Quaternion.Euler(new Vector3(-180, 0, 0)), Time.deltaTime);
        //            m_Blade[1].transform.localRotation = Quaternion.Lerp(m_Blade[1].transform.localRotation, Quaternion.Euler(new Vector3(-180, 0, 180)), Time.deltaTime);
        //            m_Blade[2].transform.localRotation = Quaternion.Lerp(m_Blade[2].transform.localRotation, Quaternion.Euler(new Vector3(-180, 0, 270)), Time.deltaTime);
        //            m_Blade[3].transform.localRotation = Quaternion.Lerp(m_Blade[3].transform.localRotation, Quaternion.Euler(new Vector3(-180, 0, 90)), Time.deltaTime);

        //            break;
        //        }
        //    case PostureState.Lie:
        //        {
        //            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Orig_position.y - DownOffset, transform.position.z), Time.deltaTime);
        //            TurnTo(new Vector3(90, 0, 0));
        //            print(m_Blade[0].transform.localPosition);
        //            break;
        //        }
        //    case PostureState.Stretch:
        //        {
        //            m_Blade[0].transform.localPosition = Vector3.Lerp(m_Blade[0].transform.localPosition, new Vector3(0, 4, 0), Time.deltaTime);
        //            m_Blade[1].transform.localPosition = Vector3.Lerp(m_Blade[1].transform.localPosition, new Vector3(0, -4, 0), Time.deltaTime);
        //            m_Blade[2].transform.localPosition = Vector3.Lerp(m_Blade[2].transform.localPosition, new Vector3(-4, 0, 0), Time.deltaTime);
        //            m_Blade[3].transform.localPosition = Vector3.Lerp(m_Blade[3].transform.localPosition, new Vector3(4, 0, 0), Time.deltaTime);

        //            break;
        //        }
        //    case PostureState.Contract:
        //        {
        //            m_Blade[0].transform.localPosition = Vector3.Lerp(m_Blade[0].transform.localPosition, new Vector3(0, 1.24f, 0), Time.deltaTime);
        //            m_Blade[1].transform.localPosition = Vector3.Lerp(m_Blade[1].transform.localPosition, new Vector3(0, -1.24f, 0), Time.deltaTime);
        //            m_Blade[2].transform.localPosition = Vector3.Lerp(m_Blade[2].transform.localPosition, new Vector3(-1.24f, 0, 0), Time.deltaTime);
        //            m_Blade[3].transform.localPosition = Vector3.Lerp(m_Blade[3].transform.localPosition, new Vector3(1.24f, 0, 0), Time.deltaTime);

        //            break;
        //        }
        //}

        #endregion
    }


    private void LookAtPlayer()
    {
        float turn_y = Quaternion
            .FromToRotation(this.transform.forward, m_player.transform.position - transform.position).eulerAngles.y;
        if (turn_y > 1f)
        {
            float angle;
            if (turn_y > 180)
                angle = -1 * TurnSpeed;
            else
                angle = 1 * TurnSpeed;
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, angle * Time.deltaTime, 0);
        }

        //Debug.DrawRay(transform.position,)(m_player.transform.position - transform.position).x,0,(m_player.transform.position - transform.position).z)
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3((m_player.transform.position - transform.position).x,0,(m_player.transform.position - transform.position).z)), Time.deltaTime * TurnSpeed);
    }

    private void TurnMySelf()
    {
        this.transform.Rotate(Vector3.forward * TurnMyselfSpeed * Time.deltaTime, Space.Self);
    }

    private bool MoveTo(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) > 0.1f)
        {
            this.transform.position = Vector3.Lerp(transform.position, target, MoveSpeed * Time.deltaTime);
            return false;
        }
        else
        {
            transform.position = target;
            return true;
        }
    }

    private bool TurnTo(Vector3 target)
    {
        if (Vector3.Distance(transform.eulerAngles, target) > 0.1f)
        {
            this.transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, target, Time.deltaTime);
            return false;
        }
        else
        {
            transform.eulerAngles = target;
            return true;
        }
    }

    private bool Front()
    {
        if (TurnTo(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0)))
            return true;
        else
            return false;
    }

    private bool Stand()
    {
        Vector3 target = new Vector3(transform.position.x, Orig_position.y, transform.position.z);
        bool move = true;
        if (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
            move = false;
        }
        else
        {
            transform.position = target;
        }

        if (TurnTo(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z)) && move)
            return true;
        else
            return false;
    }

    private bool Lie()
    {
        Vector3 target = new Vector3(transform.position.x, Orig_position.y - DownOffset, transform.position.z);
        bool move = true;
        if (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
            move = false;
        }
        else
        {
            transform.position = target;
        }

        if (TurnTo(new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z)) && move)
            return true;
        else
            return false;
    }


    private bool Stretch(float StretchLength)
    {
        if (Vector3.Distance(m_Blade[0].transform.localPosition, new Vector3(0, StretchLength, 0)) > 0.05f)
        {
            m_Blade[0].transform.localPosition = Vector3.Lerp(m_Blade[0].transform.localPosition,
                new Vector3(0, StretchLength, 0), Time.deltaTime);
            m_Blade[1].transform.localPosition = Vector3.Lerp(m_Blade[1].transform.localPosition,
                new Vector3(0, -StretchLength, 0), Time.deltaTime);
            m_Blade[2].transform.localPosition = Vector3.Lerp(m_Blade[2].transform.localPosition,
                new Vector3(-StretchLength, 0, 0), Time.deltaTime);
            m_Blade[3].transform.localPosition = Vector3.Lerp(m_Blade[3].transform.localPosition,
                new Vector3(StretchLength, 0, 0), Time.deltaTime);
            return false;
        }
        else
        {
            m_Blade[0].transform.localPosition = new Vector3(0, StretchLength, 0);
            m_Blade[1].transform.localPosition = new Vector3(0, -StretchLength, 0);
            m_Blade[2].transform.localPosition = new Vector3(-StretchLength, 0, 0);
            m_Blade[3].transform.localPosition = new Vector3(StretchLength, 0, 0);
            return true;
        }
    }

    private bool Contract()
    {
        if (Vector3.Distance(m_Blade[0].transform.localPosition, new Vector3(0, 1.24f, 0)) > 0.05f)
        {
            m_Blade[0].transform.localPosition = Vector3.Lerp(m_Blade[0].transform.localPosition,
                new Vector3(0, 1.24f, 0), Time.deltaTime);
            m_Blade[1].transform.localPosition = Vector3.Lerp(m_Blade[1].transform.localPosition,
                new Vector3(0, -1.24f, 0), Time.deltaTime);
            m_Blade[2].transform.localPosition = Vector3.Lerp(m_Blade[2].transform.localPosition,
                new Vector3(-1.24f, 0, 0), Time.deltaTime);
            m_Blade[3].transform.localPosition = Vector3.Lerp(m_Blade[3].transform.localPosition,
                new Vector3(1.24f, 0, 0), Time.deltaTime);
            return false;
        }
        else
        {
            m_Blade[0].transform.localPosition = new Vector3(0, 1.24f, 0);
            m_Blade[1].transform.localPosition = new Vector3(0, -1.24f, 0);
            m_Blade[2].transform.localPosition = new Vector3(-1.24f, 0, 0);
            m_Blade[3].transform.localPosition = new Vector3(1.24f, 0, 0);
            return true;
        }
    }

    private bool Fold()
    {
        if (Vector3.Distance(m_Blade[0].transform.localEulerAngles, new Vector3(-90, 0, 0)) > 0.1f)
        {
            m_Blade[0].transform.localRotation = Quaternion.Lerp(m_Blade[0].transform.localRotation,
                Quaternion.Euler(new Vector3(-90, 0, 0)), Time.deltaTime);
            m_Blade[1].transform.localRotation = Quaternion.Lerp(m_Blade[1].transform.localRotation,
                Quaternion.Euler(new Vector3(-270, 0, 180)), Time.deltaTime);
            m_Blade[2].transform.localRotation = Quaternion.Lerp(m_Blade[2].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, 90, 270)), Time.deltaTime);
            m_Blade[3].transform.localRotation = Quaternion.Lerp(m_Blade[3].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, -90, 90)), Time.deltaTime);
            return false;
        }
        else
        {
            m_Blade[0].transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            m_Blade[1].transform.localRotation = Quaternion.Euler(new Vector3(-270, 0, 180));
            m_Blade[2].transform.localRotation = Quaternion.Euler(new Vector3(-180, 90, 270));
            m_Blade[3].transform.localRotation = Quaternion.Euler(new Vector3(-180, -90, 90));
            return true;
        }
    }

    private bool UnFold()
    {
        if (Vector3.Distance(m_Blade[0].transform.localEulerAngles, new Vector3(-90, 0, 0)) > 0.1f)
        {
            m_Blade[0].transform.localRotation = Quaternion.Lerp(m_Blade[0].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, 0, 0)), Time.deltaTime);
            m_Blade[1].transform.localRotation = Quaternion.Lerp(m_Blade[1].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, 0, 180)), Time.deltaTime);
            m_Blade[2].transform.localRotation = Quaternion.Lerp(m_Blade[2].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, 0, 270)), Time.deltaTime);
            m_Blade[3].transform.localRotation = Quaternion.Lerp(m_Blade[3].transform.localRotation,
                Quaternion.Euler(new Vector3(-180, 0, 90)), Time.deltaTime);
            return false;
        }
        else
        {
            m_Blade[0].transform.localRotation = Quaternion.Euler(new Vector3(-180, 0, 0));
            m_Blade[1].transform.localRotation = Quaternion.Euler(new Vector3(-180, 0, 180));
            m_Blade[2].transform.localRotation = Quaternion.Euler(new Vector3(-180, 0, 270));
            m_Blade[3].transform.localRotation = Quaternion.Euler(new Vector3(-180, 0, 90));
            return true;
        }
    }

    private void Watch()
    {
        LookAtPlayer();
    }

    private void Catch()
    {
        MoveTo(new Vector3(m_player.transform.position.x, m_player.transform.position.y + UpwardOffset,
            m_player.transform.position.z));
    }

    private void Reset()
    {
        MoveTo(Orig_position);
    }

    public void GetDamage(int Damage)
    {
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
            is_dead = true;
        }
    }
}