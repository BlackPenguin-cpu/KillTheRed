using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System;
using Sirenix.OdinInspector.Editor.Validation;
using JetBrains.Annotations;
using System.Reflection;

public enum EPlayerWeaponState
{
    Hand,
    Sword,
    Pistol,
    Hammer,
    End
}
public enum EPlayerState
{
    Idle,
    Run,
    Jump,
    Attack,
    Dead,
    Dash,
    Skill
}
public enum EPlayerAttackState
{
    None,
    Upper,
    OnAir
}
public enum EPlayerSkillState
{
    NONE,
    Shotgun,
    Spear,
    Dash
}
public partial class Player : Entity
{
    public static Player instance;
    private class WeaponAttackAreaClass : OdinSerializeAttribute
    {
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D[]> weaponGroundAttack;
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D[]> weaponOnAirAttackArea;
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D> weaponUpperCutArea;
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerSkillState, BoxCollider2D> skillAttackArea;
    }

    public Coroutine attackCoroutine;

    public bool onAir;
    public float upperForcePower;
    public float upperSelfForcePower;

    public float attackDamage;

    [Header("Serializeable Variable")]

    [SerializeField]
    private WeaponAttackAreaClass weaponAttackAreaClass;
    [SerializeField]
    private GameObject gunLaser;
    [SerializeField]
    private GameObject gunSpark;
    [SerializeField]
    private GameObject gunAirspin;
    [SerializeField]
    private GameObject gunGranade;
    [SerializeField]
    private Transform gunGranadePos;

    public EPlayerState state;
    public EPlayerAttackState attackState;
    public EPlayerWeaponState playerWeaponState;
    public EPlayerSkillState playerSkillState;

    private int lookDir;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;
    private bool onAttack;

    private bool hammerCharging;
    private bool hammerChargingComplete;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    readonly public float staminaMaxValue = 3;
    readonly public float staminaaRegenSec = 3;
    public float staminaValue = 0;

    private float dashCooldown = 1;

    [DictionaryDrawerSettings]
    public Dictionary<EPlayerWeaponState, bool> weaponState;
    public Dictionary<EPlayerSkillState, bool> skillState;

    private void Awake()
    {
        instance = this;
    }
    protected override void Start()
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        state = EPlayerState.Idle;
    }
    private void Update()
    {
        Move();
        PlayerInput();
        AnimatorApply();

        if (state != EPlayerState.Skill)
            playerSkillState = EPlayerSkillState.NONE;

        if (staminaValue <= staminaMaxValue)
            staminaValue += Time.deltaTime * staminaaRegenSec;
        dashCooldown -= Time.deltaTime;
    }
    private void AnimatorApply()
    {
        onAir = isOnAir();
        animator.SetInteger("State", (int)state);
        animator.SetInteger("AttackState", (int)attackState);
        animator.SetInteger("WeaponState", (int)playerWeaponState);
        animator.SetInteger("SkillState", (int)playerSkillState);
        animator.SetInteger("HammerCharging", hammerCharging ? (hammerChargingComplete ? 2 : 1) : 0);
        animator.SetBool("OnAttack", onAttack);
        animator.SetBool("OnAir", onAir);
    }
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Dash();
        if (Input.GetKeyDown(KeyCode.C))
            Jump();
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerWeaponState == EPlayerWeaponState.Hammer)
            {
                if (state == EPlayerState.Idle && !onAir)
                    hammerCharging = true;
                else if (!onAir)
                    return;
            }


            if (Input.GetKey(KeyCode.UpArrow) && state != EPlayerState.Attack)
                UpperCut();
            if (onAir)
                attackState = EPlayerAttackState.OnAir;

            state = EPlayerState.Attack;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackDelay());
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            if (hammerChargingComplete)
                HammerKeyUp();
            else
                hammerCharging = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            ShotGun();
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Spear();
        if (Input.GetKeyDown(KeyCode.A))
            WeaponChanage(EPlayerWeaponState.Hand);
        else if (Input.GetKeyDown(KeyCode.S))
            WeaponChanage(EPlayerWeaponState.Sword);
        else if (Input.GetKeyDown(KeyCode.D))
            WeaponChanage(EPlayerWeaponState.Pistol);
        else if (Input.GetKeyDown(KeyCode.F))
            WeaponChanage(EPlayerWeaponState.Hammer);

    }
    private void Jump()
    {
        if (onAir) return;
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        state = EPlayerState.Jump;
        onAir = true;
    }
    private void Move()
    {
        if (onAttack || state == EPlayerState.Dash) return;
        var hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            lookDir = (int)hor;
            spriteRenderer.flipX = lookDir == 1 ? false : true;
            if (!onAir && rb.velocity.y == 0)
                state = EPlayerState.Run;
        }
        else if (state != EPlayerState.Attack && state != EPlayerState.Skill && rb.velocity == Vector2.zero)
        {
            state = EPlayerState.Idle;
        }
        var spdValue = hor * spd * Time.deltaTime * (hammerCharging ? 0.4f : 1);

        transform.position += Vector3.right * spdValue;
    }
    private void Dash()
    {
        if (state == EPlayerState.Attack || staminaValue < 1 || skillState[EPlayerSkillState.Dash] == false) return;
        if (dashCooldown < 0)
            dashCooldown = 1;
        else
            return;
        float duration = 0.1f;
        staminaValue -= 1;


        Vector3 pos = transform.position;
        StartCoroutine(cor());
        IEnumerator cor()
        {
            state = EPlayerState.Dash;
            while (duration > 0)
            {
                ShadowInst(0.1f, 0.5f);

                pos += Vector3.right * (lookDir * Time.deltaTime * 30);
                transform.position = pos;
                duration -= Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector3.zero;
            state = EPlayerState.Idle;
        }
    }
    private void HammerKeyUp()
    {
        state = EPlayerState.Attack;
    }
    private void ShadowInst(float duration, float startAlpha = 1)
    {
        GameObject obj = new GameObject("Player_Shadow", typeof(SpriteRenderer));
        obj.transform.position = transform.position;

        SpriteRenderer objRenderer = obj.GetComponent<SpriteRenderer>();
        objRenderer.flipX = spriteRenderer.flipX;
        objRenderer.sprite = spriteRenderer.sprite;
        Color alphaValue = spriteRenderer.color - Color.black * (1 - startAlpha);

        StartCoroutine(corutine());
        IEnumerator corutine()
        {
            while (alphaValue.a > 0.01f)
            {
                objRenderer.color = alphaValue;
                alphaValue.a -= Time.deltaTime / duration * alphaValue.a;
                yield return null;
            }
            Destroy(obj.gameObject);
        }
    }
    private void WeaponChanage(EPlayerWeaponState value)
    {
        if (weaponState[value] == false) return;
        playerWeaponState = value;

        hammerCharging = false;
        hammerChargingComplete = false;
    }
    private void BigAttack(int index)
    {
        BoxCollider2D collider2D = null;

        switch (attackState)
        {
            case EPlayerAttackState.None:
                collider2D = weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index];
                break;
            case EPlayerAttackState.Upper:
                collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];
                break;
            case EPlayerAttackState.OnAir:
                collider2D = weaponAttackAreaClass.weaponOnAirAttackArea[playerWeaponState][index];
                if (playerWeaponState == EPlayerWeaponState.Hand && index == 3)
                {
                    break;
                }
                rb.velocity = Vector3.up;
                break;
        }
        var ray = AttackCollisionCheck(collider2D);
        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage * 2;
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(lookDir * 3, 2.5f), ForceMode2D.Impulse);
        }
        Camera.main.DOShakePosition(0.2f, 2);
        CameraManager.instance.Flash(0.2f);
    }
    private void BaseAttack(int index = 0)
    {
        BoxCollider2D collider2D = null;

        if (attackState != EPlayerAttackState.Upper)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector3(Input.GetAxisRaw("Horizontal") * 2f, 1f), ForceMode2D.Impulse);
        }

        switch (attackState)
        {
            case EPlayerAttackState.None:
                collider2D = weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index];
                break;
            case EPlayerAttackState.Upper:
                if (playerWeaponState == EPlayerWeaponState.Sword)
                {
                    rb.AddForce(Vector2.up * upperSelfForcePower, ForceMode2D.Impulse);
                }
                collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];
                break;
            case EPlayerAttackState.OnAir:
                collider2D = weaponAttackAreaClass.weaponOnAirAttackArea[playerWeaponState][index];
                if (playerWeaponState == EPlayerWeaponState.Hand && index == 3)
                {
                    return;
                }
                rb.velocity = Vector3.up * 5;
                break;
        }
        var ray = AttackCollisionCheck(collider2D);
        if (ray.Length > 0)
        {
            StartCoroutine(timeDelay());
            IEnumerator timeDelay()
            {
                Time.timeScale = 0.1f;
                yield return new WaitForSecondsRealtime(0.1f);
                Time.timeScale = 1;
            }
        }

        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;

            switch (attackState)
            {
                case EPlayerAttackState.None:
                    physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(lookDir * 2, 1f);
                    if (playerWeaponState == EPlayerWeaponState.Pistol)
                    {
                        Instantiate(gunLaser, UnityEngine.Random.insideUnitCircle / 2 + (Vector2)physics2D.transform.position + physics2D.offset, Quaternion.Euler(0, 0, lookDir == -1 ? 180 : 0));
                        return;
                    }
                    break;
                case EPlayerAttackState.Upper:
                    physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * (upperForcePower / 2), ForceMode2D.Impulse);
                    break;
                case EPlayerAttackState.OnAir:
                    physics2D.transform.GetComponent<Rigidbody2D>().velocity = Vector3.up * 6;
                    if (playerWeaponState == EPlayerWeaponState.Pistol)
                    {
                        Instantiate(gunLaser, UnityEngine.Random.insideUnitCircle / 2 + (Vector2)physics2D.transform.position + physics2D.offset, Quaternion.Euler(0, 0, lookDir == -1 ? 180 : 0));
                        return;
                    }
                    break;
            }
        }
    }
    public void AttackEnd()
    {
        onAttack = false;
        state = EPlayerState.Idle;
        attackState = EPlayerAttackState.None;
    }
    public void AttackStart()
    {
        onAttack = true;
    }
    private IEnumerator AttackDelay()
    {
        yield return null;
        float duration = animator.runtimeAnimatorController.animationClips[0].length;
        onAttack = true;
        yield return new WaitForSeconds(duration);
        AttackEnd();
    }
    private void UpperCut()
    {
        if (onAir || playerWeaponState == EPlayerWeaponState.Hammer) return;
        attackState = EPlayerAttackState.Upper;
        if (playerWeaponState == EPlayerWeaponState.Pistol || playerWeaponState == EPlayerWeaponState.Sword) return;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector2.up * upperSelfForcePower, ForceMode2D.Impulse);
    }
    private Collider2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy") |  1 << LayerMask.NameToLayer("Wall");
        return Physics2D.OverlapBoxAll(transform.position + new Vector3(collider2D.offset.x * lookDir, collider2D.offset.y), collider2D.size, 0, layerMask);
    }

    public void HammerChargeComplete() => hammerChargingComplete = true;

    #region PlayerSkillEffect
    #region Hand
    private void HandAirAttackAxeKick()
    {
        var objs = AttackCollisionCheck(weaponAttackAreaClass.weaponOnAirAttackArea[EPlayerWeaponState.Hand][2]);
        foreach (Collider2D obj in objs)
        {
            obj.GetComponent<Rigidbody2D>().AddForce(Vector3.down * 20, ForceMode2D.Impulse);
        }
    }
    private void HandAirAttackFinish()
    {
        int layerMask = LayerMask.NameToLayer("Platform");
        RaycastHit2D obj = Physics2D.Raycast(transform.position, Vector2.down, 15, 1 << layerMask);

        transform.position = new Vector2(obj.point.x, obj.point.y);
    }
    #endregion
    #region Sword
    private void SwordDownAttack()
    {
        var objs = AttackCollisionCheck(weaponAttackAreaClass.weaponOnAirAttackArea[EPlayerWeaponState.Sword][3]);
        foreach (Collider2D obj in objs)
        {
            obj.GetComponent<Entity>().Hp -= attackDamage;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(10 * lookDir, -15), ForceMode2D.Impulse);
        }

        Camera.main.DOShakePosition(0.2f, 2);
        CameraManager.instance.Flash(0.1f);
    }
    #endregion
    #region Hammer
    private void HammerAttack()
    {
        hammerCharging = false;
        hammerChargingComplete = false;

        BoxCollider2D collider2D = null;
        collider2D = weaponAttackAreaClass.weaponGroundAttack[EPlayerWeaponState.Hammer][0];
        var ray = AttackCollisionCheck(collider2D);

        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage * 4;
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(lookDir * 4, 15), ForceMode2D.Impulse);
            if (ray != null)
            {
                StartCoroutine(timeDelay());
                IEnumerator timeDelay()
                {
                    Time.timeScale = 0.1f;
                    yield return new WaitForSecondsRealtime(0.2f);
                    Time.timeScale = 1;
                }
            }
        }
        Camera.main.DOShakePosition(0.2f, 2);
        CameraManager.instance.Flash(0.2f);
    }
    private void HammerDownAttack()
    {
        var objs = AttackCollisionCheck(weaponAttackAreaClass.weaponOnAirAttackArea[EPlayerWeaponState.Hammer][1]);
        foreach (Collider2D obj in objs)
        {
            obj.GetComponent<Entity>().Hp -= attackDamage;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(14 * lookDir, -10), ForceMode2D.Impulse);
        }
        rb.velocity = Vector3.up * 5;

        Camera.main.DOShakePosition(0.2f, 2);
        CameraManager.instance.Flash(0.2f);
    }
    #endregion
    #region Gun
    private void GunLastShot()
    {
        Vector2 pos = gunSpark.transform.localPosition;
        gunSpark.transform.localPosition = new Vector2(MathF.Abs(pos.x) * lookDir, pos.y);

        gunSpark.SetActive(true);
    }
    private void GunAirRoll(float index)
    {
        Vector2 pos = transform.position;
        float duration = 0;
        float dir = lookDir;

        gunAirspin.SetActive(true);

        StartCoroutine(Rolling());

        IEnumerator Rolling()
        {
            while (duration < 0.5f)
            {
                if (!onAttack)
                {
                    gunAirspin.SetActive(false);
                    yield break;
                }
                yield return null;
                transform.position = new Vector3(pos.x + dir * index, pos.y);
                index += (index > 0 ? 1 : -1) * Time.deltaTime * 5;
                duration += Time.deltaTime;
            }
            gunAirspin.SetActive(false);
        }
    }
    private void GunGranadeLanch()
    {
        GameObject obj = Instantiate(gunGranade, gunGranadePos.position, Quaternion.identity);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        rb.AddForce(Vector2.right * lookDir * 10, ForceMode2D.Impulse);
        rb.AddTorque(5);
    }
    #endregion
    #region Skill

    private void ShotGun()
    {
        if (staminaValue < 1 || playerSkillState == EPlayerSkillState.Shotgun || skillState[EPlayerSkillState.Shotgun] == false) return;
        staminaValue -= 1;

        ShadowInst(0.3f, 1);
        state = EPlayerState.Skill;
        playerSkillState = EPlayerSkillState.Shotgun;
        attackState = EPlayerAttackState.None;
        hammerCharging = false;
        hammerChargingComplete = false;
        onAttack = false;
    }
    private void ShotGunAttack()
    {
        BoxCollider2D collider2D = null;

        rb.AddForce(new Vector3(Input.GetAxisRaw("Horizontal") * 2f, 1f), ForceMode2D.Impulse);

        collider2D = weaponAttackAreaClass.skillAttackArea[EPlayerSkillState.Shotgun];
        var ray = AttackCollisionCheck(collider2D);
        if (ray.Length > 0)
        {
            StartCoroutine(timeDelay());
            IEnumerator timeDelay()
            {
                Time.timeScale = 0.1f;
                yield return new WaitForSecondsRealtime(0.1f);
                Time.timeScale = 1;
            }
        }

        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(lookDir * 2, 1f);
        }
    }
    private void ShotGunEnd()
    {
        state = EPlayerState.Idle;
        playerSkillState = EPlayerSkillState.NONE;
    }

    private void Spear()
    {
        if (staminaValue < 1 || !onAir || playerSkillState == EPlayerSkillState.Spear || skillState[EPlayerSkillState.Spear] == false) return;
        staminaValue -= 1;
        state = EPlayerState.Skill;
        playerSkillState = EPlayerSkillState.Spear;
        ShadowInst(0.3f, 1);
    }
    private void SpearAttack()
    {
        BoxCollider2D collider2D = null;

        int layerMask = LayerMask.NameToLayer("Platform");
        RaycastHit2D obj = Physics2D.Raycast(transform.position, Vector2.down, 15, 1 << layerMask);

        transform.position = new Vector2(obj.point.x, obj.point.y);

        collider2D = weaponAttackAreaClass.skillAttackArea[EPlayerSkillState.Spear];
        var ray = AttackCollisionCheck(collider2D);
        if (ray.Length > 0)
        {
            StartCoroutine(timeDelay());
            IEnumerator timeDelay()
            {
                Time.timeScale = 0.1f;
                yield return new WaitForSecondsRealtime(0.1f);
                Time.timeScale = 1;
            }
        }

        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5f);
        }
    }
    private void SpearEnd()
    {
        playerSkillState = EPlayerSkillState.NONE;
        state = EPlayerState.Idle;
    }
    #endregion
    #endregion
    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void Hit(float value)
    {
        Camera.main.DOShakePosition(0.3f, 2);
    }
}