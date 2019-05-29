using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class BossLaser : MonoBehaviour
{
    public GameObject Line;
    public GameObject FXef; //激光击中物体的粒子效果

    private FirstPersonControllerNew m_player;
    // Use this for initialization
    // Update is called once per frame

    public float AttackColdDown = 0.1f;
    private float Timer;

    public LayerMask mask;
    public int Damage = 5;
    private float offset = 1;

    private void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<FirstPersonControllerNew>();
        Timer = AttackColdDown;
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 Sc; // 变换大小
        Sc.x = 0.5f;
        Sc.z = 0.5f;
        //发射射线，通过获取射线碰撞后返回的距离来变换激光模型的y轴上的值

        if (Timer > 0)
            Timer -= Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 600f,
            mask.value))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
            Sc.y = hit.distance;
//            print("Sc.y:"+Sc.y);
            //print(hit.transform.name);
            if (hit.transform.tag == "Terrian")
            {
                Sc.y += 1f;
            }

            FXef.transform.position = hit.point; //让激光击中物体的粒子效果的空间位置与射线碰撞的点的空间位置保持一致；
            FXef.SetActive(true);

            if (Timer < 0)
            {
                if (hit.transform.tag == "MainCharacter")
                {
                    m_player.GetDamage(Damage);
                    Timer = AttackColdDown;
                }
            }
        }
        //当激光没有碰撞到物体时，让射线的长度保持为500m，并设置击中效果为不显示
        else
        {
            Sc.y = 500;
            FXef.SetActive(false);
        }

        Line.transform.localScale = Sc * offset;
    }
}