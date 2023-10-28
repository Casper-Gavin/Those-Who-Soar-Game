using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float maxHealth = 10f;

    [Header("Shield")]
    [SerializeField] private float initialShield = 5f;
    [SerializeField] private float maxShield = 5f;
    

    [Header("Settings")]
    [SerializeField] private bool destroyObject;
    
    private Character character;
    private PlayerController controller;
    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;

    private bool shieldBroken;

    public float CurrentHealth { get; set; }
    public float CurrentShield { get; set; }

    // Awake is called before start
    private void Awake()
    {
        character = GetComponent<Character>();
        controller = GetComponent<PlayerController>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentHealth = initialHealth;
        CurrentShield = initialShield;

        UIManager.Instance.UpdateHealth(CurrentHealth, maxHealth, CurrentShield, maxShield);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        // only characters can have shield
        if (!shieldBroken && character != null)
        {
            CurrentShield -= damage;
            UIManager.Instance.UpdateHealth(CurrentHealth, maxHealth, CurrentShield, maxShield);
            if (CurrentShield <= 0)
            {
                shieldBroken = true;
            }
            return;
        }

        CurrentHealth -= damage;
        UIManager.Instance.UpdateHealth(CurrentHealth, maxHealth, CurrentShield, maxShield);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (character != null)
        {
            character.enabled = false;
            controller.enabled = false;
            collider2D.enabled = false;
            spriteRenderer.enabled = false;
        }

        if (destroyObject)
        {
            DestroyObject();
        }
    }

    public void Revive()
    {
        if (character != null)
        {
            character.enabled = true;
            controller.enabled = true;
            collider2D.enabled = true;
            spriteRenderer.enabled = true;
        }

        gameObject.SetActive(true);
        CurrentHealth = initialHealth;
        CurrentShield = initialShield;
        shieldBroken = false;
        UIManager.Instance.UpdateHealth(CurrentHealth, maxHealth, CurrentShield, maxShield);
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
