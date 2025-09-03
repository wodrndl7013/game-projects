using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_FiendWheel : Weapon
{
    public GameObject FiendWheelPrefab;
    public float orbitRadius = 4f; // 오브젝트가 플레이어로부터 배치되는 반경
    public float orbitSpeed = 4f; // 공전 속도
    public float pulsateSpeed = 2f; // 반경 변화 속도
    public float pulsateAmount = 1f; // 반경 변화 진폭

    private GameObject[] RotateWeaponPrefabs;
    private float[] angleOffsets;

    private int currentCount;
    
    void Start()
    {
        base.Start();
        Setting();
    }

    void FixedUpdate()
    {
        UpdateRotatePositions();
    }

    void Setting()
    {
        currentCount = count;
        orbitSpeed = speed;
        pulsateSpeed = speed * 0.5f;
        
        RotateWeaponPrefabs = new GameObject[count];
        angleOffsets = new float[count];
        //angle에서 i의 갯수에 따라 각도를 정하고 그위치를 스폰값으로 받습니다.
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            Vector3 spawnPosition = CalculateWeaponPosition(angle, orbitRadius);
            RotateWeaponPrefabs[i] = Instantiate(FiendWheelPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
            angleOffsets[i] = angle;
            InputValue(RotateWeaponPrefabs[i]);
        }
    }
    
    private void UpdateRotatePositions() //매 프레임마다 무기들의 위치 갱신하는 함수
    {
        for (int i = 0; i < count; i++)
        {
            if (RotateWeaponPrefabs[i] != null)
            {
                float currentAngle = angleOffsets[i] + (Time.time * orbitSpeed * 360f / (2f * Mathf.PI));
                float currentRadius = orbitRadius + Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
                Vector3 newPosition = CalculateWeaponPosition(currentAngle, currentRadius);
                RotateWeaponPrefabs[i].transform.position = newPosition;
                
                if (i > 0)
                {
                    Vector3 lookDirection = newPosition - RotateWeaponPrefabs[i].transform.position;
                    if (lookDirection != Vector3.zero)
                    {
                        RotateWeaponPrefabs[i].transform.rotation = Quaternion.LookRotation(lookDirection);
                    }
                }
            }
        }
    }
    
    private Vector3 CalculateWeaponPosition(float angle, float radius) //playerTransform.position의 이동에따른 무기의 위치를 다시 계산하는 함수
    {
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius
        );
        return playerTransform.position + offset;
    }

    public override void LevelUpSetting()
    {
        base.LevelUpSetting();
        if (RotateWeaponPrefabs != null)
        {
            if (currentCount != count) // 프리팹 수가 변경되었다면, 기존 무기 모두 파괴하고 다시 세팅
            {
                foreach (var weaponPrefab in RotateWeaponPrefabs)
                {
                    Destroy(weaponPrefab);
                }
                Setting();
                return;
            }

            foreach (var weapon in RotateWeaponPrefabs) // 데미지 값만 변경되었다면 데미지 새로 적용
            {
                InputValue(weapon);
            }
        }
    }
}
