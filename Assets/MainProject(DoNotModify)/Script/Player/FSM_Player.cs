using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSM_PlayerState
{
    FSM_PlayerState_Idle,
    FSM_PlayerState_Skill,
}

public class FSM_Player : StateMachine<FSM_PlayerState>
{
}
