using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour {


    public float AttackColdDown = 0.5f;
    private float Timer;

    public LayerMask mask;

    public GameObject Bullet;



    // Use this for initialization
    void Start () {
        Timer = AttackColdDown;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitMid;
        Vector2 v = new Vector2(Screen.width / 2, Screen.height / 2); //屏幕中心点

        Vector3 alignedDirect = new Vector3();
        Vector3 target = new Vector3();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(v), out hitMid, 600f, mask.value))
        {
            Timer -= Time.deltaTime;


            target = hitMid.point;
            //print(hitMid.transform.name);
            alignedDirect = hitMid.point - this.transform.position;
            Debug.DrawRay(this.transform.position, alignedDirect);

            if(Timer<0)
            {
                GameObject m_bullet = Instantiate(Bullet, this.transform.position, this.transform.rotation) as GameObject;
                m_bullet.GetComponent<PlayerShootBullet>().TargetDirect = Vector3.Normalize(alignedDirect);
                m_bullet.SetActive(true);
                Timer += AttackColdDown;
            }
            

        }
    }


}
