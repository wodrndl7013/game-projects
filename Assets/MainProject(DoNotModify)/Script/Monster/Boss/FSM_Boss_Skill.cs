using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Skill : VMyState<FSM_BossState>
{
    public override FSM_BossState StateEnum => FSM_BossState.FSM_BossState_Skill;
    private Boss _boss;

    void Awake()
    {
        base.Awake();
        _boss = GetComponent<Boss>();
    }

    protected override void EnterState()
    {
        GameObject spawnSkill = ObjectPoolManager.Instance.SpawnFromPool(_boss.currentBS.projectileType,
            _boss.target.transform.position, Quaternion.identity);
        
        _boss._animator.CrossFade(_boss.SkillHash, 0.0f);
    }

    protected override void ExcuteState()
    {
        _boss.Fsm.ChangeState(FSM_BossState.FSM_BossState_Idle);
    }

    protected override void ExitState()
    {
        _boss.StartCooltime();
    }
}