using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour {

    public AudioClip[] BGM;
    public AudioSource m_AudioSource;
    public float ChangeBGMSpeed = 4f;
    public float[] Vol;

	// Use this for initialization
	void Start () {
        m_AudioSource.clip = BGM[1];
        m_AudioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeBGM(int index)
    {
        if(m_AudioSource.clip != BGM[index])
        {

            StopAllCoroutines();
            StartCoroutine("ChangeToBGM",index);
        }
            
    }
    
    IEnumerator ChangeToBGM(int index)
    {
       
        while(m_AudioSource.volume>0.01f)
        {
            m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, 0f, Time.deltaTime * ChangeBGMSpeed);
            yield return 0;
        }
        m_AudioSource.clip = BGM[index];
        m_AudioSource.Play();

        while (m_AudioSource.volume < Vol[index])
        {
            m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, Vol[index], Time.deltaTime * ChangeBGMSpeed);
            //m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, 2, Time.deltaTime * ChangeBGMSpeed);
            yield return 0;
        }
    }
}
