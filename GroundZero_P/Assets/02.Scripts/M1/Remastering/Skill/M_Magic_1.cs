using UnityEngine;
using System.Collections;

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
    public float magic_1_ShootDelayTIme = 1.0f;                     //Magic_1 발사 대기 시간
    public float magic_1_Speed = 15.0f;                             //Magic_1 발사 스피드
    public float magic_1_MaxDist = 100.0f;                          //Magic_1_최대 사거리



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Magic_1;

        //Magic_1 Pivot 찾음
        magic_1_Pivots = GameObject.Find("Magic_1_Pivots").GetComponentsInChildren<Transform>();
        
        magic_1_Pool.CreatePool(magic_1_Pref, (magic_1_Pivots.Length -1) * 2);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        //스킬 프로퍼티스 설정
        magic_1_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작

        Debug.Log("Magic_1 Start");


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
        

        m_Core.Animator.SetTrigger("Magic_1");                                  //애니메이션 실행

        for (int i = 0; i < 10; i++)                                            //랜덤한 순서의 위치로 마법 발사
        {
            //마법 1 오브젝트 풀 사용
            magic_1_Obj = magic_1_Pool.UseObject();
            magic_1_Obj.transform.position = magic_1_Pivots[createNum[i]].position;
            magic_1_Obj.transform.rotation = magic_1_Pivots[createNum[i]].rotation;
            
            yield return new WaitForSeconds(magic_1_CreateDelayTime);           //다음 마법 생성까지 딜레이
        }

        Debug.Log("Magic_1 Create Fin"); 

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);

        Debug.Log("Magic_1 Delay Fin");

        m_Core.IsDoingOther = false;                                            //행동 종료 
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        //<<추가>>  마법 생성 중에 캔슬되면 생성 중단? -> 아 근데 아마 저절로 잘 끊길 거 같기도?
        //<<추가>>  아직 출발하지 않은 애들은 캔슬 시 삭제된대. -> 출발했나 안했나 bool변수를 Ctrl쪽에서 설정
        //<<추가>>  오브젝트 풀에서 활성화 된 애들 가져와서 bool변수 가져와보고 이쪽에서 SetActive false 설정하면 되려나
    }
}
