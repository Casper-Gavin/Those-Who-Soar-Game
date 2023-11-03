using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CharacterWeapon : CharacterAbilities
{
    [Header("Weapon Settings")]
    [SerializeField] private Weapon weaponToUse;
    [SerializeField] private Transform weaponHolderPosition;

    public Weapon CurrentWeapon { get; set;}

    protected override void Start()
    {
        base.Start();
        EquipWeapon(weaponToUse, weaponHolderPosition);
    }

    // left mouse is shoot, R is reload
    protected override void HandleInput()
    {
        // 0 is left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Shoot()
    {
        
    }

    public void Reload()
    {

    }

    public void EquipWeapon(Weapon weapon, Transform weaponPosition)
    {
        // creates weapon to be used by player
        CurrentWeapon = Instantiate(weapon, weaponPosition.position, weaponPosition.rotation);
        CurrentWeapon.transform.parent = weaponPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
