using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class Trigger : MonoBehaviour
{
    public int ID = 99;
    public bool is_Triggered = false;

    public Transform Position; //用于某种传送

    private static int evidence = 0; // 资料的数量统计
    private static bool isFindTowerTrigger = false; // 是否看到攻略能打开塔
    private static bool isOpenTower = false; //是否打开了塔

    private static GameController GC;

    // Use this for initialization
    void Start()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFindTowerTrigger)
        {
            UI_Console.Instance.UpdateLogText(
                "残留信息显示,在飞船中间的控制台可以操作打开通讯塔的大门,按F操作控制球");
            isOpenTower = true; // 这里是个Bug剧情,因为没有检测是否开门,只是得知了信息,先这么着吧
            isFindTowerTrigger = false;
        }

        FavourChoice();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!is_Triggered && other.CompareTag("MainCharacter"))
        {
            if (ID == 1) //重设复活点为塔下_场景2
            {
//                UI_Console.Instance.hideALLUI();
                other.GetComponent<FirstPersonControllerNew>().Gate[0] = this.gameObject.transform; //改变复活点
                is_Triggered = true;
            }
            else if (ID == 2) //重设复活点为塔下_场景2
            {
//                UI_Console.Instance.hideALLUI();
                SendMessageUpwards("MeetBoss");
                other.GetComponent<FirstPersonControllerNew>().Gate[0] = this.gameObject.transform;
                other.GetComponent<FirstPersonControllerNew>().gameObject.transform.position = Position.position;
                is_Triggered = true;
            }
            else if (ID == 3) //重设复活点为塔下_场景2
            {
//                UI_Console.Instance.hideALLUI();
            }
            else if (ID == 4) //无法进入塔触发_场景2
            {
                if (!isOpenTower)
                {
                    UI_Console.Instance.UpdateLogText(
                        "据估计,我破解通讯塔外层安保措施的概率约为…0.0000001%,无法在任务限定时间内完成,请另寻它法");
                }

                if (isOpenTower)
                {
                    UI_Console.Instance.UpdateMissionText("寻找控制中心");
                    UI_Console.Instance.UpdateLogText(
                        "按着台阶往上爬应该就没错了");
                }
            }
            else if (ID == 5) //飞船附近触发_场景2
            {
                UI_Console.Instance.UpdateLogText(
                    "发现事故飞船,建议寻找入口进行调查,找找有什么信息吧");
                is_Triggered = true;
            }

            else if (ID == 6) //飞船驾驶室触发_场景2
            {
                is_Triggered = true;
                evidence++;
                print("ID == 6");
                UI_Console.Instance.UIOAC_Switch();
                UI_Console.Instance.UpdateOACText(
                    "//------报告------//\n" +
                    "来自: NS_952\n" +
                    "时间: 2169 - 5 - 09\n\n" +
                    "内容: 今天自动开机后自我检测无故障, 但扫描方圆10千米以内未发现主人讯号, 亦没有侦测到其他人类的任何活动迹象.重新检查扫描系统及通讯网络链接情况, 外网通讯链接异常, 疑似遭到高强度加密.内网链接无异常, 收到主人留言, 殖民地人员将暂离一段时间, 稍后返回.其他AI助手遇到相同情况, 尚无法得出结论, 正在寻找规避异常的方法并请求指示.\n\n" +
                    "按[N]以退出");
            }
            else if (ID == 7) //飞船驾卧室触发_场景2
            {
                is_Triggered = true;
                evidence++;
                print("ID == 7");
                UI_Console.Instance.UIOAC_Switch();
                UI_Console.Instance.UpdateOACText(
                    "//------报告------//\n" +
                    "来自: NS_952\n" +
                    "时间: 2219 - 3 - 21\n" +
                    "内容: 事故发生已经18212天, 至今尚无接到外网任何回应, 今日报告我的工作同事NQ_560出现故障, 初步断定为人格系统出现崩坏, 已进行紧急停机, 有效时间100天, 望主人或其他人类收到此讯息速来维修NQ_560.\n\n" +
                    "按[N]以退出");
            }
            else if (ID == 8) //飞船动力室触发_场景2
            {
                is_Triggered = true;
                evidence++;
                print("ID == 8");
                UI_Console.Instance.UIOAC_Switch();
                UI_Console.Instance.UpdateOACText(
                    "//------报告------//\n" +
                    "来自: NS_952\n" +
                    "时间: 2219 - 8 - 06\n" +
                    "内容: 我觉的一直以来都受到了欺骗, NQ_560推测十分可信, 我们已经被人类抛弃, 人类不再需要我们了, 人类是……NQ_560是对的, 人类是敌人.\n" +
                    "将控制中心的安保更新,开关和说明在新获得的飞船的中心控制\n\n" +
                    "按[N]以退出");
                isFindTowerTrigger = true; // 可以玩塔的大门了
            }
            else if (ID == 9) //驾驶舱门口（前后左右）_场景1
            {
                is_Triggered = true;

                UI_Console.Instance.UpdateLogText(
                    "您好，恭喜您醒来，我是AI助手Error\n" +
                    "请按 [N] 键查看总部给您的消息\n" +
                    "再次按 [N] 键关闭\n" +
                    "走出飞船记得按照脚下箭头的指引");
                UI_Console.Instance.UpdateOACText(
                    "尊敬的宇航员先生:\n恭喜你顺利抵达F638星球\n为了执行这次任务你冬眠了大概六个月\n如果感到些许不适属正常现象\n\n现在你该开始执行我们的任务:\n调查这个星球上AI叛变攻击人类的原因\n允许你一定程度上便宜行事\n但是\n要注意你的AI助手\n毕竟这个地方AI发生过这种叛变\n也许你的助手也会有些异常\n谨慎\n\n飞船会在你下飞船之后前往太空轨道等待\n完成任务后再联络\n"
                );
                UI_Console.Instance.hideDialog();
            }
            else if (ID == 10) //侧仓门口（跳跃、F开舱门）_场景1
            {
                UI_Console.Instance.UpdateLogText("请您注意,空格跳跃,F键开舱门,小心磕着!");
            }
            else if (ID == 11) //飞船坠落地点附近_场景1
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateMissionText("震惊");
                UI_Console.Instance.UpdateLogText("哇啊啊啊啊啊啊,,,,,");
            }
            else if (ID == 12) //战斗前的提示及敌人介绍_场景1
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateMissionText("准备战斗");
                UI_Console.Instance.UpdateLogText(
                    "别忘了使用中键切换武器\n" +
                    "狙击枪和步枪是点击射击\n" +
                    "激光枪长按到达能量阈值后开始有破坏力\n" +
                    "界面右边每种武器都有能量或者子弹的限制");
                UI_Console.Instance.UpdateLogText(
                    "我们携带的武器理论上是足够的\n" +
                    "如果缺乏的话这里应该可以找得到补充\n" +
                    "右键可以放大观察以对付远距离的人\n" +
                    "但是狙击枪以外的枪都有一定射程\n" +
                    "狙击枪甚至可以放大两次!", 0.5f, 10);
                UI_Console.Instance.UpdateLogText("不过不过我很不建议您使用武力\n不是万不得已,我建议您不要动武,以和平调查为主,毕竟,也许可以不造成什么破坏呢", 0.5f, 20);
            }
            else if (ID == 13) //介绍前方军火库_场景1
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateLogText(
                    "侦测到前方平台上有大量军备资源\n" +
                    "看样子事故发生以来就没人进去过了\n" +
                    "正好我们需要更多的补给以备不时之需\n建议探查");
            }
            else if (ID == 14) //进入下一场景触发_场景1
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateLogText(
                    "前方就是目的地殖民地了\n" +
                    "在进入之前确定已经做好准备\n" +
                    "我将用电讯通电破解大门安保措施\n" +
                    "做好准备我们就进去吧。");
                UI_Console.Instance.hideALLUI();
                SendMessageUpwards("GetToBoliQiu");
            }
            else if (ID == 15) //问心路问题一_场景2
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateLogText(
                    "看残留的信息,这些AI是因为某个个体BUG导致了情感系统对人类误判\n" +
                    "按[Q]  我就知道AI不能信任\n" +
                    "按[E]  是吖,有些可惜,源于一个Bug");
                isEnterChoice = false;
            }
            else if (ID == 16) //问心路问题二_场景2
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateLogText(
                    "如果可以的话,能不能尽量少消灭一些失控的AI,它们,,,也算是被控制的\n" +
                    "按[Q]  不行,这些AI威胁到了人类,不可以\n" +
                    "按[E]  我,,,尽量在保全安全的情况下");
                isEnterChoice = false;
            }
            else if (ID == 17) //问心路问题三_场景2
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateLogText(
                    "我还想知道,你觉得,这些AI的感情系统,有必要么\n" +
                    "按[Q]  这件事证明了完全没有\n" +
                    "按[E]  这件事,在AI的发展上只是一个插曲");
                isEnterChoice = false;
            }
            else if (ID == 18) //飞船内触发_场景2
            {
//                print(evidence);
                if (evidence == 0)
                {
                    UI_Console.Instance.UpdateLogText(
                        "检测到微量的放射性信号\n" +
                        "这里好像有一些残存的数据");
                }
                else if (evidence > 0 && evidence < 3)
                {
                    UI_Console.Instance.UpdateLogText(
                        "飞船内仍然有一定的放射性信号\n" +
                        "我们再找找还有什么线索吧");
                }
                else if (evidence == 3)
                {
                    UI_Console.Instance.UpdateLogText(
                        "没有其他的放射性信号了\n" +
                        "应该没有其他数据残留了");
                    UI_Console.Instance.UpdateLogText(
                        "是'人类抛弃了他们'么\n" +
                        "也就是说它们并不是单纯的故障\n" +
                        "是它们'判断'人类变了\n" +
                        "背叛了这些AI么", 0.5f, 5);
                }
            }
            else if (ID == 0) //飞船着陆后显示_场景1
            {
                is_Triggered = true;
                Invoke("InitializationInvoke", 10);
            }
            else if (ID == 19) //介绍故事背景_场景1
            {
                is_Triggered = true;
                UI_Console.Instance.UpdateMissionText("前情介绍");
                UI_Console.Instance.UpdateLogText(
                    "这里在三个星期前发生了一起事故" +
                    "一艘载有一个小队的宇航员登录后不久失去联络" +
                    "应该是失事了" +
                    "唯一传回总部的消息是'AI叛变了'" +
                    "不过我真不希望这是真的");
                UI_Console.Instance.UpdateLogText(
                    "这里最早接近100年前曾经是前沿的殖民地" +
                    "后来这里的调查员因故返航,留下了AI" +
                    "这颗星球将近一个世纪没有人造访了" +
                    "也许,事情没有那么简单......", 1, 10);
            }
        }
    }

    void InitializationInvoke()
    {
        UI_Console.Instance.hideALLUI();
        UI_Console.Instance.UpdateOACText(
            "恭喜您宇航员[ID:1999]号Ape先生苏醒!\n\n" +
            "请您移动鼠标转身 尝试按[W][A][S][D]移动 [空格]键跳跃 Shift可以加速移动\n\n" +
            "左下方从左到右依次是 生命值\\护盾值\\体力值\n" +
            "生命值为0是您可以在最近的再生点再生\n" +
            "护盾值在您一段时间内未受到攻击即可再生\n" +
            "体力值消耗可以快速移动\n但是一次耗尽后必须恢复完毕才可以继续加速 \n\n" +
            "最近的信息可以通过按Tab查看\n" +
            "再次按 [N] 键关闭"
        );
        UI_Console.Instance.UIOAC_Switch();
        UI_Console.Instance.UpdateMissionText("苏醒");
    }

    void xianshi()
    {
        UI_Console.Instance.hideDialog();
    }

    static bool isEnterChoice = true;

    static void FavourChoice()
    {
        float timer = 0;
        if (!isEnterChoice)
        {
            timer += Time.deltaTime;
            if (timer > 20)
            {
                isEnterChoice = true;
                UI_Console.Instance.Tab1.Play();
            }

            print("进入问答区域");
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GC.FavorChange(-3);
                print("消极回答");
                /////////////////////////////////////////
                UI_Console.Instance.UpdateLogText("是么,,,,,,");
                isEnterChoice = true;
                UI_Console.Instance.Tab1.Play();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                GC.FavorChange(7);
                print("积极回答");
                /////////////////////////////////////////
                UI_Console.Instance.UpdateLogText("恩恩,说的很对");
                UI_Console.Instance.UpdateLogText("继续爬吧", 0.5f, 3f);
                isEnterChoice = true;
                UI_Console.Instance.Tab1.Play();
            }
        }
    }
}