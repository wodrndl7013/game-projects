using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Idle : VMyState<FSM_BossState>
{
    public override FSM_BossState StateEnum => FSM_BossState.FSM_BossState_Idle;
    private Boss _boss;

    void Awake()
    {
        base.Awake();
        _boss = GetComponent<Boss>();
    }

    protected override void EnterState()
    {
        _boss._animator.CrossFade(_boss.MoveHash, 0.0f);
    }

    protected override void ExcuteState()
    {
        if (!_boss.IsCooltiming() && _boss.IsTargetInRange())
        {
            _boss.Fsm.ChangeState(FSM_BossState.FSM_BossState_Skill);
        }

        if (!_boss.isDead && _boss.currentHp <= 0)
        {
            _boss.Fsm.ChangeState(FSM_BossState.FSM_BossState_Dead);
        }
    }

    protected override void ExitState()
    {
    }

    protected override void ExcuteState_FixedUpdate()
    {
        _boss.TrackingPlayer();
    }
}