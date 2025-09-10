using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private GachaStateMachine fsm;

    [Header("Pools")]
    [SerializeField] private List<ItemDefinition> commons = new();
    [SerializeField] private List<ItemDefinition> rares = new();
    [SerializeField] private List<ItemDefinition> epics = new();
    [SerializeField] private List<ItemDefinition> legendaries = new();

    [Header("Styles")]
    [SerializeField] private RarityStyle styleCommon;
    [SerializeField] private RarityStyle styleRare;
    [SerializeField] private RarityStyle styleEpic;
    [SerializeField] private RarityStyle styleLegendary;

    [Header("Rates (%)")]
    [Range(0, 100)] public int commonRate = 50;
    [Range(0, 100)] public int rareRate = 20;
    [Range(0, 100)] public int epicRate = 20;
    [Range(0, 100)] public int legendaryRate = 10;

    [Header("UI")]
    [SerializeField] private RewardPopup popup;
    [SerializeField] private Button btnRollOne;
    [SerializeField] private Button btnRollTen;

    [Header("Options")]
    [SerializeField] private bool qualityFxOn = true; // ������ Off

    private Coroutine _co;

    void Awake()
    {
        if (!fsm) fsm = GetComponent<GachaStateMachine>();
        if (btnRollOne) btnRollOne.onClick.AddListener(RollOne);
        if (btnRollTen) btnRollTen.onClick.AddListener(RollTen);
    }

    // ���� �̱�
    public void RollOne()
    {
        if (fsm.IsBusy) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoRollOne());
    }

    // 10�� �̱�
    public void RollTen()
    {
        if (fsm.IsBusy) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoRollTen());
    }

    IEnumerator CoRollOne()
    {
        fsm.Set(GachaState.Rolling);

        var rarity = PickRarity();
        var item = PickItem(rarity);
        var style = GetStyle(rarity);

        bool rareEmphasis = rarity >= Rarity.Rare; // Rare �̻� ���� ����
        fsm.Set(GachaState.Reveal);
        float hold = rarity switch
        {
            Rarity.Legendary => 1.2f,
            Rarity.Epic => 1.0f,
            Rarity.Rare => 0.75f,
            _ => 0.65f,
        }; ;
        yield return StartCoroutine(popup.ShowFor(item, style, rareEmphasis, hold));

        fsm.Set(GachaState.Idle);
    }

    IEnumerator CoRollTen()
    {
        fsm.Set(GachaState.Rolling);

        var results = new List<(ItemDefinition, Rarity)>(10);
        for (int i = 0; i < 10; i++)
        {
            var rr = PickRarity();
            results.Add((PickItem(rr), rr));
        }

        // Reveal 10��
        fsm.Set(GachaState.Reveal);
        for (int i = 0; i < results.Count; i++)
        {
            var (item, r) = results[i];
            var style = GetStyle(r);
            bool rareEmphasis = r >= Rarity.Rare;

            float hold = r switch
        {
            Rarity.Legendary => 1.2f,
            Rarity.Epic      => 1.0f,
            Rarity.Rare      => 0.75f,
            _                => 0.65f,
        };
            // �������ְ��������ݱ⡱�� �� �徿 Ȯ���� ������ ��������
            yield return StartCoroutine(popup.ShowFor(item, style, rareEmphasis, hold));
        }

        // ��� �г��� �̹� ���� ���� ���̴� ����(�ʿ� �� �߰�)
        fsm.Set(GachaState.Idle);
    }

    // === ��ƿ ===
    private Rarity PickRarity()
    {
        int total = commonRate + rareRate + epicRate + legendaryRate;
        int r = Random.Range(0, total);
        if (r < commonRate) return Rarity.Common;
        r -= commonRate;
        if (r < rareRate) return Rarity.Rare;
        r -= rareRate;
        if (r < epicRate) return Rarity.Epic;
        return Rarity.Legendary;
    }

    private ItemDefinition PickItem(Rarity rarity)
    {
        var list = rarity switch
        {
            Rarity.Common => commons,
            Rarity.Rare => rares,
            Rarity.Epic => epics,
            Rarity.Legendary => legendaries,
            _ => commons
        };
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    private RarityStyle GetStyle(Rarity r)
    {
        return r switch
        {
            Rarity.Common => styleCommon,
            Rarity.Rare => styleRare,
            Rarity.Epic => styleEpic,
            Rarity.Legendary => styleLegendary,
            _ => styleCommon
        };
    }

    // ����� ���(��ƼŬ/���� ���� ���� ��å�� ��Ÿ�� �ڻ꿡�� ���� ��õ)
    public void SetQualityFx(bool on) => qualityFxOn = on;
}
