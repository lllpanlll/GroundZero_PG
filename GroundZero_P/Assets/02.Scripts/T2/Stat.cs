using UnityEngine;
using System.Collections;

namespace T2
{
    /// <summary>
    /// 2016-05-04
    /// 캐릭터의 기본 스탯 정보
    /// 초기에 정해지는 상수값들이다.
    /// </summary>
    public class Stat
    {
        public const int MAX_HP = 1;
        public const int MAX_RECOVERY = 20;
        public const int MAX_CRITICAL_WOUND = 15;

        public const int INIT_DP = 100;
        public const int MAX_DP = 100;
        public const int OVER_DP = 10;

        //public const int AP = 100;
        public const int MAX_AP = 200;
        public const int MAX_MAGAZINE = 60;

        public const float MAX_EP = 100;
        public const int MAX_PP = 100;

        //public const float MAX_AIM_MOVE = 5.0f;
        public const float MAX_RUN_MOVE = 10.0f;
        public const float MAX_SPRINT_MOVE = 20.0f;
    }
}