using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CharacterWeapon : CharacterAbilities
{
    public static Action OnStartShooting;

    [Header("Weapon Settings")]
    [SerializeField] private WeaponBase weaponToUse;
    [SerializeField] private Transform weaponHolderPosition;

    public WeaponBase CurrentWeapon { get; set; }

    public WeaponBase SecondaryWeapon { get; set; }

    private bool secondaryEquipped = false;
    public bool SecondaryEquipped => secondaryEquipped;

    public WeaponAim WeaponAim { get; set; }

    public List<object> information = new List<object>();

    protected override void Start()
    {
        base.Start();
        if (weaponToUse)
        {
            weaponHolderPosition.localScale = new Vector3(transform.root.transform.localScale.x > 0 ? weaponHolderPosition.localScale.x : -weaponHolderPosition.localScale.x, weaponHolderPosition.localScale.y, weaponHolderPosition.localScale.z);
            EquipWeapon(weaponToUse, weaponHolderPosition);
        }
    }

    // left mouse is shoot, R is reload, more to come
    protected override void HandleInput()
    {
        if (UIManager.GameIsPaused)
        {
            return;
        }

        if (character.CharacterTypes == Character.CharacterTypeEnum.Player)
        {
            // 0 is left mouse button - GetMouseButtonDown makes a non-auto weapon vs GetMouseButton which is auto
            if (Input.GetMouseButton(0))
            {
                Attack();
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopAttack();
            }

            if (InputManager.instance.GetKeyDown(KeybindingActions.Reload))
            {
                Reload();
            }
            // Aplha1 is 1 on num pad - && stops equiping if there's no secondary weapon
            if (secondaryEquipped && Input.GetKeyDown(KeyCode.Alpha1) && SecondaryWeapon != null)
            {
                secondaryEquipped = false;
                EquipWeapon(weaponToUse, weaponHolderPosition);
            }

            if (!secondaryEquipped && Input.GetKeyDown(KeyCode.Alpha2) && SecondaryWeapon != null)
            {
                EquipWeapon(SecondaryWeapon, weaponHolderPosition);
                secondaryEquipped = true;
            }
        }
    }

    public void Attack()
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        CurrentWeapon.Attack();
    }

    public void StopAttack()
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        CurrentWeapon.StopAttack();
    }

    public void Reload()
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        CurrentWeapon.Reload();
    }

    public void EquipWeapon(WeaponBase weapon)
    {
        // use weapon holder position by default (it's private)
        EquipWeapon(weapon, weaponHolderPosition);
    }

    public void Disable()
    {
        CurrentWeapon.enabled = false;
        WeaponAim.enabled = false;
    }

    public void Enable()
    {
        CurrentWeapon.enabled = true;
        WeaponAim.enabled = true;
    }

    public void EquipWeapon(WeaponBase weapon, Transform weaponPosition)
    {
        List<object> temp = null;
        if (CurrentWeapon != null)
        {
            // destroys the current reticle, projectile pool, and weapon after saving the weapon ammo
            CurrentWeapon.HolsterWeapon();
            CurrentWeapon.WeaponAim.DestroyReticle(); // could do just WeaponAim.DestoryReticle() without the CurrentWeapon maybe
            temp = CurrentWeapon.GetWeaponInformation();

            // destroy in 3 seconds
            CurrentWeapon.gameObject.SetActive(false);
            Destroy(CurrentWeapon.gameObject, 3f);
        }
        // creates reference to weapon to be used by player
        CurrentWeapon = Instantiate(weapon, weaponPosition.position, weaponPosition.rotation);
        CurrentWeapon.transform.parent = weaponPosition;
        CurrentWeapon.SetOwner(character);
        WeaponAim = CurrentWeapon.GetComponent<WeaponAim>();

        CurrentWeapon.SetWeaponInformation(information);
        information = temp;
        CurrentWeapon.EquipWeapon();
    }
}