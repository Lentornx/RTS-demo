using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class healthSystem : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public GameObject healthBarPrefab;
    private Image healthBarFill;
    private GameObject healthBarInstance;
    public string faction = "Player"; // or "Enemy"

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarPrefab)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 0.3f, Quaternion.identity, transform);
            healthBarFill = healthBarInstance.transform.Find("Background/Fill").GetComponent<Image>();
        };
      
}
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");

        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;

        if (healthBarFill)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}
