using UnityEngine;
using System.Collections;

public class M_Magic_3 : M_Skill
{
    #region SingleTon

    public static M_Magic_3 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //마법 3
    public GameObject magic_3_Pref;                                     //Magic_3 프리팹
    private GameObject magic_3_Obj;                                     //Magic_3 오브젝트
    private ObjectPool magic_3_Pool = new ObjectPool();                 //Magic_3 오브젝트풀

    public Transform magic_3_Pivot;                                     //Magic_3가 발동될 Pivot 


    //폭발 이펙트
    public GameObject explosion_Pref;                              //폭발 프리팹
    private ObjectPool explosion_Pool = new ObjectPool();          //폭발 오브젝트풀
    public ObjectPool Explosion_Pool { get { return explosion_Pool; } }


    public float magic_3_ShootDelayTime = 1.0f;                         //Magic_3 발사 대기시간

    public float gravity = -9.8f;
    public float horizontalSpeed = 10.0f;



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Magic_3;

        //스킬 프로퍼티스 설정 
        magic_3_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        //스킬 프로퍼티스 설정 
        explosion_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        magic_3_Pool.CreatePool(magic_3_Pref, 2);
        explosion_Pool.CreatePool(explosion_Pref, 2);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        //안쪽 코너를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작

        
        //마법 3 오브젝트 풀 사용
        magic_3_Obj = magic_3_Pool.UseObject();
        
        magic_3_Obj.transform.position = magic_3_Pivot.position;
        magic_3_Obj.transform.rotation = magic_3_Pivot.rotation;
        
        yield return new WaitForSeconds(skillStatus.AfterDelayTime);
        

        m_Core.IsDoingOther = false;                                            //행동 종료 
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        //활성화된 마법 3이 아직 출발하지 않았다면 삭제
        if(magic_3_Obj.activeSelf &&
            (magic_3_Obj.GetComponent<M_Magic_3_Ctrl>().IsShooted.Equals(false)))
        {
            magic_3_Obj.SetActive(false);
        }
    }
}
