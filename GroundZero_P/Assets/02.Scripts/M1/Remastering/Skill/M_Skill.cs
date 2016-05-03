using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Skill Super Class
/// 
/// *코멘트
/// </summary>



public enum M_SkillCoad
{
    BodyPress,
    Breath,
    Magic_1,
    Magic_2,
    Magic_3,
    JumpAttack,
    AlleyForeFootPress,
    AlleyBreath_1,
    AlleyBreath_2,
    EnergyEmission
}



[System.Serializable]
public class M_SkillStatus
{
    private M_SkillCoad skillCode;
    public M_SkillCoad SkillCode
    {
        get { return skillCode; }
        set { skillCode = value; }
    }

    public int damage;

    public float beforeDelayTime;
    public float curTime;
    public float AfterDelayTime;
}



public class M_Skill : MonoBehaviour
{
    protected M_AICore m_Core;                              //AI Core

    public M_SkillStatus skillStatus;                       //스킬 기본 Status

    public float lookRotationTime = 0.5f;                   //플레이어를 돌아보는 시간 



    void Start()
    {
        m_Core = GameObject.FindGameObjectWithTag(Tags.Monster).GetComponent<M_AICore>();  //AI Core 가져오기

        InitSkill();                                        //스킬 초기화
    }



    public virtual void InitSkill() { }                     //최초 스킬 초기화
    public virtual IEnumerator UseSkill(Vector3 target)   //스킬 사용
    { yield return null; } 
    public virtual void CancelSkill() { }                   //스킬 캔슬 시 처리



    //Transfrom을 rotateTime동안 Pos를 향하게 회전 -> 추후, 몸 회전과 구분되는, 고개만 회전하는 바라보기 추가 예정
    protected IEnumerator RotateToPoint(Transform tr, Vector3 pos, float rotateTime)
    {
        m_Core.IsDoingOther = true;


        Quaternion startRotation = tr.rotation;
        float rotateGage = 0;

        Vector3 trToPosVector = Vector3.Normalize(pos - tr.position);
        Quaternion tempRot = Quaternion.LookRotation(trToPosVector);
        tempRot.eulerAngles = new Vector3(0, tempRot.eulerAngles.y, 0);


        //->위치를 바라보도록 회전
        while (rotateGage <= 1)
        {
            tr.rotation = Quaternion.Slerp(startRotation, tempRot, rotateGage);

            rotateGage += 0.01f / rotateTime;

            yield return new WaitForSeconds(0.01f);
        }


        m_Core.IsDoingOther = false;
    }
}
