using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossSkillInfo
{
    public float AttackDistance;
    public float Cooltime;
    public float Damage;
    public float Speed;
}

public abstract class BossSkill : MonoBehaviour
{
    public BossSkillInfo Info; // Inspector 에서 옵션 설정 가능하게 함
    public PooledObjectData projectileData; // 풀에서 가져올 발사체 타입
    public string projectileType;
    protected Vector3 moveDirection; // 처음 설정된 이동 방향
    protected Vector3 startPosition; // 시작 위치를 저장
    
    public void Awake()
    {
        projectileType = projectileData.objectTypeName; // 플레이어에서도 설정 하지만, 무기 자체의 returnToPool 을 위해 무기에서도 설정해야함
    }
}