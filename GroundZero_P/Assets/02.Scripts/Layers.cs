using UnityEngine;
using System.Collections;

public class Layers : MonoBehaviour {

    //플레이어
    public const string T_HitCollider = "T_HITCOL";
    public const string T_Invincibility = "T_INVINCIBILITY";

    //몬스터
    public const string MonsterAttkCollider = "MON_ATTKCOL";
    public const string MonsterHitCollider = "MON_HITCOL";

    //오브젝트
    public const string NormalObj = "NORMAL_OBJ";
    public const string Bullet = "BULLET";
    public const string Floor = "FLOOR";
    public const string AlleyTrigger = "ALLEY_TRIGGER";
}
