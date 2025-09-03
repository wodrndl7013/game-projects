using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_MonsterState_Dead : VMyState<FSM_MonsterState>
{
    public override FSM_MonsterState StateEnum => FSM_MonsterState.FSM_MonsterState_Dead;
    private Monster _monster;

    void Awake()
    {
        base.Awake();
        _monster = GetComponent<Monster>();
    }
    
    protected override void EnterState()
    {
        // 애니메이션 초기화
        // _monster._animator.Rebind();
        // _monster._animator.Update(0.0f);
            
        _monster._animator.CrossFade(_monster.DeadHash, 0.0f); // 스킬 인포에 할당된 애니메이션 재생
        
        StartCoroutine(WaitForAnimationThenReturn()); // Dead 애니메이션 작동을 확인하고 나머지 로직을 처리
    }
    
    protected override void ExcuteState()
    {
        //_monster.Fsm.ChangeState(FSM_MonsterState.FSM_MonsterState_Idle);
    }
    
    protected override void ExitState()
    {
        //MonsterSpawner.Instance.RemoveMonster(gameObject.GetInstanceID()); // !!! 오브젝트를 false or Destroy하는 함수 = 이 함수 전에 모든 기능이 구현되어야 함.
    }
    
    private IEnumerator WaitForAnimationThenReturn()
    {
        yield return null;
        while (true)
        {
            var stateInfo = _monster._animator.GetCurrentAnimatorStateInfo(0); // 애니메이션의 현재 상태 정보를 가져온다
            if (stateInfo.shortNameHash != _monster.DeadHash)
            {
                break; // 현재 애니메이션 상태가 스킬 애니메이션이 아닌 경우 루프를 종료
            }

            if (stateInfo.normalizedTime >= 1.0f)
                
                break; // 애니메이션이 끝난 경우 루프를 종료
            

            yield return null;
        }

        _monster.Die(); // 몬스터 아이템 스폰
        MonsterSpawner.Instance.RemoveMonster(gameObject.GetInstanceID()); // 몬스터 삭제 처리
    }
}