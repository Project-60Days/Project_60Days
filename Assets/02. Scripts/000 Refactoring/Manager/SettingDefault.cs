using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSetting", menuName = "Scriptable Object/Default Setting Data")]
public class SettingDefault : ScriptableObject
{
    [SerializeField] string path;

    [SerializeField] SoundData sound;
    [SerializeField] ScreenData screen;

    public string Path => path;

    public SoundData Sound => sound;
    public ScreenData Screen => screen;
}
