using UnityEngine;
using System.Collections;

namespace T2.Skill
{
    public class DroneCall : T2.Skill.Skill
    {
        //    public Transform[] trSetPos = new Transform[6];
        //    public GameObject oDronePref;
        //    private GameObject[] oDrones = new GameObject[6];

        //    private T2.Manager mgr;

        //    //private bool bAction;

        //    private int iDecPoint = 10;

        //    //드론 지속 시간
        //    //private bool bLife = false;
        //    //private float lifeTime = 30.0f, lifeTimer = 0.0f;

        //    private float beforeDelayTime = 0.0f;
        //    private float actionTime = 2.0f;
        //    private float afterDelayTime = 0.0f;
        //    private float coolTime = 0.0f;

        //    //드론! 마지막 일격!
        //    private bool bFinishAttack = false;

        //    void Start()
        //    {
        //        mgr = GetComponent<T2.Manager>();

        //        //드론 위치 생성
        //        for (int i = 0; i < 6; i++)
        //        {
        //            oDrones[i] = (GameObject)Instantiate(oDronePref);
        //            oDrones[i].SetActive(false);
        //            oDrones[i].GetComponent<T2.Pref.Drone>().setSpeed(Random.Range(2.0f, 7.0f));
        //            oDrones[i].GetComponent<T2.Pref.Drone>().setTargetPos(trSetPos[i].position);

        //        }
        //        base.SetCoolTime(this.coolTime);
        //    }

        //    void Update()
        //    {
        //        if (mgr.GetState() == T2.Manager.State.be_Shot)
        //            base.SkillCancel();

        //        if (!base.IsCoolTime())
        //        {
        //            if (Input.GetKeyDown(KeyCode.Alpha2) && !base.IsRunning())
        //                InputCommand(T2.Manager.SkillType.EP, iDecPoint);
        //            if (base.IsBeforeDelay())
        //                BeforeActionDelay(beforeDelayTime);
        //            if (base.IsExecute())
        //                Execute(actionTime);
        //            if (base.IsAfterDelay())
        //                AfterActionDelay(afterDelayTime);
        //        }
        //        else
        //        {
        //            if (base.IsRunning())
        //            {
        //                base.CoolTimeDelay();
        //            }
        //        }
        //    }

        //    protected override void InputCommand(T2.Manager.SkillType type, int decPoint)
        //    {
        //        base.InputCommand(type, decPoint);
        //    }
        //    protected override void BeforeActionDelay(float time)
        //    {
        //        print("선딜");
        //        base.BeforeActionDelay(time);
        //    }
        //    protected override void Execute(float time)
        //    {
        //        print("액션");

        //        for (int i = 0; i < 6; i++)
        //        {
        //            oDrones[i].transform.position = trSetPos[i].position;
        //            oDrones[i].SetActive(true);
        //        }

        //        mgr.ChangeState(T2.Manager.State.idle);



        //        this.StartCoroutine(FollowDrone(time));

        //        base.SetAction(false);
        //        //base.Execute(time);
        //    }
        //    protected override void AfterActionDelay(float time)
        //    {
        //        print("후딜");

        //        for (int i = 0; i < 6; i++)
        //        {
        //            oDrones[i].SetActive(false);
        //        }

        //        base.AfterActionDelay(time);
        //    }
        //    protected override void CoolTimeDelay()
        //    {
        //        base.CoolTimeDelay();
        //    }

        //    IEnumerator FollowDrone(float time)
        //    {

        //        //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.

        //        float timeConut = 0;

        //        while (time > timeConut)
        //        {
        //            print("이동");
        //            for (int i = 0; i < 6; i++)
        //            {
        //                oDrones[i].GetComponent<T2.Pref.Drone>().setTargetPos(trSetPos[i].position);
        //            }
        //            yield return new WaitForEndOfFrame();

        //            timeConut += Time.deltaTime;
        //        }
        //        base.SetAfterDelay(true);
        //    }
        //}
    }
}