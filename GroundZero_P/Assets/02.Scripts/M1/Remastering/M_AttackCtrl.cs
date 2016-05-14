using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Attack Collider Control
/// 몬스터의 공격 콜리더 컨트롤. 모든 몬스터의 공격 콜리더에는 이 스크립트가 포함된다.
/// 
/// *코멘트
/// </summary>



public class M_AttackCtrl : MonoBehaviour
{
    private int damage;                 //데미지
    private bool isRigid;               //경직
    //private bool isTotter;              //비틀거림
    //private bool isKnockback;           //넉벡
    //private bool isKnockdown;           //넉다운
    //private bool isSlam;                //날아감


    //공격에 따른 전달 수치 설정
    public void SetAttackProperty(int _damage, bool _isRigid)
    {
        damage = _damage;
        isRigid = _isRigid;
    }
    

    public int GetDamage() { return damage; }
    public bool IsRigid() { return isRigid; }
    //public bool IsTotter() { return isTotter; }
    //public bool IsKnockback() { return isKnockback; }
    //public bool IsKnockdown() { return isKnockdown; }
    //public bool IsSlam() { return isSlam; }
}

