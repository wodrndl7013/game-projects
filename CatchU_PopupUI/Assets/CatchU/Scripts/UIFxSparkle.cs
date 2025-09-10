using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFxSparkle : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] int count = 24;          // ��ƼŬ ����
    [SerializeField] float radius = 220f;     // ���� �ݰ�(px)
    [SerializeField] float lifeMin = 0.35f;
    [SerializeField] float lifeMax = 0.65f;

    [Header("Visual")]
    [SerializeField] Sprite sprite;           // ������ �⺻ �簢 Sprite ���
    [SerializeField] Color color = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Vector2 sizeStart = new Vector2(28, 28);
    [SerializeField] Vector2 sizeEnd = new Vector2(2, 2);
    [SerializeField] float spinMin = -360f;   // ȸ��(��)
    [SerializeField] float spinMax = 360f;

    [Header("Motion")]
    [SerializeField] float gravity = -260f;   // ��¦ �������� ����(���δ� ���, �Ʒ��δ� ����)
    [SerializeField] float drag = 2.0f;       // ����

    [Header("Cleanup")]
    [SerializeField] float autoDestroyAfter = 1.2f;

    readonly List<Image> pool = new();
    RectTransform rt;


    private static Sprite _fallbackWhite;
    private static Sprite GetFallbackWhite()
    {
        if (_fallbackWhite == null)
        {
            var tex = Texture2D.whiteTexture; // Unity ���� 1x1 ȭ��Ʈ
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
        // �̸� count��ŭ �ڽ� Image ����
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
            // ���ÿ� ����, �ʹ� �ѹ��� ������ �ʰ� 1~2������ ��
            if (i % 8 == 0) yield return null;
        }
        // ��� ��ƼŬ ������� ���
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
        float spd = Random.Range(radius * 2.0f, radius * 3.2f) / life; // life �ȿ� radius �̻� �������� �ӵ� ����
        Vector2 v = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * spd;

        float spin = Random.Range(spinMin, spinMax);
        Color col = color; col.a = 0f; img.color = col;

        Vector2 size0 = sizeStart;
        Vector2 size1 = sizeEnd;

        while (t < life)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / life);

            // ��ġ: ���� ���� + �߷�
            v *= (1f - drag * Time.unscaledDeltaTime);
            v.y += gravity * Time.unscaledDeltaTime;
            rti.anchoredPosition += v * Time.unscaledDeltaTime;

            // ������/������ ��ȭ
            Vector2 sz = Vector2.Lerp(size0, size1, p);
            rti.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sz.x);
            rti.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sz.y);
            rti.localRotation = Quaternion.Euler(0, 0, spin * p);

            // ����: �ʹ� 20% ���̵� ��, �Ĺ� 40% ���̵� �ƿ�
            float a = (p < 0.2f) ? Mathf.InverseLerp(0f, 0.2f, p)
                                 : 1f - Mathf.InverseLerp(0.6f, 1f, p);
            col.a = a;
            img.color = col;

            yield return null;
        }
        // ��
        img.enabled = false;
    }
}
