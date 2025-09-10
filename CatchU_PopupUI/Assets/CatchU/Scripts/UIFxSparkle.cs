using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFxSparkle : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] int count = 24;          // 파티클 개수
    [SerializeField] float radius = 220f;     // 퍼질 반경(px)
    [SerializeField] float lifeMin = 0.35f;
    [SerializeField] float lifeMax = 0.65f;

    [Header("Visual")]
    [SerializeField] Sprite sprite;           // 없으면 기본 사각 Sprite 사용
    [SerializeField] Color color = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Vector2 sizeStart = new Vector2(28, 28);
    [SerializeField] Vector2 sizeEnd = new Vector2(2, 2);
    [SerializeField] float spinMin = -360f;   // 회전(도)
    [SerializeField] float spinMax = 360f;

    [Header("Motion")]
    [SerializeField] float gravity = -260f;   // 살짝 떨어지는 느낌(위로는 양수, 아래로는 음수)
    [SerializeField] float drag = 2.0f;       // 감속

    [Header("Cleanup")]
    [SerializeField] float autoDestroyAfter = 1.2f;

    readonly List<Image> pool = new();
    RectTransform rt;


    private static Sprite _fallbackWhite;
    private static Sprite GetFallbackWhite()
    {
        if (_fallbackWhite == null)
        {
            var tex = Texture2D.whiteTexture; // Unity 내장 1x1 화이트
            _fallbackWhite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                                           new Vector2(0.5f, 0.5f), 100f); // PPU=100
        }
        return _fallbackWhite;
    }

    void Awake()
    {

        rt = transform as RectTransform;
        if (!rt) rt = gameObject.AddComponent<RectTransform>();
        if (!sprite) sprite = GetFallbackWhite();
    }

    void OnEnable()
    {
        StartCoroutine(PlayCo());
        if (autoDestroyAfter > 0f) Destroy(gameObject, autoDestroyAfter);
    }

    IEnumerator PlayCo()
    {
        // 미리 count만큼 자식 Image 생성
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("sparkle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(rt, false);
            var img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            pool.Add(img);
        }

        var coroutines = new List<Coroutine>(count);
        for (int i = 0; i < count; i++)
        {
            coroutines.Add(StartCoroutine(OneParticle(pool[i])));
            // 동시에 뻗되, 너무 한번에 만들지 않게 1~2프레임 텀
            if (i % 8 == 0) yield return null;
        }
        // 모든 파티클 종료까지 대기
        foreach (var c in coroutines) yield return c;
    }

    IEnumerator OneParticle(Image img)
    {
        var rti = img.rectTransform;
        rti.anchoredPosition = Vector2.zero;
        rti.localScale = Vector3.one;

        float life = Random.Range(lifeMin, lifeMax);
        float t = 0f;

        float ang = Random.Range(0f, Mathf.PI * 2f);
        float spd = Random.Range(radius * 2.0f, radius * 3.2f) / life; // life 안에 radius 이상 퍼지도록 속도 조정
        Vector2 v = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * spd;

        float spin = Random.Range(spinMin, spinMax);
        Color col = color; col.a = 0f; img.color = col;

        Vector2 size0 = sizeStart;
        Vector2 size1 = sizeEnd;

        while (t < life)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / life);

            // 위치: 점점 감속 + 중력
            v *= (1f - drag * Time.unscaledDeltaTime);
            v.y += gravity * Time.unscaledDeltaTime;
            rti.anchoredPosition += v * Time.unscaledDeltaTime;

            // 스케일/사이즈 변화
            Vector2 sz = Vector2.Lerp(size0, size1, p);
            rti.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sz.x);
            rti.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sz.y);
            rti.localRotation = Quaternion.Euler(0, 0, spin * p);

            // 알파: 초반 20% 페이드 인, 후반 40% 페이드 아웃
            float a = (p < 0.2f) ? Mathf.InverseLerp(0f, 0.2f, p)
                                 : 1f - Mathf.InverseLerp(0.6f, 1f, p);
            col.a = a;
            img.color = col;

            yield return null;
        }
        // 끝
        img.enabled = false;
    }
}
