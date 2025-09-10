using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioMixerDuck : MonoBehaviour
{
    [SerializeField] private AudioSource target;
    private Coroutine _c;

    public void DuckFor(float seconds, float dB)
    {
        if (_c != null) StopCoroutine(_c);
        _c = StartCoroutine(Co(seconds, dB));
    }

    IEnumerator Co(float sec, float dB)
    {
        float orig = target.volume;
        float mul = Mathf.Pow(10f, dB / 20f); // dB ¡æ gain
        target.volume = orig * mul;
        yield return new WaitForSecondsRealtime(sec);
        target.volume = orig;
    }
}
