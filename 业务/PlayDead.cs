using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class PlayDead : MonoBehaviour
{
    public PlayableDirector m_Director;

    void Update()
    {
        if (m_Director.time >= (m_Director.duration - 0.05f))
        {
            print("dead");
            SendMessageUpwards("Relive");
            this.gameObject.SetActive(false);

            // 重生重新显示UI
            UI_Console.Instance.hideALLUI();
        }
    }
}