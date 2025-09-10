using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;    // 루트(반투명)
    [SerializeField] private RectTransform panel;        // 카드 패널
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnDim;              // 바깥 딤 클릭
    [SerializeField] private RectTransform border;       // 외곽(옵션)
    [SerializeField] private RewardPopupShine shine;     // 샤인 스윕(옵션)
    [SerializeField] private Transform fxAnchor;         // 파티클 붙일 위치
    [SerializeField] private Image flash;

    [Header("Timings (초)")]
    [SerializeField] private float dimFadeIn = 0.18f;    // 0→0.6
    [SerializeField] private float panelScaleUp = 0.11f; // 0.86→1.06
    [SerializeField] private float panelScaleDown = 0.09f;// 1.06→1.00
    [SerializeField] private float textStagger = 0.03f;  // 텍스트 순차 간격
    [SerializeField] private float textFade = 0.09f;     // 텍스트 페이드 시간
    [SerializeField] private float closeDuration = 0.12f;// 닫기 애니

    [Header("Dim")]
    [SerializeField, Range(0f, 1f)] private float dimTarget = 0.6f;

    [Header("Audio")]
    [SerializeField] private AudioSource sfx;            // 2D UI SFX
    [SerializeField] private UIAudioMixerDuck duck;      // (선택) 볼륨 다이킹

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
            if (g) g.raycastTarget = false; // 중요
        }
    }

    public void Apply(ItemDefinition item, RarityStyle style)
    {
        _item = item;                               // ★ 나중에 ShowRoutine에서 등급 판단
        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
        rarityText.text = item.rarity.ToString();
        priceText.text = $"{item.price:N0}";

        nameText.color = style.nameColor;
        rarityText.color = style.rarityColor;
        if (border) border.GetComponent<Image>().color = style.borderColor;

        // FX 프리셋
        if (_spawnedFx) { Destroy(_spawnedFx); _spawnedFx = null; }

        // 등급이 Epic/Legendary가 아니면 조용히 패스
        bool isHigh = (item.rarity == Rarity.Epic || item.rarity == Rarity.Legendary);
        if (!isHigh) return;

        // 프리팹 없으면 조용히 패스 (경고 로그 X)
        var prefab = style.highFxPrefab;
        if (!prefab) return;

        // 부모 정하기 (fxAnchor 우선, 없으면 panel)
        var parent = (Transform)(fxAnchor ? fxAnchor : panel);
        if (!parent) return;

        try
        {
            // parent 오버로드 대신 생성 후 SetParent(false)로 붙이기 (혼종/널 이슈 회피)
            _spawnedFx = Instantiate(prefab);
            _spawnedFx.name = $"{prefab.name}_FX(instance)";
            _spawnedFx.transform.SetParent(parent, false);

            // UI 프리팹이면 RectTransform 정렬
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
            // 면접 과제용: 조용히 무시 (콘솔 더럽히지 않음)
            if (_spawnedFx) { Destroy(_spawnedFx); _spawnedFx = null; }
        }
    }

    public void Show(ItemDefinition item, RarityStyle style, bool playRareEmphasis)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;                 // 컴포넌트 자체가 꺼져 있어도 켜줌
        if (canvasGroup)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true; // ★ 추가
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

        // 빠르게 밝아짐 (35%)
        float up = dur * 0.35f;
        while (t < up)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, peakAlpha, t / up);
            flash.color = c;
            yield return null;
        }

        // 천천히 사라짐 (65%)
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

        // 사운드
        if (sfx && style.appearSfx) sfx.PlayOneShot(style.appearSfx);

        // 3) 텍스트 순차 페이드
        var txts = new TMP_Text[] { nameText, rarityText, priceText };
        foreach (var tx in txts) tx.alpha = 0f;

        for (int i = 0; i < txts.Length; i++)
        {
            StartCoroutine(FadeText(txts[i], textFade, 0f, 1f));
            yield return new WaitForSecondsRealtime(textStagger);
        }
        // 스킵 시 즉시 완료
        foreach (var tx in txts) tx.alpha = 1f;

        // 4) 샤인/하이라이트, 레어 이상 강조
        if (style.useShine && shine) shine.PlayOnce();
        if (rareEmphasis && sfx && style.rareSfx)
        {
            if (duck) duck.DuckFor(0.3f, -8f);
            sfx.PlayOneShot(style.rareSfx);
        }
        if (_spawnedFx) // FX 자동 플레이 전제(Play On Awake)
        {
            // nothing
        }

        if (_item != null && _item.rarity == Rarity.Legendary)
            yield return StartCoroutine(FlashCo());     // 전광 효과 0.12s

        _revealed = true;
    }

    IEnumerator HideRoutine()
    {
        if (_isClosing) yield break;   
        _isClosing = true;
        // scale down + alpha down 동시
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

        _isClosing = false;   // ★ 레이스 플래그 해제
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

        // 1) 연출이 완전히 드러날 때까지 대기
        while (!_revealed) yield return null;

        // 2) 지정 시간 유지
        if (holdSeconds > 0f)
            yield return new WaitForSecondsRealtime(holdSeconds);

        // 3) 닫기 (이미 다른 코루틴이 돌고 있을 수 있으니 정리)
        if (_routine != null) { StopCoroutine(_routine); _routine = null; }
        yield return StartCoroutine(HideRoutine());

        _revealed = true;
    }

    // Easing
    static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    static float EaseInOutCubic(float x) => x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
}

