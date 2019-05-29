using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public EnemyUFO[] m_UFO;
    public EnemyCannon[] m_Cannon;
    public EnemySoldier[] m_Soldier;
    public EnemyBoss m_Boss;

    public GameObject StaffMain;
    private Transform m_Player;
    private FirstPersonControllerNew PlayerInfo;
    public Collider[] Tri;
    public Transform[] Positions;
    public int Scenen;

    public Camera m_CGcamera;
    public GameObject m_VirtualCamera;

    public int Favor;
    private int BGM;

    private int Indanger;

    // Use this for initialization
    void Start()
    {
        Indanger = 0;

        BGM = 0;

        m_Player = GetComponentInChildren<FirstPersonControllerNew>().gameObject.transform;
        PlayerInfo = m_Player.gameObject.GetComponent<FirstPersonControllerNew>();
        if (Scenen == 1)
        {
            UI_Console.Instance.hideALLUI();
        }

        if (Scenen == 2)
        {
            Invoke("InvokeTheLogPanel", 1);
            UI_Console.Instance.UpdateMissionText("调查殖民地");
            UI_Console.Instance.UpdateLogText("已成功进入殖民地\n正在扫描搜索附近单位");
            UI_Console.Instance.UpdateLogText("位于殖民地中央的是用于远距离信号传输的增强器装置, 是本殖民地的数据中心,很可能与阻止这一切有关,我们要设法到达位于塔顶的AI控制中心.",
                0.5f, 9f);
        }

        PlayerInfo.n_Laser = PlayerPrefs.GetFloat("n_Laser", 300);
        PlayerInfo.n_ShootGun = PlayerPrefs.GetInt("n_ShootGun", 30);
        PlayerInfo.n_Snipe = PlayerPrefs.GetInt("n_Snipe", 30);
        Favor = PlayerPrefs.GetInt("Favor", 0);
        UI_Console.Instance.Journal.text = PlayerPrefs.GetString("n_JournalList", "JournalListContents");
    }

    // Update is called once per frame
    void Update()
    {
        if (Scenen == 1)
        {
            if ((m_Cannon[0] != null && m_Cannon[0].Detected) || (m_Cannon[1] != null && m_Cannon[1].Detected) ||
                (m_Soldier[0].m_State == EnemySoldier.State.Attack))
            {
                m_Cannon[0].Detected = true;
                m_Cannon[1].Detected = true;
                if (m_Soldier[0].m_State != EnemySoldier.State.Attack)
                {
                    m_Soldier[0].Degree = 20f;
                    m_Soldier[0].m_State = EnemySoldier.State.Alert;
                    m_Soldier[0].PlayerLastPosition = m_Player.position;
                }
            }

            if ((m_Soldier[1] != null && m_Soldier[1].m_State == EnemySoldier.State.Attack) ||
                (m_UFO[1] != null && m_UFO[1].m_state == EnemyUFO.State.Attack))
            {
                if (m_Soldier[1].m_State != EnemySoldier.State.Attack)
                {
                    m_Soldier[1].Degree = 20f;
                    m_Soldier[1].m_State = EnemySoldier.State.Alert;
                    m_Soldier[1].PlayerLastPosition = m_Player.position;
                }

                m_UFO[1].m_state = EnemyUFO.State.Attack;
            }


            for (int i = 0; i < 4; i++)
            {
                if ((m_Soldier[2 + i] != null && m_Soldier[2 + i].m_State == EnemySoldier.State.Attack) ||
                    (m_UFO[2 + i] != null && m_UFO[2 + i].m_state == EnemyUFO.State.Attack))
                {
                    if (m_Soldier[2].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[2].Degree = 20f;
                        m_Soldier[2].m_State = EnemySoldier.State.Alert;
                        m_Soldier[2].PlayerLastPosition = m_Player.position;
                    }

                    if (m_Soldier[3].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[3].Degree = 20f;
                        m_Soldier[3].m_State = EnemySoldier.State.Alert;
                        m_Soldier[3].PlayerLastPosition = m_Player.position;
                    }

                    if (m_Soldier[4].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[4].Degree = 20f;
                        m_Soldier[4].m_State = EnemySoldier.State.Alert;
                        m_Soldier[4].PlayerLastPosition = m_Player.position;
                    }

                    if (m_Soldier[5].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[5].Degree = 20f;
                        m_Soldier[5].m_State = EnemySoldier.State.Alert;
                        m_Soldier[5].PlayerLastPosition = m_Player.position;
                    }

                    m_UFO[2].m_state = EnemyUFO.State.Attack;
                    m_UFO[3].m_state = EnemyUFO.State.Attack;
                    m_UFO[4].m_state = EnemyUFO.State.Attack;
                    m_UFO[5].m_state = EnemyUFO.State.Attack;
                }
            }
        }
        else if (Scenen == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((m_UFO[0 + i] != null && m_UFO[0 + i].m_state == EnemyUFO.State.Attack) ||
                    (m_Cannon[0 + i * 2] != null && m_Cannon[0 + i * 2].Detected) ||
                    (m_Cannon[1 + i * 2] != null && m_Cannon[1 + i * 2].Detected) ||
                    (m_Soldier[0 + i * 2] != null && m_Soldier[0 + i * 2].m_State == EnemySoldier.State.Attack) ||
                    (m_Soldier[1 + i * 2] != null && m_Soldier[1 + i * 2].m_State == EnemySoldier.State.Attack))
                {
                    //print("attack");
                    m_UFO[0 + i].m_state = EnemyUFO.State.Attack;
                    m_UFO[0 + i].Degree = 20f;
                    m_Cannon[0 + i * 2].Detected = true;
                    m_Cannon[1 + i * 2].Detected = true;
                    if (m_Soldier[0 + i * 2].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[0 + i * 2].Degree = 20f;
                        m_Soldier[0 + i * 2].m_State = EnemySoldier.State.Alert;
                        m_Soldier[0 + i * 2].PlayerLastPosition = m_Player.position;
                    }

                    if (m_Soldier[1 + i * 2].m_State != EnemySoldier.State.Attack)
                    {
                        m_Soldier[1 + i * 2].Degree = 20f;
                        m_Soldier[1 + i * 2].m_State = EnemySoldier.State.Alert;
                        m_Soldier[1 + i * 2].PlayerLastPosition = m_Player.position;
                    }
                }
            }

            if ((m_Soldier[11] != null && m_Soldier[11].m_State == EnemySoldier.State.Attack) ||
                (m_Soldier[12] != null && m_Soldier[12].m_State == EnemySoldier.State.Attack) ||
                (m_Cannon[8] != null && m_Cannon[8].Detected))
            {
                if (m_Soldier[11].m_State != EnemySoldier.State.Attack)
                {
                    m_Soldier[11].Degree = 20f;
                    m_Soldier[11].m_State = EnemySoldier.State.Alert;
                    m_Soldier[11].PlayerLastPosition = m_Player.position;
                }

                if (m_Soldier[12].m_State != EnemySoldier.State.Attack)
                {
                    m_Soldier[12].Degree = 20f;
                    m_Soldier[12].m_State = EnemySoldier.State.Alert;
                    m_Soldier[12].PlayerLastPosition = m_Player.position;
                }
            }
        }

        for (int i = 0; i < m_UFO.Length; i++)
        {
            if (m_UFO[i] != null &&
                (m_UFO[i].m_state == EnemyUFO.State.Attack || m_UFO[i].m_state == EnemyUFO.State.Alert))
            {
                //print(m_UFO[i].transform.name);
                Indanger++;
                break;
            }
        }

        for (int i = 0; i < m_Soldier.Length; i++)
        {
            if (m_Soldier[i] != null && (m_Soldier[i].m_State == EnemySoldier.State.Attack ||
                                         m_Soldier[i].m_State == EnemySoldier.State.Ensure ||
                                         m_Soldier[i].m_State == EnemySoldier.State.Alert))
            {
                //print(m_Soldier[i].transform.parent.name);
                Indanger++;
                break;
            }
        }

        for (int i = 0; i < m_Cannon.Length; i++)
        {
            if (m_Cannon[i] != null && m_Cannon[i].Detected == true)
            {
                //print(m_Cannon[i].transform.parent.name);
                Indanger++;
                break;
            }
        }

        if (m_Boss != null && m_Boss.KillAll)
        {
            Indanger++;
        }
        else if (m_Boss == null && Scenen == 2)
        {
            Invoke("Ending", 5f);
        }

        if (Indanger > 0)
        {
            SendMessage("ChangeBGM", 2);
        }
        else
        {
            SendMessage("ChangeBGM", 1);
        }

        Indanger = 0;

        // 各种按键的操作
        ToggleMenu();
        TogglePause();
        ChangBGM();
        弹出Log();
        UIsetSwitch();
        UIOVCSwitch();
        TestMode();
    }


    public void ToggleMenu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UI_Console.Instance.hideInventory();
        }
    }

    public void TogglePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale < 0.01f)
            {
                Time.timeScale = 1;
            }
            else if (Time.timeScale > 0.99f)
            {
                Time.timeScale = 0;
            }
        }
    }

    public void ChangBGM()
    {
        if (Input.GetKey(KeyCode.M))
        {
            SendMessage("ChangeBGM", BGM);
            BGM++;
            BGM %= 3;
        }
    }

    public void 弹出Log()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UI_Console.Instance.hideDialog();
        }
    }

    public void UIsetSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            UI_Console.Instance.hideALLUI();
        }
    }

    public void UIOVCSwitch()
    {
        if (Input.GetKeyDown((KeyCode.N)))
        {
            UI_Console.Instance.UIOAC_Switch();
        }
    }

    private bool isTest;

    public void TestMode()
    {
        if (Input.GetKeyDown((KeyCode.T)))
        {
            if (!isTest)
            {
                PlayerInfo.m_WalkSpeed = 100;
                isTest = !isTest;
            }
            else
            {
                PlayerInfo.m_WalkSpeed = 10;
                isTest = !isTest;
            }
        }
    }

    public void Relive()
    {
        FirstPersonControllerNew m_player = StaffMain.GetComponentInChildren<FirstPersonControllerNew>();
        m_player.Relive();
        PlayerInfo.n_Laser = PlayerPrefs.GetInt("n_Laser", 300);
        PlayerInfo.n_ShootGun = PlayerPrefs.GetInt("n_ShootGun", 30);
        PlayerInfo.n_Snipe = PlayerPrefs.GetInt("n_Snipe", 30);

        for (int i = 0; i < m_UFO.Length; i++)
        {
            if (m_UFO[i] != null)
            {
                //print(m_UFO[i].transform.name);
                m_UFO[i].Degree = 0;
                m_UFO[i].m_state = EnemyUFO.State.Patrol;
            }
        }

        for (int i = 0; i < m_Soldier.Length; i++)
        {
            if (m_Soldier[i] != null)
            {
                //print(m_Soldier[i].transform.parent.name);
                m_Soldier[i].Degree = 0f;
                m_Soldier[i].m_State = EnemySoldier.State.Patrol;
            }
        }

        for (int i = 0; i < m_Cannon.Length; i++)
        {
            if (m_Cannon[i] != null)
            {
                //print(m_Cannon[i].transform.parent.name);
                m_Cannon[i].Detected = false;
            }
        }
    }

    private int Doors;

    public void OperateTower()
    {
        UI_Console.Instance.UpdateLogText(" 按 [1] [2] [3] 分别打开独立的门,都打开后方可进入,需要爬很长的路才能到达顶层的控制中枢\n再次按 [F] 退出界面");
        m_CGcamera.gameObject.SetActive(true);
        m_VirtualCamera.gameObject.SetActive(true);
    }


    private void Ending()
    {
        SceneManager.LoadScene(3);
    }

    public void FavorChange(int Num)
    {
        Favor = PlayerPrefs.GetInt("Favor", 0);
        Favor += Num;
        PlayerPrefs.SetInt("Favor", Favor);
        print("favor:\t" + Favor);
    }

    public void InvokeTheLogPanel()
    {
        UI_Console.Instance.hideDialog();
    }
}