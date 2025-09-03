using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FSM_BossState
{
    FSM_BossState_Idle,
    FSM_BossState_Skill,
    FSM_BossState_Dead
}

public class FSM_Boss : StateMachine<FSM_BossState>
{
}