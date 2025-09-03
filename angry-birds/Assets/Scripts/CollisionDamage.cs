using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    public float damageThreshold = 5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 Destructible 컴포넌트를 가지고 있는지 확인
        Destructible destructible = collision.gameObject.GetComponent<Destructible>();
        if (destructible != null)
        {
            ApplyDamage(destructible, collision);
        }

        // 자신이 Destructible 컴포넌트를 가지고 있는지 확인
        Destructible selfDestructible = GetComponent<Destructible>();
        if (selfDestructible != null)
        {
            ApplyDamage(selfDestructible, collision);
        }
    }

    private void ApplyDamage(Destructible destructible, Collision2D collision)
    {
        // 충돌의 강도를 계산하여 데미지 적용
        float collisionForce = 0f;
        if (collision.rigidbody != null && collision.relativeVelocity != Vector2.zero)
        {
            collisionForce = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
        }

        Debug.Log($"Collision Force: {collisionForce}"); // 충돌 강도 로그 출력

        if (collisionForce > damageThreshold)
        {
            destructible.TakeDamage(collisionForce);
        }
    }
}