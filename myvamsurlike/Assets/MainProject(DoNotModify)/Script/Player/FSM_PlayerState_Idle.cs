using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_PlayerState_Idle : VMyState<FSM_PlayerState>
{
    public override FSM_PlayerState StateEnum => FSM_PlayerState.FSM_PlayerState_Idle;
    private Player _player;

    void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }

    protected override void EnterState()
    {
        //_player._animator.SetBool("IsMoving", false); // 이동 애니메이션 중지
    }
    
    protected override void ExcuteState()
    {
        if (!_player.IsCooltiming())
        {
            _player.Fsm.ChangeState(FSM_PlayerState.FSM_PlayerState_Skill);
        }
    }
    
    protected override void ExitState()
    {
        
    }
}
