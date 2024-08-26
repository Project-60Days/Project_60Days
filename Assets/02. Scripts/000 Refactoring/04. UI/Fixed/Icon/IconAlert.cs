using UnityEngine;

public class IconAlert : IconBase
{
    [SerializeField] AlertType type;

    protected override string GetString() => type switch
    {
        AlertType.Note => App.Data.Game.GetString("STR_NEW_NOTICE_ALERT"),
        AlertType.Caution => App.Data.Game.GetString("STR_WARNING_NOTICE_ALERT"),
        _ => "",
    };
}
