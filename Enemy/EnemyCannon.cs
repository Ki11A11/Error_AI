using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyCannon : MonoBehaviour
{
    public float DetectForward;
    public float DetectAngle = 30f;
    public float _upward;
    public float _left;
    public float getup;
    public float getfoword;

    public float DetectRedius = 50f;

    public int MaxHp = 100;
    public int MaxSheild = 100;
    public int m_Attack = 3;
    public float TurnSpeed = 50f;
    public float DetectSpeed = 10f;

    private int n_Hp;
    private int n_Sheild;

    private GameObject m_player; //玩家
    public GameObject m_assistant; //助手

    public float coldDown = 0.1f; //攻击冷却
    private float AttackTimer;

    public GameObject Bullet; //子弹
    public GameObject DeadExplode;
    public LineRenderer m_lineRenderer; //射线本体


    public Transform[] PushBullet; //炮管口

    private int LayerMask = 0 << 20; //没用
    public bool Detected; //是否发现玩家

    private Vector3 m_initial_angle; //初始时的角度，用于TurnAround
    private int sign; //旋转符号

    public bool is_Dead;

    RaycastHit hit; //保存射线信息

    Ray m_ray; //射线

    //public bool isTrigger = false;//控制显示射线
    // Use this for initialization
    void Start()
    {
        n_Hp = MaxHp;
        n_Sheild = MaxSheild;
        Detected = false;
        sign = Random.Range(0, 1) > 0.5f ? 1 : -1;

        m_initial_angle = transform.eulerAngles;
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Dead)
        {
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Instantiate(DeadExplode, transform.position, transform.rotation);
            Detected = false;
            SendMessageUpwards("FavorChange", -1);
            Destroy(this.transform.parent.gameObject);
        }


        if (Detected)
        {
            AttackTimer -= Time.deltaTime;

            LookAtPlayer();
            m_lineRenderer.transform.gameObject.SetActive(false);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, this.transform.forward, out hit);

            float angle = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z),
                new Vector3((m_player.transform.position - transform.position).x, 0,
                    (m_player.transform.position - transform.position).z));
            //print("angle:" + angle);
            if (angle < 15)
                if (AttackTimer < 0)
                {
                    GameObject m_Laser = Instantiate(Bullet,
                        transform.position + Vector3.up * getup + transform.forward * getfoword,
                        transform.rotation) as GameObject;
                    _left = Random.Range(-1.5f, 1.5f);
                    m_Laser.transform.LookAt(m_player.transform.position + Vector3.up * _upward + Vector3.left * _left);
                    m_Laser.SetActive(true);
                    m_Laser.GetComponent<Rigidbody>().AddForce(m_Laser.transform.forward * 2000f);
                    //m_Laser.GetComponent<ProjectileScript>().impactNormal = .normal;
                    AttackTimer += coldDown;
                }


            //Attack();
        }
        else
        {
            m_lineRenderer.transform.gameObject.SetActive(true);
            TurnAround();
            if (DetectPlayer())
            {
                Detected = true;
            }
        }


        Debug.DrawRay(this.transform.position, this.transform.forward * 5f);
    }

    private void LookAtPlayer() //看向玩家
    {
        //Quaternion tar = new Quaternion(0, Quaternion.LookRotation(m_player.transform.position - transform.position).y, 0, Quaternion.LookRotation(m_player.transform.position - transform.position).w);
        //transform.rotation = Quaternion.Slerp(transform.rotation, tar, Time.deltaTime * 3f);
        float turn_y = Quaternion
            .FromToRotation(this.transform.forward, m_player.transform.position - transform.position).eulerAngles.y;
        if (turn_y > 1f)
        {
            float angle;
            if (turn_y > 180)
                angle = -1 * TurnSpeed;
            else
                angle = 1 * TurnSpeed;
            transform.Rotate(new Vector3(0f, angle * Time.deltaTime, 0f));
            ;
        }
    }

    private void TurnAround() //左右循环转动
    {
        float turn_y = Quaternion.FromToRotation(this.transform.forward,
            new Vector3(Mathf.Sin(DetectAngle / 57.3f * sign + DetectForward / 57.3f), 0,
                Mathf.Cos(DetectAngle / 57.3f * sign + DetectForward / 57.3f))).eulerAngles.y;


        if (turn_y > 1f)
        {
            float angle;
            if (turn_y > 180)
                angle = -1 * DetectSpeed;
            else
                angle = 1 * DetectSpeed;

            transform.Rotate(new Vector3(0f, angle * Time.deltaTime, 0f));
        }
        else
        {
            sign = -sign;
            //print(sign);
        }
    }

    private void Attack() //实例化两个子弹
    {
        //coldDown -= Time.deltaTime;
        //if (coldDown > 0)
        //    return;
        //GameObject new_bullet1 = Instantiate(Bullet, PushBullet[0].position + transform.forward * 0.15f, PushBullet[0].rotation);
        //GameObject new_bullet2 = Instantiate(Bullet, PushBullet[1].position + transform.forward * 0.15f, PushBullet[1].rotation);
        //coldDown = 3f;
    }

    bool DetectPlayer() //发射射线，设置linerenderer长度，判断是否碰到tag为player的物体，返回bool
    {
        m_ray = new Ray(PushBullet[0].transform.position, transform.TransformDirection(Vector3.forward));
        Debug.DrawRay(PushBullet[0].transform.position, transform.TransformDirection(Vector3.forward) * 5f);


        //print(hit.transform.position);
        //print(hit.collider.gameObject.name);
        m_lineRenderer.SetPosition(0, (PushBullet[0].transform.position + PushBullet[1].transform.position) / 2);

        if (Physics.Raycast(m_ray, out hit, DetectRedius))
        {
            m_lineRenderer.SetPosition(1, hit.point);
            if (hit.transform.tag == "MainCharacter")
            {
                return true;
            }
        }
        else
        {
            m_lineRenderer.SetPosition(1,
                (PushBullet[0].transform.position + PushBullet[1].transform.position) / 2 +
                this.transform.forward * DetectRedius);
        }


        return false;
    }


    public void GetDamage(int Damage)
    {
        Detected = true;


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