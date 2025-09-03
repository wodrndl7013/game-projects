using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_MonsterState_Idle : VMyState<FSM_MonsterState>
{
    public override FSM_MonsterState StateEnum => FSM_MonsterState.FSM_MonsterState_Idle;
    private Monster _monster;

    void Awake()
    {
        base.Awake();
        _monster = GetComponent<Monster>();
    }
    
    protected override void EnterState()
    {
        _monster._animator.CrossFade(_monster.MoveHash, 0.0f);
    }
    
    protected override void ExcuteState()
    {
        if (!_monster.isDead && _monster.currentHp <= 0)
        {
            _monster.Fsm.ChangeState(FSM_MonsterState.FSM_MonsterState_Dead);
        }
    }
    
    protected override void ExitState()
    {
        
    }

    protected override void ExcuteState_FixedUpdate()
    {
        _monster.TrackingPlayer();
    }
}
