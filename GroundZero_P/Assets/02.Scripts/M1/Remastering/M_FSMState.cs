using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster FSM SuperClass
/// 스킬에서 공통으로 사용될 변수와 인터페이스 정의.
/// 
/// *코멘트
/// </summary>



public class M_FSMState : MonoBehaviour
{
    protected M_AICore m_Core;

    protected M_TopState topState;
    public M_TopState TopState { get { return topState; }}

    public float rigidTime = 1.0f;



    //Start
    void Start()
    {
        m_Core = GetComponent<M_AICore>();                  //AI Core 가져오기
        FSMInitialize();                                    //상태 초기화
    }



    public virtual void FSMInitialize() { }                 //상태 초기화

    public virtual void FSMUpdate() { }                     //상태 Update
    public virtual void FSMMustUpdate() { }                 //상태 매 프레임 Update

    public virtual void Enter() { }                         //상태 진입
    public virtual void Exit() { }                          //상태 탈출



    #region 공통 행동

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

    
    //MonRigid  몬스터 경직 
    public virtual void MonRigid()
    {
        m_Core.NvAgent.Stop();
        m_Core.Animator.SetTrigger("Rigid");
        m_Core.Animator.SetBool("IsRunning", false);

        StartCoroutine(DoNothingInRigid());
    }

    protected IEnumerator DoNothingInRigid()
    {
        m_Core.IsDoingOther = true;
        
        yield return new WaitForSeconds(rigidTime);
        
        m_Core.IsDoingOther = false;
        m_Core.IsRigid = false;
    }

    #endregion
}
