using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Image hpBar;
    public Image[] skillIcon;
    public Image[] weaponIcon;
    public Image[] staminaGauge;

    public int lastStaminaInt;
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
        BaseUIApply();
    }

    private void BaseUIApply()
    {
        hpBar.fillAmount = player.Hp / player.maxHp;
        if ((int)player.staminaValue > lastStaminaInt)
            SoundManager.instance.PlaySound("SFX_Skill_Gage");

        for (int i = 0; i < player.staminaMaxValue; i++)
        {
            staminaGauge[i].fillAmount = player.staminaValue - i;
        }
        lastStaminaInt = (int)player.staminaValue ;
    }
    private void WeaponIconAnim()
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
