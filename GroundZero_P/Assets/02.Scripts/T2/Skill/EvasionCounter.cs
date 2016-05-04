using UnityEngine;
using System.Collections;

namespace T2.Skill
{
    /// <summary>
    /// 2016-05-04
    /// 현재 위치에 폭발물을 설치한 뒤 에임방향의 뒤로 이동하는 스킬이다.
    /// 폭발물 설치를 제외하면 기본 구조는 Evasion_E스킬과 비슷하다.
    /// 폭발물은 플레이어의 탄환이나 보스의 콜리더에 닿으면 폭발한다.
    /// </summary>
    public class EvasionCounter : T2.Skill.Skill
    {
        //private T2.MoveCtrl moveCtrl;  //이동상태 변경을 위함
        //private T2.Manager mgr;
        //private CharacterController controller;
        //private Vector3 moveDir = Vector3.zero;

        //private GameObject oPlayerModel;  //플레이어의 모습을 사라지게 하기 위함

        //private int iDecPoint = 10;

        //private float beforeDelayTime = 0.0f;
        //private float afterDelayTime = 0.2f;
        //private float coolTime = 1.2f;

        //private float blinkTime = 0.2f;
        //private float fBlinkDist = 15.0f;
        //private float fBlinkSpeed;

        //public GameObject oGrenade;

        //void Start()
        //{
        //    moveCtrl = GetComponent<T2.MoveCtrl>();
        //    mgr = GetComponent<T2.Manager>();
        //    oPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        //    controller = GetComponent<CharacterController>();

        //    fBlinkSpeed = fBlinkDist / blinkTime;

        //    oGrenade = (GameObject)Instantiate(oGrenade);
        //    oGrenade.SetActive(false);

        //    base.SetCoolTime(this.coolTime);
        //}

        //void Update()
        //{
        //    if (mgr.GetState() == T2.Manager.State.be_Shot)
        //        base.SkillCancel();

        //    if (!base.IsCoolTime())
        //    {
        //        if (Input.GetKeyDown(KeyCode.Alpha1) && !base.IsRunning())
        //            InputCommand(T2.Manager.SkillType.EP, iDecPoint);
        //        if (base.IsBeforeDelay())
        //            BeforeActionDelay(beforeDelayTime);
        //        if (base.IsExecute())
        //            Execute(blinkTime);
        //        if (base.IsAfterDelay())
        //            AfterActionDelay(afterDelayTime);
        //    }
        //    else
        //    {
        //        if (base.IsRunning())
        //        {
        //            base.CoolTimeDelay();
        //        }
        //    }
        //}

        //protected override void InputCommand(T2.Manager.SkillType type, int decPoint)
        //{
        //    base.InputCommand(type, decPoint);
        //}
        //protected override void BeforeActionDelay(float time)
        //{
        //    print("선딜");
        //    base.BeforeActionDelay(time);
        //}
        //protected override void Execute(float time)
        //{
        //    print("액션");

        //    float CamRot = Camera.main.transform.eulerAngles.y;
        //    //플레이어와 캐릭터 모델링을 뒤로 돌린다.
        //    float targetRot = CamRot + 180.0f;

        //    //플레이어와 캐릭터(모델)를 회전시킨다.
        //    transform.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
        //    oPlayerModel.transform.rotation = transform.rotation;

        //    //수류탄 처리
        //    oGrenade.transform.position = transform.position + (transform.up * 2.5f);
        //    oGrenade.SetActive(true);

        //    //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        //    moveCtrl.SetMoveState(T2.MoveCtrl.MoveState.Stop);

        //    moveDir = transform.forward;
        //    //이동 코루틴.
        //    this.StartCoroutine(StartBlinkCounter(time));

        //    base.Execute(time);
        //}
        //protected override void AfterActionDelay(float time)
        //{
        //    print("후딜");
        //    oPlayerModel.transform.position = transform.position;

        //    base.AfterActionDelay(time);
        //}
        //protected override void CoolTimeDelay()
        //{
        //    base.CoolTimeDelay();
        //}

        //IEnumerator StartBlinkCounter(float time)
        //{
        //    oPlayerModel.transform.position = new Vector3(1000.0f, 1000.0f, 1000.0f);

        //    //회피가 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
        //    moveCtrl.SetMoveState(T2.MoveCtrl.MoveState.Stop);

        //    float timeConut = 0;

        //    while (time > timeConut)
        //    {
        //        print("이동");
        //        //transform.Translate(transform.forward * Time.deltaTime * fBlinkSpeed, Space.World);
        //        //moveDir.y -= 20.0f * Time.deltaTime;
        //        controller.Move(moveDir * Time.deltaTime * fBlinkSpeed);
        //        yield return new WaitForEndOfFrame();

        //        timeConut += Time.deltaTime;
        //    }
        //}
    }
}