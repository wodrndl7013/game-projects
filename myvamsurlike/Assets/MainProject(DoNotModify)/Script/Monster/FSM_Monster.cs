using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSM_MonsterState
{
    FSM_MonsterState_Idle,
    FSM_MonsterState_Dead,
}

public class FSM_Monster : StateMachine<FSM_MonsterState>
{
}
