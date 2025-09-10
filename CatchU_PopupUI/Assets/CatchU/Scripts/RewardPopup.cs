using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;    // ��Ʈ(������)
    [SerializeField] private RectTransform panel;        // ī�� �г�
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnDim;              // �ٱ� �� Ŭ��
    [SerializeField] private RectTransform border;       // �ܰ�(�ɼ�)
    [SerializeField] private RewardPopupShine shine;     // ���� ����(�ɼ�)
    [SerializeField] private Transform fxAnchor;         // ��ƼŬ ���� ��ġ
    [SerializeField] private Image flash;

    [Header("Timings (��)")]
    [SerializeField] private float dimFadeIn = 0.18f;    // 0��0.6
    [SerializeField] private float panelScaleUp = 0.11f; // 0.86��1.06
    [SerializeField] private float panelScaleDown = 0.09f;// 1.06��1.00
    [SerializeField] private float textStagger = 0.03f;  // �ؽ�Ʈ ���� ����
    [SerializeField] private float textFade = 0.09f;     // �ؽ�Ʈ ���̵� �ð�
    [SerializeField] private float closeDuration = 0.12f;// �ݱ� �ִ�

    [Header("Dim")]
    [SerializeField, Range(0f, 1f)] private float dimTarget = 0.6f;

    [Header("Audio")]
    [SerializeField] private AudioSource sfx;            // 2D UI SFX
    [SerializeField] private UIAudioMixerDuck duck;      // (����) ���� ����ŷ

    // runtime
    private Coroutine _routine;
    private GameObject _spawnedFx;
    private ItemDefinition _item;
    private bool _isClosing = false;
    private bool _revealed = false;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.86f;
        gameObject.SetActive(false);
        if (btnClose) btnClose.onClick.AddListener(() => Hide());
        if (btnDim) btnDim.onClick.AddListener(() => Hide());
        if (flash)
        {
            var c = flash.color; c.a = 0f; flash.color = c;
            var g = flash as Graphic;       // using UnityEngine.UI;
            if (g) g.raycastTarget = false; // �߿�
        }
    }

    public void Apply(ItemDefinition item, RarityStyle style)
    {
        _item = item;                               // �� ���߿� ShowRoutine���� ��� �Ǵ�
        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
        rarityText.text = item.rarity.ToString();
        priceText.text = $"{item.price:N0}";

        nameText.color = style.nameColor;
        rarityText.color = style.rarityColor;
        if (border) border.GetComponent<Image>().color = style.borderColor;

        // FX ������
        if (_spawnedFx) { Destroy(_spawnedFx); _spawnedFx = null; }

        // ����� Epic/Legendary�� �ƴϸ� ������ �н�
        bool isHigh = (item.rarity == Rarity.Epic || item.rarity == Rarity.Legendary);
        if (!isHigh) return;

        // ������ ������ ������ �н� (��� �α� X)
        var prefab = style.highFxPrefab;
        if (!prefab) return;

        // �θ� ���ϱ� (fxAnchor �켱, ������ panel)
        var parent = (Transform)(fxAnchor ? fxAnchor : panel);
        if (!parent) return;

        try
        {
            // parent �����ε� ��� ���� �� SetParent(false)�� ���̱� (ȥ��/�� �̽� ȸ��)
            _spawnedFx = Instantiate(prefab);
            _spawnedFx.name = $"{prefab.name}_FX(instance)";
            _spawnedFx.transform.SetParent(parent, false);

            // UI �������̸� RectTransform ����
            if (_spawnedFx.transform is RectTransform rt)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }
            else
            {
                _spawnedFx.transform.localPosition = Vector3.zero;
                _spawnedFx.transform.localScale = Vector3.one;
            }
        }
        catch (System.Exception)
        {
            // ���� ������: ������ ���� (�ܼ� �������� ����)
            if (_spawnedFx) { Destroy(_spawnedFx); _spawnedFx = null; }
        }
    }

    public void Show(ItemDefinition item, RarityStyle style, bool playRareEmphasis)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;                 // ������Ʈ ��ü�� ���� �־ ����
        if (canvasGroup)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true; // �� �߰�
        }

        _isClosing = false;
        _revealed = false;
        
        if (_routine != null) StopCoroutine(_routine);
        Apply(item, style);
        _routine = StartCoroutine(ShowRoutine(style, playRareEmphasis));
    }
    public void Hide()
    {
        if (!gameObject.activeSelf) return;
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(HideRoutine());
    }


    private IEnumerator FlashCo(float peakAlpha = 0.35f, float dur = 0.12f)
    {
        if (!flash) yield break;
        var c = flash.color;
        float t = 0f;

        // ������ ����� (35%)
        float up = dur * 0.35f;
        while (t < up)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, peakAlpha, t / up);
            flash.color = c;
            yield return null;
        }

        // õõ�� ����� (65%)
        t = 0f; float down = dur * 0.65f;
        while (t < down)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(peakAlpha, 0f, t / down);
            flash.color = c;
            yield return null;
        }
        c.a = 0f; flash.color = c;
    }

    IEnumerator ShowRoutine(RarityStyle style, bool rareEmphasis)
    {
        // 1) Dim in
        float t = 0f;
        while (t < dimFadeIn)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, dimTarget, t / dimFadeIn);
            yield return null;
        }
        canvasGroup.alpha = dimTarget;

        // 2) Scale up/down
        panel.localScale = Vector3.one * 0.86f;
        t = 0f;
        while (t < panelScaleUp)
        {
            t += Time.unscaledDeltaTime;
            float p = t / panelScaleUp;
            float s = Mathf.Lerp(0.86f, 1.06f, EaseOutCubic(p));
            panel.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
        panel.localScale = Vector3.one * 1.06f;

        t = 0f;
        while (t < panelScaleDown)
        {
            t += Time.unscaledDeltaTime;
            float p = t / panelScaleDown;
            float s = Mathf.Lerp(1.06f, 1.0f, EaseInOutCubic(p));
            panel.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
        panel.localScale = Vector3.one;

        // ����
        if (sfx && style.appearSfx) sfx.PlayOneShot(style.appearSfx);

        // 3) �ؽ�Ʈ ���� ���̵�
        var txts = new TMP_Text[] { nameText, rarityText, priceText };
        foreach (var tx in txts) tx.alpha = 0f;

        for (int i = 0; i < txts.Length; i++)
        {
            StartCoroutine(FadeText(txts[i], textFade, 0f, 1f));
            yield return new WaitForSecondsRealtime(textStagger);
        }
        // ��ŵ �� ��� �Ϸ�
        foreach (var tx in txts) tx.alpha = 1f;

        // 4) ����/���̶���Ʈ, ���� �̻� ����
        if (style.useShine && shine) shine.PlayOnce();
        if (rareEmphasis && sfx && style.rareSfx)
        {
            if (duck) duck.DuckFor(0.3f, -8f);
            sfx.PlayOneShot(style.rareSfx);
        }
        if (_spawnedFx) // FX �ڵ� �÷��� ����(Play On Awake)
        {
            // nothing
        }

        if (_item != null && _item.rarity == Rarity.Legendary)
            yield return StartCoroutine(FlashCo());     // ���� ȿ�� 0.12s

        _revealed = true;
    }

    IEnumerator HideRoutine()
    {
        if (_isClosing) yield break;   
        _isClosing = true;
        // scale down + alpha down ����
        float t = 0f;
        Vector3 start = panel.localScale;
        float startA = canvasGroup.alpha;
        while (t < closeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / closeDuration;
            float s = Mathf.Lerp(start.x, 0.92f, EaseInOutCubic(p));
            panel.localScale = new Vector3(s, s, 1f);
            canvasGroup.alpha = Mathf.Lerp(startA, 0f, p);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.86f;

        if (_spawnedFx) { Destroy(_spawnedFx); _spawnedFx = null; }
        if (canvasGroup)
        {
            canvasGroup.blocksRaycasts = false;                     
            canvasGroup.interactable = false;
        }

        _isClosing = false;   // �� ���̽� �÷��� ����
        gameObject.SetActive(false);
    }

    IEnumerator FadeText(TMP_Text t, float dur, float a0, float a1)
    {
        float tt = 0f;
        while (tt < dur)
        {
            tt += Time.unscaledDeltaTime;
            t.alpha = Mathf.Lerp(a0, a1, tt / dur);
            yield return null;
        }
        t.alpha = a1;
    }

    public IEnumerator ShowFor(ItemDefinition item, RarityStyle style, bool playRareEmphasis, float holdSeconds)
    {
        Show(item, style, playRareEmphasis);

        // 1) ������ ������ �巯�� ������ ���
        while (!_revealed) yield return null;

        // 2) ���� �ð� ����
        if (holdSeconds > 0f)
            yield return new WaitForSecondsRealtime(holdSeconds);

        // 3) �ݱ� (�̹� �ٸ� �ڷ�ƾ�� ���� ���� �� ������ ����)
        if (_routine != null) { StopCoroutine(_routine); _routine = null; }
        yield return StartCoroutine(HideRoutine());

        _revealed = true;
    }

    // Easing
    static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    static float EaseInOutCubic(float x) => x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
}

