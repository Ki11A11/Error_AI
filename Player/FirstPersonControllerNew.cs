using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonControllerNew : MonoBehaviour
    {
        public int MaxHP = 100;
        public int MaxSheild = 100;
        public int MaxEnergy = 100;

        private int m_Hp;
        private int m_Sheild;
        private int m_Energy;
        public int RunEnergy = 2;
        public int RecoverEnergy = 1;
        private float EnergyTimer;
        private float RecoverEnergyTimer;
        public float RecoverEnergySet = 0.25f;


        public bool is_Dead;
        private bool is_Tired;
        private bool is_Attacked;
        private bool is_Tab;
        private float is_Attacked_Timer;
        private float SheildTimer;

        public GameObject Timeline_Dead;
        public Transform[] Gate;


        [SerializeField] private bool m_IsWalking;
        [SerializeField] public float m_WalkSpeed;
        [SerializeField] public float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;

        [SerializeField]
        private AudioClip[] m_FootstepSounds; // an array of footstep sounds that will be randomly selected from.

        [SerializeField] private AudioClip m_JumpSound; // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound; // the sound played when character touches back on ground.


        public GameObject[] Gun;
        public GameObject[] GunModels;
        public AudioClip SnipeFire;
        public AudioSource SnipeSource;
        public GameObject ShootGunBullet;
        public GameObject Crystal;
        private float CrystalRotate;
        public GameObject CrystalPoint;

        public enum GunState
        {
            Laser,
            ShootGun,
            Snipe
        }

        public float ShootGunColdDown = 0.3f;
        private float m_ShootGunTimer;
        private int Snipe_State;
        public GameObject SnipeSpark;

        public GunState m_GunState = GunState.Laser;
        private bool ChangeGun;
        public GameObject Hand;
        public float m_ChangTimer;
        public float ChangTimer = 0.5f;
        private Vector3 Orig_Hand_Position;
        private Vector3 Orig_Hand_Angle;
        private Vector3 Orig_Camera;
        public LayerMask mask;
        public LayerMask SnipeMask;

        public float SnipeAttackColdDown = 2f;
        private float SnipeTimer;
        public int SnipeDamage = 50;

        private int MaxLaser = 1000;
        private int MaxShootGun = 100;
        private int MaxSnipe = 100;

        public float n_Laser;
        public int n_ShootGun;
        public int n_Snipe;

        public float FieldOfViewSpeed = 5f;


        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        public AudioSource m_breathe;
        public AudioSource m_laserSound;
        public AudioClip LaserCharge;
        public AudioClip LaserBeam;
        public AudioClip GainAmmo;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);

            m_Hp = MaxHP;
            m_Sheild = MaxSheild;
            m_Energy = MaxEnergy;

            is_Dead = false;
            is_Tired = false;
            is_Attacked = false;
            is_Attacked_Timer = 0f;
            is_Tab = false;
            SheildTimer = 0f;

            UI_Console.Instance.switchBulletType(0);

            ChangeGun = false;
            m_ChangTimer = ChangTimer;
            m_ShootGunTimer = 0f;
            EnergyTimer = 0f;
            SnipeTimer = 0f;
            CrystalRotate = 1f;

            Orig_Hand_Position = Hand.transform.localPosition;
            Orig_Hand_Angle = Hand.transform.localEulerAngles;

            Orig_Camera = m_Camera.transform.localPosition;
        }


        // Update is called once per frame
        private void Update()
        {
            UI_Console.Instance.pointHealth(m_Hp);
            UI_Console.Instance.pointShield(m_Sheild);
            UI_Console.Instance.pointStrength(m_Energy);
            UpDateEnergy();
            UpDateSheild();

            //更新弹药
            UI_Console.Instance.UpdateBullet();

            if (Input.GetKeyDown(KeyCode.Tab))
                is_Tab = !is_Tab;

            if (is_Tab)
                return;
            if (is_Dead)
                return;

            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            if (Time.timeScale > 0.01f)
                PlayGun();

            if (is_Tired)
            {
                if (!m_breathe.isPlaying)
                    m_breathe.Play();
            }
            else
            {
                m_breathe.Stop();
            }

            GetItem();

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        /// <summary>
        /// control and recover player's energy
        /// </summary>
        private void UpDateEnergy()
        {
            if ((m_Energy < 100 && m_IsWalking) || is_Tired)
            {
                if (RecoverEnergyTimer > 0)
                {
                    RecoverEnergyTimer -= Time.deltaTime;
                }
                else
                {
                    m_Energy += RecoverEnergy;
                    RecoverEnergyTimer += RecoverEnergySet;
                    //UI_Console.Instance.strengthGain((int)RecoverEnergy);
                }
            }

            if (m_Energy >= 100)
            {
                //print("notTired");
                is_Tired = false;
                m_Energy = 100;
            }

            if (m_Energy <= 0)
            {
                //print("Tired");
                is_Tired = true;
                m_Energy = 0;
            }
        }


        private void UpDateSheild()
        {
            if (is_Attacked)
            {
                is_Attacked_Timer -= Time.deltaTime;
                if (is_Attacked_Timer < 0)
                    is_Attacked = false;
            }

            if (!is_Attacked && m_Sheild < MaxSheild)
            {
                SheildTimer += Time.deltaTime;
                if (SheildTimer >= 0.5f)
                {
                    m_Sheild += 1;
                    //UI_Console.Instance.shieldGain(1);
                    SheildTimer = 0f;
                }
            }
        }


        /// <summary>
        /// get energy attack
        /// </summary>
        /// <param name="lose"></param>
        public void LoseEnergy(float lose)
        {
            //UI_Console.Instance.strengthDamage(m_Energy>lose ? Mathf.FloorToInt(lose) : Mathf.FloorToInt(m_Energy));
            m_Energy -= (int) lose;
            if (m_Energy < 0)
            {
                m_Energy = 0;
                is_Tired = true;
            }
        }


        /// <summary>
        /// player get damage
        /// </summary>
        /// <param name="Damage"></param>
        public void GetDamage(int Damage)
        {
            is_Attacked = true;
            is_Attacked_Timer = 5f;
            StartCoroutine("GetDamageAction");
            if (m_Sheild >= Damage)
            {
                m_Sheild -= Damage;
            }
            else
            {
                m_Hp -= (Damage - m_Sheild);
                m_Sheild = 0;
            }

            if (m_Hp <= 0)
            {
                UI_Console.Instance.pointHealth(m_Hp);
                is_Dead = true;
                // 死亡隐藏UI
                if (!Timeline_Dead.active)
                {
                    UI_Console.Instance.hideALLUI();
                }

                Timeline_Dead.SetActive(true);
                m_Hp = 0;
            }
        }

        public void Relive()
        {
            // 这里可能要写一个隐藏UI的
            m_Hp = MaxHP;
            m_Sheild = MaxSheild;
            m_Energy = MaxEnergy;
            is_Dead = false;
            transform.position = Gate[0].transform.position;
        }


        /// <summary>
        /// imitate get hurt by move the main camera
        /// </summary>
        /// <returns></returns>
        IEnumerator GetDamageAction()
        {
            float Timer = 0.05f;

            while (Timer > 0)
            {
                Timer -= Time.deltaTime;
                m_Camera.transform.Translate(Vector3.down * Time.deltaTime * 5f, Space.Self);
                yield return null;
            }

            Timer = -0.05f;

            while (Timer < 0)
            {
                Timer += Time.deltaTime;
                m_Camera.transform.Translate(Vector3.up * Time.deltaTime * 5f, Space.Self);
                yield return null;
            }

            m_Camera.transform.localPosition = Orig_Camera;

            yield return null;
        }


        private void GetItem()
        {
            RaycastHit hitMid;
            Vector2 v = new Vector2(Screen.width / 2, Screen.height / 2);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(v), out hitMid, 2f))
            {
                // 开飞船门和开Tower的门
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (hitMid.transform.name == "Gate_Ship")
                    {
                        transform.position = Gate[0].transform.position;
                        SendMessageUpwards("GetOutOfShip");
                    }
                    else if (hitMid.transform.name == "Ship_Core")
                    {
                        SendMessageUpwards("OperateTower");
                    }
                }

                // 各种物品的接触获得
                if (hitMid.transform.tag == "FirstAid")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nHp up 30");
                    m_Hp += 30;
                    Destroy(hitMid.transform.gameObject);
                    if (m_Hp > MaxHP)
                        m_Hp = MaxHP;
                }
                else if (hitMid.transform.tag == "FirstShield")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nShield up 30");
                    m_Sheild += 30;
                    Destroy(hitMid.transform.gameObject);
                    if (m_Sheild > MaxSheild)
                        m_Sheild = MaxSheild;
                }
                else if (hitMid.transform.tag == "FirstEnergy")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nEnergy up 30");
                    m_Energy += 30;
                    Destroy(hitMid.transform.gameObject);
                    if (m_Energy > MaxEnergy)
                        m_Energy = MaxEnergy;
                }
                else if (hitMid.transform.tag == "laserbullet")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nLaserPower up 300");
                    n_Laser += 300;
                    Destroy(hitMid.transform.gameObject);
                    if (n_Laser > MaxLaser)
                        n_Laser = MaxLaser;
                }
                else if (hitMid.transform.tag == "snipebullet")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nSniperBullet up 30");
                    n_Snipe += 30;
                    Destroy(hitMid.transform.gameObject);
                    if (n_Snipe > MaxSnipe)
                        n_Snipe = MaxSnipe;
                }
                else if (hitMid.transform.tag == "shootbullet")
                {
                    m_AudioSource.clip = GainAmmo;
                    m_AudioSource.Play();
                    UI_Console.Instance.UpdateJournal("\nShootBullet up 30");
                    n_ShootGun += 30;
                    Destroy(hitMid.transform.gameObject);
                    if (n_ShootGun > MaxShootGun)
                        n_ShootGun = MaxShootGun;
                }
            }
        }


        private void PlayGun()
        {
            ////------------cast a ray from the mid of maincamera --------------/////
            RaycastHit hitMid;
            Vector2 v = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector3 alignedDirect = new Vector3();
            Vector3 target = new Vector3();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(v), out hitMid, 6000f, mask.value))
            {
                //m_ShootGunTimer -= Time.deltaTime;


                target = hitMid.point;
                //print(hitMid.transform.name);

                //print(hitMid.point);
                //Debug.DrawRay(this.transform.position, alignedDirect);
            }

            if (SnipeTimer >= 0)
            {
                SnipeTimer -= Time.deltaTime;
            }

            if (m_ShootGunTimer >= 0)
            {
                m_ShootGunTimer -= Time.deltaTime;
            }


            if (ChangeGun == false) //if not change then you can do anything with gun
            {
                //////-----------when left mouse butten click shoot------------////////
                if (Input.GetMouseButtonDown(0))
                {
                    switch (m_GunState)
                    {
                        case GunState.Laser:
                        {
                            break;
                        }

                        case GunState.ShootGun:
                        {
                            //print("start");
                            //Gun[1].gameObject.SetActive(true);
                            if (m_ShootGunTimer < 0 && n_ShootGun > 0)
                            {
                                if (Snipe_State != 0)
                                {
                                    StartCoroutine("GetDamageAction");
                                }

                                n_ShootGun--;
                                m_ShootGunTimer = ShootGunColdDown;
                                alignedDirect = hitMid.point - Gun[1].transform.position;
                                GameObject m_bullet = Instantiate(ShootGunBullet, Gun[1].transform.position,
                                    this.transform.rotation) as GameObject;
                                m_bullet.transform.LookAt(hitMid.point);
                                m_bullet.GetComponent<Rigidbody>().AddForce(m_bullet.transform.forward * 2000f);
                                m_bullet.GetComponent<ProjectileScript>().impactNormal = hitMid.normal;


                                //GameObject projectile = Instantiate(projectiles[currentProjectile], spawnPosition.position, Quaternion.identity) as GameObject;
                                //projectile.transform.LookAt(hit.point);
                                //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
                                //projectile.GetComponent<ProjectileScript>().impactNormal = hit.normal;


                                //m_bullet.GetComponent<PlayerShootBullet>().TargetDirect = Vector3.Normalize(alignedDirect);
                                m_bullet.SetActive(true);

                                UI_Console.Instance.shakeAimPoint();
                                StartCoroutine("ShakeHand", 0.1f);

                                //m_ChangTimer -= Time.deltaTime * 2f * 5f;
                                //Hand.transform.Translate(m_ChangTimer * Vector3.back * Time.deltaTime * 0.4f, Space.Self);//ǰ��
                                //Hand.transform.Rotate(Vector3.left, 150f * m_ChangTimer * Time.deltaTime * 0.1f);//��ת

                                if (m_ChangTimer < 0)
                                {
                                    ChangeGunModel();
                                }

                                if (m_ChangTimer <= -ChangTimer)
                                {
                                    Hand.transform.localPosition = Orig_Hand_Position;
                                    Hand.transform.localEulerAngles = Orig_Hand_Angle;
                                    m_ChangTimer = ChangTimer;
                                    ChangeGun = false;
                                }
                            }

                            break;
                        }
                        case GunState.Snipe:
                        {
                            if (SnipeTimer < 0 && n_Snipe > 0)
                            {
                                RaycastHit hit;
                                if (Physics.Raycast(Camera.main.ScreenPointToRay(v), out hit, 6000f, SnipeMask.value))
                                {
                                    GameObject m_bullet =
                                        Instantiate(SnipeSpark, hit.point, transform.rotation) as GameObject;
                                }


                                SnipeSource.volume = 0.5f;
                                SnipeSource.clip = SnipeFire;
                                SnipeSource.Play();
                                n_Snipe--;
                                SnipeTimer += SnipeAttackColdDown;
                                if (Snipe_State != 0)
                                {
                                    StartCoroutine("GetDamageAction");
                                }

                                UI_Console.Instance.shakeAimPoint();
                                //                                    print("snipe");
                                StartCoroutine("ShakeHand", 0.5f);
                                print("狙击击中物体 :\t" + hit.transform.name);

                                if (hit.transform.tag == "UFO")
                                {
                                    hit.transform.gameObject.GetComponentInParent<EnemyUFO>().GetDamage(SnipeDamage);
                                }
                                else if (hit.transform.tag == "Cannon")
                                {
                                    EnemyCannon m_cannon;
                                    m_cannon = hit.transform.gameObject.GetComponentInChildren<EnemyCannon>();
                                    if (m_cannon == null)
                                    {
                                        m_cannon = hit.transform.gameObject.GetComponentInParent<EnemyCannon>();
                                    }


                                    m_cannon.GetDamage(SnipeDamage);
                                }
                                else if (hit.transform.tag == "Soldier")
                                {
                                    EnemySoldier m_soldier;
                                    m_soldier = hit.transform.gameObject.GetComponent<EnemySoldier>();
                                    if (m_soldier == null)
                                    {
                                        m_soldier = hit.transform.gameObject.GetComponentInParent<EnemySoldier>();
                                    }


                                    m_soldier.GetDamage(SnipeDamage);
                                }
                                else if (hit.transform.tag == "Boss")
                                {
                                    EnemyBoss m_boss;
                                    m_boss = hit.transform.gameObject.GetComponent<EnemyBoss>();
                                    if (m_boss == null)
                                    {
                                        m_boss = hit.transform.gameObject.GetComponentInParent<EnemyBoss>();
                                    }


                                    m_boss.GetDamage(SnipeDamage);
                                }
                            }

                            break;
                        }
                    }
                }

                //////-----------when right botton click change the camera view---------///////
                if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift))
                {
                    if (Snipe_State == 0)
                    {
                        StopCoroutine("camera_FieldOfView");
                        Snipe_State = 1;
                        StartCoroutine("camera_FieldOfView", 15f);
                    }

                    else if (Snipe_State == 1)
                    {
                        if (m_GunState == GunState.Snipe)
                        {
                            StopCoroutine("camera_FieldOfView");
                            Snipe_State = 2;
                            StartCoroutine("camera_FieldOfView", 5f);
                        }
                        else
                        {
                            StopCoroutine("camera_FieldOfView");
                            Snipe_State = 0;
                            StartCoroutine("camera_FieldOfView", 60f);
                        }
                    }


                    else if (Snipe_State == 2)
                    {
                        StopCoroutine("camera_FieldOfView");
                        Snipe_State = 0;
                        StartCoroutine("camera_FieldOfView", 60f);
                    }
                }

                //////-----------continiue push left mouse botton shoot---------///////
                if (Input.GetMouseButton(0))
                {
                    switch (m_GunState)
                    {
                        case GunState.Laser:
                        {
                            if (n_Laser > 0)
                            {
                                CrystalRotate += Time.deltaTime * 2;
                                CrystalPoint.SetActive(true);
                                if (CrystalRotate > 3)
                                {
                                    n_Laser -= Time.deltaTime * 10;
                                    Gun[0].gameObject.SetActive(true);
                                    CrystalRotate = 3;
                                    if (m_laserSound.clip != LaserBeam)
                                    {
                                        m_laserSound.clip = LaserBeam;
                                        m_laserSound.loop = true;
                                        m_laserSound.Play();
                                    }
                                }
                                else
                                {
                                    if (m_laserSound.clip != LaserCharge || !m_laserSound.isPlaying)
                                    {
                                        m_laserSound.clip = LaserCharge;
                                        if (LaserCharge.length * (CrystalRotate / 2.75f) < LaserCharge.length / 2)
                                        {
                                            m_laserSound.time = LaserCharge.length * (CrystalRotate / 2.75f);
                                        }
                                        else
                                        {
                                            m_laserSound.time = LaserCharge.length;
                                        }

                                        if (m_laserSound.isActiveAndEnabled)
                                        {
                                            m_laserSound.loop = false;
                                            m_laserSound.Play();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CrystalPoint.SetActive(false);
                                Gun[0].gameObject.SetActive(false);
                                m_laserSound.Stop();
                                if (CrystalRotate > 0.25)
                                    CrystalRotate -= Time.deltaTime;

                                if (CrystalRotate < 0.25)
                                    CrystalRotate = 0.25f;
                            }


                            break;
                        }

                        case GunState.ShootGun:
                        {
                            //Gun[1].gameObject.SetActive(true);
                            //if (m_ShootGunTimer < 0 && n_ShootGun>0)
                            //{
                            //    n_ShootGun--;
                            //    if (Snipe_State != 0)
                            //    {
                            //        StartCoroutine("GetDamageAction");
                            //    }
                            //    m_ShootGunTimer = ShootGunColdDown;
                            //    alignedDirect = hitMid.point - Gun[1].transform.position;
                            //    GameObject m_bullet = Instantiate(ShootGunBullet, Gun[1].transform.position, this.transform.rotation) as GameObject;
                            //    m_bullet.transform.LookAt(hitMid.point);
                            //    m_bullet.GetComponent<Rigidbody>().AddForce(m_bullet.transform.forward * 2000f);
                            //    m_bullet.GetComponent<ProjectileScript>().impactNormal = hitMid.normal;
                            //    //GameObject m_bullet = Instantiate(ShootGunBullet, Gun[1].transform.position, this.transform.rotation) as GameObject;
                            //    ////print(alignedDirect);
                            //    //m_bullet.GetComponent<PlayerShootBullet>().TargetDirect = Vector3.Normalize(alignedDirect);
                            //    m_bullet.SetActive(true);

                            //    StartCoroutine("ShakeHand",0.1f);
                            //    UI_Console.Instance.shakeAimPoint();

                            //}


                            if (m_ShootGunTimer < 0 && n_ShootGun > 0)
                            {
                                if (Snipe_State != 0)
                                {
                                    StartCoroutine("GetDamageAction");
                                }

                                n_ShootGun--;
                                m_ShootGunTimer = ShootGunColdDown;
                                alignedDirect = hitMid.point - Gun[1].transform.position;
                                GameObject m_bullet = Instantiate(ShootGunBullet, Gun[1].transform.position,
                                    this.transform.rotation) as GameObject;
                                m_bullet.transform.LookAt(hitMid.point);
                                m_bullet.GetComponent<Rigidbody>().AddForce(m_bullet.transform.forward * 2000f);
                                m_bullet.GetComponent<ProjectileScript>().impactNormal = hitMid.normal;


                                //GameObject projectile = Instantiate(projectiles[currentProjectile], spawnPosition.position, Quaternion.identity) as GameObject;
                                //projectile.transform.LookAt(hit.point);
                                //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
                                //projectile.GetComponent<ProjectileScript>().impactNormal = hit.normal;


                                //m_bullet.GetComponent<PlayerShootBullet>().TargetDirect = Vector3.Normalize(alignedDirect);
                                m_bullet.SetActive(true);

                                UI_Console.Instance.shakeAimPoint();
                                StartCoroutine("ShakeHand", 0.1f);

                                //m_ChangTimer -= Time.deltaTime * 2f * 5f;
                                //Hand.transform.Translate(m_ChangTimer * Vector3.back * Time.deltaTime * 0.4f, Space.Self);//ǰ��
                                //Hand.transform.Rotate(Vector3.left, 150f * m_ChangTimer * Time.deltaTime * 0.1f);//��ת

                                if (m_ChangTimer < 0)
                                {
                                    ChangeGunModel();
                                }

                                if (m_ChangTimer <= -ChangTimer)
                                {
                                    Hand.transform.localPosition = Orig_Hand_Position;
                                    Hand.transform.localEulerAngles = Orig_Hand_Angle;
                                    m_ChangTimer = ChangTimer;
                                    ChangeGun = false;
                                }
                            }

                            break;
                        }
                        case GunState.Snipe:
                        {
                            break;
                        }
                    }
                }
                else
                {
                    switch (m_GunState)
                    {
                        case GunState.Laser:
                        {
                            CrystalPoint.SetActive(false);
                            Gun[0].gameObject.SetActive(false);
                            m_laserSound.Stop();
                            if (CrystalRotate > 0.25)
                                CrystalRotate -= Time.deltaTime;

                            if (CrystalRotate < 0.25)
                                CrystalRotate = 0.25f;
                            break;
                        }
                    }
                }

                if (m_GunState == GunState.Laser)
                    Crystal.transform.localEulerAngles = Crystal.transform.localEulerAngles +
                                                         new Vector3(0, 0, Time.deltaTime * 400 * CrystalRotate);

                //////-----------release left mouse botton stop shoot---------////////
                if (Input.GetMouseButtonUp(0))
                {
                    switch (m_GunState)
                    {
                        case GunState.Laser:
                        {
                            Gun[0].gameObject.SetActive(false);
                            break;
                        }

                        case GunState.ShootGun:
                        {
                            //Gun[1].gameObject.SetActive(false);
                            break;
                        }
                        case GunState.Snipe:
                        {
                            break;
                        }
                    }
                }
            }


            ///////---------mouse mid botton change weapon----------///////
            if (ChangeGun == false && Snipe_State == 0)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    m_GunState = (GunState) (((int) m_GunState + 1) % 3);
                    Gun[0].SetActive(false);
                    ChangeGun = true;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    m_GunState = (GunState) (((int) m_GunState - 1) < 0 ? 2 : (int) m_GunState - 1);
                    Gun[0].SetActive(false);
                    ChangeGun = true;
                }
            }


            ///////---------when change gun raise hand and call to change gun model---------///////
            if (ChangeGun == true)
            {
                m_ChangTimer -= Time.deltaTime * 2f;
                Hand.transform.Translate(m_ChangTimer * Vector3.back * Time.deltaTime, Space.Self);
                Hand.transform.Rotate(Vector3.left, 150f * m_ChangTimer * Time.deltaTime);

                CrystalPoint.SetActive(false);
                Gun[0].gameObject.SetActive(false);
                CrystalRotate = 0.25f;


                if (m_ChangTimer < 0)
                {
                    ChangeGunModel();
                }

                if (m_ChangTimer <= -ChangTimer)
                {
                    Hand.transform.localPosition = Orig_Hand_Position;
                    Hand.transform.localEulerAngles = Orig_Hand_Angle;
                    m_ChangTimer = ChangTimer;
                    ChangeGun = false;
                }
            }
        }

        /// <summary>
        /// change the camera view by lerp
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        IEnumerator camera_FieldOfView(float degree)
        {
            while (Mathf.Abs(m_Camera.fieldOfView - degree) > 1f)
            {
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, degree, Time.deltaTime * FieldOfViewSpeed);
                yield return null;
            }
        }


        IEnumerator ShakeHand(float time)
        {
            float Timer = time;
            while (Timer >= -time)
            {
                Timer -= Time.deltaTime * 5f;
                Hand.transform.Translate(Timer * Vector3.back * Time.deltaTime * 4f, Space.Self); //ǰ��
                Hand.transform.Rotate(Vector3.left, 150f * Timer * Time.deltaTime); //��ת

                yield return null;
            }

            Hand.transform.localPosition = Orig_Hand_Position;
            Hand.transform.localEulerAngles = Orig_Hand_Angle;
            Timer = time;
        }


        /// <summary>
        /// when change gun change gun model
        /// </summary>
        private void ChangeGunModel()
        {
            for (int i = 0; i < GunModels.Length; i++)
                GunModels[i].SetActive(false);

            switch (m_GunState)
            {
                case GunState.Laser:
                {
                    GunModels[0].SetActive(true);
                    UI_Console.Instance.switchBulletType(0);
                    break;
                }
                case GunState.ShootGun:
                {
                    GunModels[1].SetActive(true);
                    UI_Console.Instance.switchBulletType(1);
                    break;
                }
                case GunState.Snipe:
                {
                    GunModels[2].SetActive(true);
                    UI_Console.Instance.switchBulletType(2);
                    break;
                }
            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.volume = 0.056f;
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;


            if (Vector3.Magnitude(desiredMove) > 0 && !m_IsWalking && !is_Tired)
            {
                EnergyTimer += Time.deltaTime;
                if (EnergyTimer >= 0.1f)
                {
                    //print("runloseenergy");
                    m_Energy -= RunEnergy;
                    EnergyTimer = 0;
                    UI_Console.Instance.strengthDamage((int) RunEnergy);
                }
            }


            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;


            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.volume = 0.056f;
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude +
                                (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                               Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }

            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                        (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }

            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            if (is_Tab)
            {
                speed = 0;
                return;
            }

            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running


            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif


            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            if (is_Tired)
            {
                speed = m_WalkSpeed;
            }


            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used

            //跑动放大视野，与枪械瞄准冲突
            //if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            //{
            //    StopAllCoroutines();
            //    StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            //}
        }


        private void RotateView()
        {
            if (Time.timeScale > 0.01f)
            {
                m_MouseLook.LookRotation(transform, m_Camera.transform);
            }
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }

            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}