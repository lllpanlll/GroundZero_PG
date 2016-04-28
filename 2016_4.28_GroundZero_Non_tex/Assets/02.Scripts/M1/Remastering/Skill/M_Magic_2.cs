using UnityEngine;
using System.Collections;

public class M_Magic_2 : M_Skill
{
    #region SingleTon

    public static M_Magic_2 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    public GameObject magic_2_Pref;                                 //Magic_2의 오브젝트   
    private GameObject magic_2_Obj;                                 //Magic_2 오브젝트
    private ObjectPool magic_2_Pool = new ObjectPool();             //Magic_2 오브젝트풀 

    public Transform[] magic_2_Pivots;                              //Magic_2가 발동될 Pivot

    public float magic_2_ShootDelayTIme = 1.0f;                     //Magic_2 발사 대기 시간
    public float magic_2_Speed = 3500.0f;                           //Magic_2 발사 스피드
    public float magic_2_MaxDist = 100.0f;                          //Magic_2_최대 사거리


    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Magic_2;

        magic_2_Pool.CreatePool(magic_2_Pref, magic_2_Pivots.Length * 2);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        //스킬 프로퍼티스 설정
        magic_2_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작


        m_Core.Animator.SetTrigger("Magic_2");                                  //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);

        foreach (Transform pivot in magic_2_Pivots)                             //지정된 Pivot에 마법 생성
        {
            //마법 2 오브젝트 풀 사용
            magic_2_Obj = magic_2_Pool.UseObject();
            magic_2_Obj.transform.position = pivot.position;
            magic_2_Obj.transform.rotation = pivot.rotation;
        }

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);    


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
