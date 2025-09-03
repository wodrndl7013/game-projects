using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    public float damageMultiplier = 0.5f;
    public AudioSource audioSource;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 Destructible 컴포넌트를 가지고 있는지 확인
        Destructible destructible = collision.gameObject.GetComponent<Destructible>();
        if (destructible != null)
        {
            // 충돌의 강도를 계산하여 데미지 적용
            float collisionForce = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
            float damage = collisionForce * damageMultiplier;
            destructible.TakeDamage(damage);
        } 
    }
}
