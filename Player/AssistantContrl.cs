using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 机器人往哪里看
/// </summary>
public enum lookState
{
    lookForward,
    lookPlayer,
    lookAround,
    lookEnemy,
};

public class AssistantContrl : MonoBehaviour
{

    private Animator m_Animator;

    public Transform m_Target;//右上角标志物

    public Transform m_Player;//玩家，实体时方块
    private Vector3 offset;//位移差值
    private Vector3 offset_r;//角度差值

    public int MaxHp = 100;//最大生命
    public int Sheild = 100;//最大护盾
    public int Attack = 15;//攻击力

    private int n_Hp;//当前生命
    private int n_Sheild;//当前护盾
    public lookState m_lookState;//当前旋转模式
//----------------------
    private float m_Timer = 2f;//瞎jb看旋转倒计时
    private bool is_turning = false;//判定是否正在瞎j旋转
    private float n_target;//旋转角度
    private int n_direct = 1;//正反转动
    private Quaternion orig_direct;//开启转动时的朝向
    private Quaternion target_direct;//xj旋转的当前方向

    private float Theta = 0f;//控制悬浮效果的角度
    public float FlowSpeed = 1.5f;//悬浮速度
    public float FlowRange = 1f;//悬浮范围


    public Quaternion _offset;
    // Use this for initialization
    void Start()
    {
        m_Animator = this.gameObject.GetComponent<Animator>();

        offset = transform.position - m_Target.position;
        offset_r = transform.eulerAngles - m_Target.eulerAngles;
        n_Hp = MaxHp;
        n_Sheild = Sheild;

        //-----------------

    }

    // Update is called once per frame
    void Update()
    {
        //AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        followTarget();
        upAndDown(FlowSpeed, FlowRange);
        

        //Vector3 theta = m_player.transform.rotation

    }


    //悬浮
    void upAndDown(float Speed,float Range)
    {
        transform.Translate(new Vector3(0, Mathf.Cos(Theta), 0) * Time.deltaTime*Range);
        Theta += Time.deltaTime * Speed;
    }

    //跟随主角与转向
    void followTarget()
    {
        float disTargetMyself = Vector3.Distance(m_Target.position, transform.position);

        if (disTargetMyself <= 2.0f)//距离小使用差值
        {
            
            transform.position = Vector3.Lerp(this.transform.position, m_Target.position, Time.deltaTime * 4f);
        }
        else//距离大直接定速移动
        {
            transform.position = Vector3.MoveTowards(transform.position, m_Target.position, Time.deltaTime * 20f);

        }

        switch (m_lookState)
        {
            case lookState.lookForward://向玩家前方看
                {

                    //print(m_Target.eulerAngles - transform.eulerAngles);
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_Target.rotation, Time.deltaTime * 2f);
                    break;

                }
            case lookState.lookAround://xj看
                {



                    m_Timer -= Time.deltaTime;
                    if(m_Timer>0)
                    {
                        break;
                    }
                    else
                    {
                        if(!is_turning)
                        {
                            is_turning = true;
                            n_target = Random.Range(20f, 40f);
                            if (Random.Range(0, 2) >= 1)
                            {
                                n_direct = 1;
                            }
                            else
                            {
                                n_direct = -1;
                            }


                            orig_direct = transform.rotation;
                            target_direct = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, n_target * n_direct, 0f));
                            if(Quaternion.Angle(target_direct,m_Target.rotation)>=50f)
                            {
                                target_direct = Quaternion.Euler(m_Target.eulerAngles + new Vector3(0f, n_target * n_direct, 0f));
                            }
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, target_direct, Time.deltaTime * 2f);

      

                            if(Quaternion.Angle(transform.rotation,target_direct)<5f || m_Timer<-2)
                            {
                                is_turning = false;
                                m_Timer = Random.Range(2f, 3f);
                            }
                        }
                        

                    }


                    break;
                }
            case lookState.lookPlayer://看玩家
                {
                    Debug.DrawRay(this.transform.position, m_Player.transform.position - this.transform.position);
                    //Quaternion sb = new Quaternion(m_Player.rotation.x+_offset.x, m_Player.rotation.y+_offset.y, m_Player.rotation.z+_offset.z,m_Player.rotation.w+_offset.w);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(m_Player.transform.position-this.transform.position), Time.deltaTime * 1f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_Player.transform.position - transform.position), Time.deltaTime * 1f);
                    break;
                }
        }


    }
}
