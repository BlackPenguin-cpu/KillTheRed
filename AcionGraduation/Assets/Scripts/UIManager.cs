using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Image hpBar;
    public Image[] skillIcon;
    public Image[] weaponIcon;
    public Image[] staminaGauge;

    #region 무기아이콘을 위한 변수
    readonly Color semiInvisible = new Color(1, 1, 1, 0.4f);
    readonly Vector3 weaponIconStartPos = new Vector2(0, -17.5f);
    readonly Vector3 weaponIconEndPos = new Vector2(0, -22.5f);
    #endregion
    private Player player;
    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        player = Player.instance;
    }

    public void Update()
    {
        WeaponIconAnim();
    }

    public void WeaponIconAnim()
    {
        for (int i = 0; i < (int)EPlayerWeaponState.End; i++)
        {
            Vector3 pos = weaponIcon[i].rectTransform.anchoredPosition;
            if (i == (int)player.playerWeaponState)
            {
                weaponIcon[i].color = Color.white;
                weaponIcon[i].rectTransform.anchoredPosition = Vector3.Lerp(pos, new Vector3(pos.x, weaponIconStartPos.y), Time.deltaTime * 10);

            }
            else
            {
                weaponIcon[i].color = semiInvisible;
                weaponIcon[i].rectTransform.anchoredPosition = Vector3.Lerp(pos, new Vector3(pos.x, weaponIconEndPos.y), Time.deltaTime * 10);
            }
        }
    }



}
