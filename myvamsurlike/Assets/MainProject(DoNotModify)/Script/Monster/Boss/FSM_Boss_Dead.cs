using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class FSM_Boss_Dead : VMyState<FSM_BossState>
{
    public override FSM_BossState StateEnum => FSM_BossState.FSM_BossState_Dead;
    private Boss _boss;

    void Awake()
    {
        base.Awake();
        _boss = GetComponent<Boss>();
    }

    protected override void EnterState()
    {
        _boss._animator.CrossFade(_boss.DeadHash, 0.0f);
        _boss.Die();
        StartCoroutine(GameOver());
    }

    protected override void ExcuteState()
    {
    }

    protected override void ExitState()
    {
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.GameOver();
    }
}