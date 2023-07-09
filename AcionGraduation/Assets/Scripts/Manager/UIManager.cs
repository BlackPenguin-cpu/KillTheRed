using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Image hpBar;
    public Image hitEffectImage;
    public Image[] skillIcon;
    public Image[] weaponIcon;
    public Image[] staminaGauge;

    public int lastStaminaInt;
    #region 무기아이콘을 위한 변수
    readonly Color semiInvisible = new Color(1, 1, 1, 0.4f);
    readonly Vector3 weaponIconStartPos = new Vector2(0, -17.5f);
    readonly Vector3 weaponIconEndPos = new Vector2(0, -22.5f);
    #endregion

    #region 게임 오버 관련
    [Header("게임 오버 관련")]
    public Image gameOverObject;
    public Image gameOverImage;
    public TextMeshProUGUI gameOverTextObj;
    public string gameOverText;
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
            SoundManager.instance.PlaySound("SFX_Skill_Gage", SoundType.SE, 0.5f);

        for (int i = 0; i < player.staminaMaxValue; i++)
        {
            staminaGauge[i].fillAmount = player.staminaValue - i;
        }
        lastStaminaInt = (int)player.staminaValue;
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

    public void HitEffect(float duration)
    {
        float i = 0;
        Color color = new Color(1, 1, 1, 1);

        StartCoroutine(fadeIn());
        IEnumerator fadeIn()
        {
            hitEffectImage.color = Color.white;
            while (i <= 1)
            {
                hitEffectImage.color = color;
                color.a = 1 - Mathf.Lerp(0, 1, i);
                i += Time.deltaTime / (1 - duration);
                yield return null;
            }
            color.a = 0;
            hitEffectImage.color = color;
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }
    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSecondsRealtime(4);
        gameOverObject.gameObject.SetActive(true);
        yield return ImageFadeOut(gameOverImage);

        SoundManager.instance.PlaySound("X2Download.app-Undertale_Game_Over_Theme_mp3cut.net", SoundType.BGM);
        int i = 0;

        while (i < gameOverText.Length)
        {
            gameOverTextObj.text += gameOverText[i];
            i++;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        yield return new WaitForSecondsRealtime(1);
        Player.instance.transform.position = Player.instance.revivePos;
        Player.instance.Hp = Player.instance.maxHp;
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1);


        gameOverObject.gameObject.SetActive(false);
        gameOverImage.color = new Color(1, 1, 1, 0);
        gameOverTextObj.text = null;
        foreach (SmallEnemy enemy in FindObjectsOfType<SmallEnemy>())
        {
            Destroy(enemy.gameObject);
        }
        foreach (Wall wall in FindObjectsOfType<Wall>())
        {
            wall.GetComponent<SpriteRenderer>().sprite = wall.sprites[4];
            wall.Hp = wall.maxHp;
        }
        CameraManager.instance.cameraState = ECameraState.InGame;
        SoundManager.instance.PlaySound("BGM_03_Ingame", SoundType.BGM);


    }
    IEnumerator ImageFadeOut(Image image)
    {
        float i = 0;
        image.color = new Color(1, 1, 1, 0);

        Color color = image.color;
        while (i <= 1)
        {
            image.color = color;
            color.a = Mathf.Lerp(0, 1, i);
            i += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        color.a = 1;
        image.color = color;
    }

}
