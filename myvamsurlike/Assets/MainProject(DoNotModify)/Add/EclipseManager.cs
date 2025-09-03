using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EclipseManager : Singleton<EclipseManager>
{
    private Player _player;
    public Image eclipseImage;
    public Sprite[] images;
    private bool avatarChanged = false;

    void Start()
    {
        _player = WeaponManager.Instance._player;
        eclipseImage.sprite = images[0];
    }
    public void CheckingEclipse(float angelProb)
    {
        eclipseImage.sprite = images[ChangeEclipse(angelProb)];
    }

    private int ChangeEclipse(float angelProb)
    {
        if (angelProb >= 0.9f)
        {
            ChangeAvatar(angelProb);
            return 4;
        }
        else if (angelProb >= 0.8f) return 3;
        else if (angelProb >= 0.7f) return 2;
        else if (angelProb >= 0.6f) return 1;
        else if (angelProb >= 0.45f)
        {
            if (avatarChanged)
            {
                ChangeAvatar(angelProb);
            }
            return 0;
        }
        else if (angelProb >= 0.35f) return 5;
        else if (angelProb >= 0.25f) return 6;
        else if (angelProb >= 0.15f) return 7;
        else if (angelProb >= 0.05f)
        {
            ChangeAvatar(angelProb);
            return 8;
        }
        return 0;
    }

    private void ChangeAvatar(float angelProb)
    {
        if (angelProb >= 0.9f) _player.SettingAvatar(1);
        else if (angelProb >= 0.45f) _player.SettingAvatar(3);
        else if (angelProb >= 0.05f) _player.SettingAvatar(2);
        avatarChanged = true;
    }
}
