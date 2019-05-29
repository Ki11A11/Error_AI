using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;


public class WorkFlow : MonoBehaviour
{
    public PlayableDirector[] m_Director;
    private PlayableDirector n_Director;
    private FirstPersonControllerNew PlayerInfo;


    public GameObject StaffMain;

    // Use this for initialization
    void Start()
    {
        ShipComeIn();
        PlayerInfo = GetComponentInChildren<FirstPersonControllerNew>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpCG();
        if (m_Director[3] != null && m_Director[3].time >= m_Director[3].duration - 0.05f)
        {
            SceneManager.LoadScene(2);
        }
    }

    public void ShipComeIn()
    {
        if (m_Director[0] != null)
        {
            m_Director[0].gameObject.SetActive(true);
            n_Director = m_Director[0];
        }
    }

    public void GetOutOfShip()
    {
        m_Director[2].gameObject.SetActive(true);
        n_Director = m_Director[2];

        // 狗日的终于找到你了
        UI_Console.Instance.hideALLUI();
        UI_Console.Instance.hideDialog();
        Invoke("ReadyInvoke", 8);
    }

    public void GetToBoliQiu()
    {
        m_Director[3].gameObject.SetActive(true);
        PlayerPrefs.SetFloat("n_Laser", PlayerInfo.n_Laser);
        PlayerPrefs.SetInt("n_Snipe", PlayerInfo.n_Snipe);
        PlayerPrefs.SetInt("n_ShootGun", PlayerInfo.n_ShootGun);
        PlayerPrefs.SetString("n_JournalList", UI_Console.Instance.Journal.text);
        n_Director = m_Director[3];
        StaffMain.SetActive(false);
    }

    public void MeetBoss()
    {
        m_Director[4].gameObject.SetActive(true);
        n_Director = m_Director[1];
        UI_Console.Instance.UpdateLogText(
            "看来这就是信息和电力控制塔了\n" +
            "无论如何,先摧毁它吧!");
    }

    public void JumpCG()
    {
        if (Input.GetKey(KeyCode.J))
        {
            n_Director.time = n_Director.duration;
        }
    }

    void ReadyInvoke()
    {
        UI_Console.Instance.hideALLUI();
        UI_Console.Instance.hideDialog();
        UI_Console.Instance.UpdateLogText("看到了么,就是那里,应该就是发生事故的地点了");
    }
}