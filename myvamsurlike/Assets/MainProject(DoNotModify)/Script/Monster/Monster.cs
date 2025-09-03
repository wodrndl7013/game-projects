using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CharacterBase<FSM_Monster>, IMonsterType, IDamageable
{
    public MonsterType monsterType; // 인스펙터에서 타입 지정
    public MonsterType MonsterType => monsterType; // 인스펙터에서 지정한 타입을 가져와서 해당 오브젝트의 타입으로 설정, FSM 전역에서 사용할때는 해당 변수를 사용
    
    public Rigidbody target;
    
    public float Speed = 3;
    public float Hp = 10;
    public float currentHp;
    public event Action OnDestroyed; // 이벤트 정의
   
    [NonSerialized]public bool isDead = false;
    
    private bool isSlowed = false;
    
    public string hitSoundKey;   // 몬스터 피격 사운드 키
    
    public Animator _animator;
    
    public readonly int MoveHash = Animator.StringToHash("Idle");
    public readonly int DeadHash = Animator.StringToHash("Dead");

    void Awake()
    {
        base.Awake();
        FindPlayer();
        
        _animator = GetComponentInChildren<Animator>();
        currentHp = Hp;
    }
    
    private void OnEnable()
    {
        currentHp = Hp;
        isDead = false;
        Fsm.ChangeState(FSM_MonsterState.FSM_MonsterState_Idle);
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.GetComponent<Rigidbody>();

            if (target == null)
            {
                Debug.LogWarning("Player object does not have a Rigidbody.");
            }
        }
        else
        {
            Debug.LogWarning("No Player object found.");
        }
    }

    public void TrackingPlayer()
    {
        // 이동 로직
        Vector3 dirVec = target.position - _rb.position;
        dirVec.y = 0; // Y축 제거로 평면에서의 방향 벡터 설정
        Vector3 nextVec = dirVec.normalized * (Speed * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + nextVec);

        // 타겟 바라보기 로직 (Y축 제거된 방향 사용)
        if (dirVec != Vector3.zero) // 방향 벡터가 0이 아닐 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirVec); // 평면상의 타겟 방향으로 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * Speed); // 부드럽게 회전
        }
    }

    public void GetSlowed(float time)
    {
        if (!isSlowed && gameObject.activeInHierarchy) 
            StartCoroutine(Slowed(time));
    }
    
    IEnumerator Slowed(float time)
    {
        isSlowed = true;
        float originSpeed = Speed;
        Speed *= 0.5f;
        yield return new WaitForSeconds(time);
        Speed = originSpeed;
        isSlowed = false; // 슬로우 효과 종료 후 플래그 초기화
    }
    
    public void GetDamaged(float damage)
    {
        currentHp -= damage;
        
        // 피격 사운드 재생
        // if (!string.IsNullOrEmpty(hitSoundKey))
        // {
        //     SoundManager.Instance.PlaySound(hitSoundKey);
        // }
    }

    public void Die()
    {
        isDead = true;
        
        // 이벤트 호출
        OnDestroyed?.Invoke();
        
        // 몬스터 삭제
        MonsterSpawner.Instance.RemoveMonster(gameObject.GetInstanceID());
        
        if (monsterType == MonsterType.Normal)
        {
            RewardManager.Instance.DropExperienceOrItem(transform.position);
        }
        else if (monsterType == MonsterType.Elite)
        {
            RewardManager.Instance.DropAbilityItem(transform.position);
        }
    }
    
    // 이벤트를 추가할 수 있는 메서드
    public void RegisterOnDestroyed(Action callback)
    {
        OnDestroyed += callback;
    }

    // 이벤트를 제거할 수 있는 메서드
    public void UnregisterOnDestroyed(Action callback)
    {
        OnDestroyed -= callback;
    }
}
