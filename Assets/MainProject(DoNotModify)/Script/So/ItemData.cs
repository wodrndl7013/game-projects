using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPooledObject", menuName = "SO/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("# Main Info")]
    public string itemName; // 아이템 타입을 구분하기 위한 이름
    public ItemType itemType; // 천사 or 악마
    public Sprite itemIcon; // 보상 선택에서 아이템 이미지를 보여줌 !! 이건 구현해서 나중에 보상선택에서 쓸 수 있게 사용 할 것

    [Header("# Level Data")] 
    public float baseDamage; // 기본 데미지
    public int baseCount; // 기본 생성 숫자
    public float speed; // 기본 발사 속도 or 이동 속도
    public float distance; // 사정거리
    public float cooldown; // 쿨타임
    public float duration; // 지속시간
    
    public float[] damages; // 레벨마다 증가할 데미지 백분율
    public int[] counts; // 레벨마다 추가될 생성 숫자

    public string itemExplain;
    
    private void Awake()
    {
        itemName = this.name;
    }

    public enum ItemType
    {
        Angel,
        Demon
    }
}