using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : CharacterBase<FSM_Player>
{
    // Movement 필드
    public Vector3 inputVec;
    public float speed;
    
    // BasicWeapon 필드
    public List<BasicWeapon> BWList;  // 플레이어가 장착할 수 있는 무기 목록
    public BasicWeapon currentBW;    // 현재 장착된 무기
    public float BWCooltime; // 장착된 무기에 설정된 쿨타임 참조 변수
    public float currentCooltime; // 실제 쿨타임을 관장할 변수
    
    public float AttackDistance;
    public float Damage;
    public float Speed;
    
    // HP 관련 필드 추가
    public float maxHealth = 100f; // 최대 HP
    private float currentHealth;   // 현재 HP
    public PlayerHealthBar healthBar; // PlayerHealthBar 참조
    
    // 아바타
    public GameObject[] Avatars;
    private GameObject currentAvatar;

    public GameObject[] AvatarEffects;
    
    public Animator _animator;
    public static readonly int AttackHash = Animator.StringToHash("Attack");
    public static readonly int MoveHash = Animator.StringToHash("Move");
    public static readonly int IdleHash = Animator.StringToHash("Idle");
    
    void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        SettingBW(); // BasicWeapon 의 이름 설정이 Awake 에서 일어나므로 플레이어가 받아올 때 그보다 늦게 하기 위해 Start 에서 진행
        
        foreach (var avatar in Avatars) avatar.SetActive(false);
        foreach (var effect in AvatarEffects) effect.SetActive(false);
            
        SettingAvatar(0);
        
        currentHealth = maxHealth; // 초기 HP 설정
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth); // 체력바 최대값 설정
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // 체력을 최대 체력으로 제한
        Debug.Log($"Player 체력 회복: {currentHealth}/{maxHealth}");
    }

    private void FixedUpdate()
    {
        Vector3 nextVec = inputVec * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + nextVec);
        
        // Animator에 이동 벡터 크기 전달
        if (_animator != null)
        {
            _animator.SetFloat("MoveSpeed", inputVec.magnitude);
            
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        
            // Attack 상태가 아닌 경우에만 Move/Idle 상태로 전환
            if (!stateInfo.IsName("Attack"))
            {
                if (inputVec.magnitude > 0.01f)
                {
                    _animator.CrossFade("Move", 0.02f); // Move 애니메이션 재생 !! Move 감지가 FixedUpdate 감지 주기 보다 길면, inputVec 가 바뀌어서 감지되는 경우 때문에 애니메이션이 제대로 재생 되지 않고 Move 와 Idle 이 연속적으로 번갈아 재생되는 현상이 있으니 주의할 것.
                }
                else
                {
                    _animator.CrossFade("Idle", 0.1f); // Idle 애니메이션 재생
                }
            }
        }
        
        // 마우스 위치를 기준으로 아바타 회전
        if (currentAvatar != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(
                mousePos.x, 
                mousePos.y, 
                Camera.main.WorldToScreenPoint(_rb.position).z // 플레이어의 z 좌표를 기준
            ));
        
            // Y축 회전을 위한 방향 벡터 계산
            Vector3 direction = (mouseWorldPos - _rb.position).normalized; // 마우스와 캐릭터 간 방향 계산
            if (direction.sqrMagnitude > 0.01f) // 유효한 방향 벡터일 경우에만 회전
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                currentAvatar.transform.rotation = Quaternion.Slerp(
                    currentAvatar.transform.rotation, 
                    targetRotation, 
                    10f * Time.fixedDeltaTime // 회전 속도 조정
                );
            }
        }
    }

    // 인풋 시스템 함수
    void OnMove(InputValue value)
    {
       Vector2 moveInput = value.Get<Vector2>();
       inputVec = new Vector3(moveInput.x, 0, moveInput.y);
    }

    public void TakeDamage(float damage) // 데미지 받을 때
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // HP 바 업데이트
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die() // 플레이어가 죽었을 때
    {
        GameManager.Instance.GameOver();
    }
    
    private void SettingBW() // 인스펙터에서 설정된 BW 에 기반하여 쿨타임 설정
    {
        BWCooltime = currentBW.Info.Cooltime;
        currentBW.projectileType = currentBW.projectileData.objectTypeName; // 프리팹에서 설정이 잘못되어 있을 경우를 방지
    }

    public void SettingBWValue(float attackDistance, float cooltime, float damage, float speed)
    {
        AttackDistance = attackDistance;
        BWCooltime = cooltime;
        Damage = damage;
        Speed = speed;
    }
    
    public bool IsCooltiming() // 쿨타임 중인지 확인
    {
        return currentCooltime > 0.0f;
    }
    
    public void StartCooltime() // 쿨타임 시작
    {
        StartCoroutine(StartCooltime_Internal());
    }

    IEnumerator StartCooltime_Internal() // 쿨타임 코루틴
    {
        currentCooltime = BWCooltime;
        while (currentCooltime > 0.0f)
        {
            currentCooltime -= Time.deltaTime;
            yield return null;
        }
    }

    public void SettingAvatar(int index)
    {
        // 현재 활성화된 아바타가 변경하려는 아바타와 같으면 무시
        if (currentAvatar == Avatars[index]) return;

        // 이전 아바타 비활성화 및 새로운 아바타 활성화
        if (currentAvatar != null) currentAvatar.SetActive(false);
        Avatars[index].SetActive(true);
        currentAvatar = Avatars[index];
        
        // 바뀐 아바타의 애니메이터 불러옴
        _animator = currentAvatar.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"Animator not found on avatar {currentAvatar.name}");
        }

        if (index != 0)
        {
            StartCoroutine(ActiveAvatarEffect(AvatarEffects[index - 1]));
        }
    }

    IEnumerator ActiveAvatarEffect(GameObject effect)
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(1);
        effect.SetActive(false);
    }

    void Update()
    {
        
    }
}
