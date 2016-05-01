using UnityEngine;
using System.Collections;

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

