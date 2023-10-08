﻿using BepInEx.Configuration;
using Core;
using UnityEngine;

namespace kft.oribf.uilib;

public class ToggleCustomSettingsAction : MonoBehaviour
{
    public void Awake()
    {
        ToggleSettingsAction componentInChildren = base.GetComponentInChildren<ToggleSettingsAction>();
        OnSound = componentInChildren.OnSound;
        OffSound = componentInChildren.OffSound;
        Destroy(componentInChildren);
    }

    private void PlaySound(bool on)
    {
        if (on && OnSound)
        {
            Sound.Play(OnSound.GetSound(null), base.transform.position, null);
            return;
        }
        if (OffSound && !on)
        {
            Sound.Play(OffSound.GetSound(null), base.transform.position, null);
        }
    }

    public void Toggle()
    {
        SetSetting(!IsEnabled);
        PlaySound(IsEnabled);
        Setting.Value = IsEnabled;
    }

    public void SetSetting(bool enabled)
    {
        MessageBox.SetMessage(new MessageDescriptor(enabled ? "ON" : "OFF"));
        IsEnabled = enabled;
    }

    public void Init()
    {
        MessageBox = base.transform.FindChild("text/stateText").GetComponent<MessageBox>();
        SetSetting(Setting.Value);
    }

    public SoundProvider OnSound;

    public SoundProvider OffSound;

    public MessageBox MessageBox;

    public bool IsEnabled;

    public ConfigEntry<bool> Setting;
}
