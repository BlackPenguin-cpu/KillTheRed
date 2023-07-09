using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using TMPro;

public enum EItemList
{
    Dash,
    Sword,
    Shotgun,
    Gun,
    Spear,
    Hammer
}
public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    [System.Serializable]
    public class ItemInfo
    {
        public Sprite icon;
        public string itemName;
        public string info;
        public string keyName;
    }
    public List<ItemInfo> itemInfos;

    public GameObject mainBoard;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI info;
    public TextMeshProUGUI keyName;

    public void ItemGet(EItemList value)
    {
        ItemInfo nowItemInfos = itemInfos[(int)value];

        icon.sprite = nowItemInfos.icon;
        itemName.text = nowItemInfos.itemName;
        info.text = nowItemInfos.info;
        keyName.text = nowItemInfos.keyName;

        switch (value)
        {
            case EItemList.Dash:
                Player.instance.skillState[EPlayerSkillState.Dash] = true;
                break;
            case EItemList.Sword:
                Player.instance.weaponState[EPlayerWeaponState.Sword] = true;
                break;
            case EItemList.Shotgun:
                Player.instance.skillState[EPlayerSkillState.Shotgun] = true;
                break;
            case EItemList.Gun:
                Player.instance.weaponState[EPlayerWeaponState.Pistol] = true;
                break;
            case EItemList.Spear:
                Player.instance.skillState[EPlayerSkillState.Spear] = true;
                break;
            case EItemList.Hammer:
                Player.instance.weaponState[EPlayerWeaponState.Hammer] = true;
                break;
        }


        StartCoroutine(MainBoardTurnOn());
        IEnumerator MainBoardTurnOn()
        {
            mainBoard.gameObject.SetActive(true);
            yield return new WaitForSeconds(5);
            mainBoard.gameObject.SetActive(false);
        }
    }
}
