using UnityEngine;

public class HealthItem : ItemBase
{
    public float healAmount = 200f; // 회복할 체력량

    protected override void ApplyEffect(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.Heal(healAmount); // 플레이어 체력 회복
        }
        else
        {
            Debug.LogWarning("Player 스크립트가 없습니다.");
        }
    }
}