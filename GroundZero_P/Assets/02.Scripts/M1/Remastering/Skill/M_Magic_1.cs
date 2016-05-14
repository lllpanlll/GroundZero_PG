using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Magic 1
/// 마법 1. 10개의 지점에서 랜덤한 순서로 생성된 발사체가 발사.
/// 
/// *코멘트
/// </summary>



public class M_Magic_1 : M_Skill
{
    #region SingleTon

    public static M_Magic_1 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //마법 1
    public GameObject magic_1_Pref;                                 //Magic_1 프리팹  
    private GameObject magic_1_Obj;                                 //Magic_1 오브젝트
    private ObjectPool magic_1_Pool = new ObjectPool();             //Magic_1 오브젝트풀

    public Transform[] magic_1_Pivots;                              //Magic_1가 발동될 Pivot 

    public float magic_1_CreateDelayTime = 0.05f;                   //Magic_1 생성 시간 차
    public float magic_1_ShootDelayTime = 1.0f;                     //Magic_1 발사 대기 시간
    public float magic_1_TraceSpeed = 7.0f;                         //Magic_1 원래 Pivot 추적 Speed
    public float magic_1_Speed = 15.0f;                             //Magic_1 발사 스피드
    public float magic_1_MaxDist = 100.0f;                          //Magic_1 최대 사거리

    public float magic_1_CastingMonSpeed = 3.0f;                    //Magic_1 캐스팅 동안 몬스터 속력
    private float magic_1_OriginMonSpeed = 20.0f;                   //Magic_1 원래의 몬스터 속력



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Magic_1;

        //스킬 프로퍼티스 설정
        magic_1_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        //Magic_1 Pivot 찾음
        magic_1_Pivots = GameObject.Find("Magic_1_Pivots").GetComponentsInChildren<Transform>();
        
        magic_1_Pool.CreatePool(magic_1_Pref, (magic_1_Pivots.Length -1) * 2);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작


        //원래 속도 저장
        magic_1_OriginMonSpeed = m_Core.NvAgent.speed;                  

        //이 스킬은 특별히 본체의 속도를 제어합니다.
        m_Core.NvAgent.speed = magic_1_CastingMonSpeed;
        

        //랜덤 순서 설정
        int[] createNum = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };                    //인덱스 숫자를 준비 (0번은 Pivot들의 부모라 제외)
        int index, oldNum;

        Random.seed = (int)System.DateTime.Now.Ticks;                           //랜덤값의 시드를 무작위로 만든다
        for (int i = 0; i < 10; i++)                                            //숫자를 무작위로 섞는다
        {
            index = Random.Range(0, 9);

            oldNum = createNum[i];
            createNum[i] = createNum[index];
            createNum[index] = oldNum;
        }
        
        //m_Core.Animator.SetTrigger("Magic_1");                                  //애니메이션 실행 <- 달리기 애니메이션 쓰기로 해서 일단은...

        for (int i = 0; i < 10; i++)                                            //랜덤한 순서의 위치로 마법 발사
        {
            //마법 1 오브젝트 풀 사용
            magic_1_Obj = magic_1_Pool.UseObject();
            
            magic_1_Obj.GetComponent<M_Magic_1_Ctrl>().ShootPointTr = magic_1_Pivots[createNum[i]];
            magic_1_Obj.transform.position = magic_1_Pivots[createNum[i]].position;
            magic_1_Obj.transform.rotation = magic_1_Pivots[createNum[i]].rotation;
            
            yield return new WaitForSeconds(magic_1_CreateDelayTime);           //다음 마법 생성까지 딜레이
        }

        //이 스킬은 특별히 본체의 속도를 제어합니다.
        m_Core.NvAgent.speed = magic_1_OriginMonSpeed;
        
        yield return new WaitForSeconds(skillStatus.AfterDelayTime);
         

        m_Core.IsDoingOther = false;                                            //행동 종료  

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        int num = magic_1_Pool.ObjectNum;                           
        GameObject tempObj = null;

        for (int i = 0; i < num; ++i)                                           //오브젝트 풀 수 만큼 반복
        {
            tempObj = magic_1_Pool.DetectiveAllObject(i);                       //해당 인덱스의 오브젝트를 빼옴

            if(tempObj &&                                                       //가져온 오브젝트가 활성화되어 있으며
                !tempObj.GetComponent<M_Magic_1_Ctrl>().IsShooted)              //오브젝트가 아직 발사되지 않았다면
            {
                tempObj.SetActive(false);                                       //오브젝트 비활성화
            }
        }
    }
}
