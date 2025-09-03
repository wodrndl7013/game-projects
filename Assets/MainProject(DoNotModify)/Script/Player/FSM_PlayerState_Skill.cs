using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_PlayerState_Skill : VMyState<FSM_PlayerState>
{
    public override FSM_PlayerState StateEnum => FSM_PlayerState.FSM_PlayerState_Skill;
    private Player _player;

    void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }

    protected override void EnterState()
    {
        if (_player._animator != null)
        {
            _player._animator.CrossFade(Player.AttackHash, 0.0f); // 공격 애니메이션 실행
        }
        
        // 오브젝트 스폰
        GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(_player.currentBW.projectileType, transform.position, Quaternion.identity);
        BasicWeapon bw = spawnWeapon.GetComponent<BasicWeapon>();

        // 마우스 위치 계산
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.WorldToScreenPoint(transform.position).z));

        // Y축 회전을 위한 방향 벡터 계산
        Vector3 direction = (mouseWorldPos - transform.position).normalized;
        float yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; // Y축 회전 각도 계산

        // Y축 회전만 적용하여 로테이션 설정
        spawnWeapon.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // 무기 설정
        bw.Setting(_player.AttackDistance, _player.Damage, _player.Speed);
        bw.SetPositionInfo(mouseWorldPos, transform.position);
    }
    
    protected override void ExcuteState()
    {
        _player.Fsm.ChangeState(FSM_PlayerState.FSM_PlayerState_Idle);
    }
    
    protected override void ExitState()
    {
        _player.StartCooltime();
    }
}