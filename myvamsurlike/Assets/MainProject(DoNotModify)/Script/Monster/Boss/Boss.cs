using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boss : CharacterBase<FSM_Boss>, IDamageable
{
    public Rigidbody target;

    public float Speed = 3;
    public float Hp = 100;
    public float currentHp;

    [NonSerialized] public bool isDead = false;

    public List<BossSkill> BSList; // 보스 스킬 리스트
    public BossSkill currentBS; // 현재 발동할 스킬
    public float BSCooltime; // 현재 발동할 스킬의 쿨타임
    public float currentCooltime; // 실제 쿨타임의 변화를 측정할 변수
    
    private bool isSlowed = false;
    
    public string hitSoundKey;   // 몬스터 피격 사운드 키

    public Animator _animator;
    
    public readonly int MoveHash = Animator.StringToHash("Idle");
    public readonly int DeadHash = Animator.StringToHash("Dead");
    public readonly int SkillHash = Animator.StringToHash("Skill");

    void Awake()
    {
        base.Awake();
        FindPlayer();

        _animator = GetComponentInChildren<Animator>();
        currentHp = Hp;
    }
    
    private void Start()
    {
        SettingBS();
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

    public void GetDamaged(float damage)
    {
        currentHp -= damage;
        // 피격 사운드 재생
        // if (!string.IsNullOrEmpty(hitSoundKey))
        // {
        //     SoundManager.Instance.PlaySound(hitSoundKey);
        // }
        // Debug.Log($"남은 HP {currentHp}");
    }
    
    public void GetSlowed(float time)
    {
        if (!isSlowed) StartCoroutine(Slowed(time));
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

    public void Die()
    {
        isDead = true;
    }
    
    private void SettingBS() // 인스펙터에서 설정된 BS 에 기반하여 쿨타임 설정
    {
        BSCooltime = currentBS.Info.Cooltime;
        currentBS.projectileType = currentBS.projectileData.objectTypeName;
    }

    public bool IsTargetInRange()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance < currentBS.Info.AttackDistance;
    }

    public bool IsCooltiming() // 쿨타임 중인지 확인
    {
        return currentCooltime > 0.0f;
    }

    public void StartCooltime() // 쿨타임 시작
    {
        StartCoroutine(StartCooltime_Internal());

        //ObjectPoolManager.Instance.SpawnFromPool("Skill_A", transform.position, Quaternion.identity);
    }

    IEnumerator StartCooltime_Internal() // 쿨타임 코루틴
    {
        currentCooltime = BSCooltime;
        while (currentCooltime > 0.0f)
        {
            currentCooltime -= Time.deltaTime;
            yield return null;
        }
    }
}

