using System;
using System.Collections;
using UnityEngine;

public class SharedDamageable : MonoBehaviour
{
    public float health = 100f;
    public GameObject SpawnPoint;
    private bool isDead = false;

    public int totalLives = 2;

    private void Update()
    {
        if (totalLives <= 0)
        {
            UIManager.Instance.ShowGameOverMenu();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        Debug.Log($"Shared health is now {health}");

        if (health <= 0)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        totalLives = totalLives - 1;
        Transform player1 = transform.Find("PlayerForm1");
        Transform player2 = transform.Find("PlayerForm2");

        Rigidbody2D[] rbs = {
            player1.GetComponent<Rigidbody2D>(),
            player2.GetComponent<Rigidbody2D>()
        };

        BoxCollider2D[] colliders = {
            player1.GetComponent<BoxCollider2D>(),
            player2.GetComponent<BoxCollider2D>()
        };

        SpriteRenderer[] renderers = {
            player1.GetComponent<SpriteRenderer>(),
            player2.GetComponent<SpriteRenderer>()
        };

        // غیرفعالسازی کامل فرم‌ها
        for (int i = 0; i < 2; i++)
        {
            if (rbs[i] != null) rbs[i].bodyType = RigidbodyType2D.Static;
            if (colliders[i] != null) colliders[i].enabled = false;
            if (renderers[i] != null) renderers[i].enabled = false;
        }

        yield return new WaitForEndOfFrame();
        
        if (IsGameOver())
        {
            UIManager.Instance.ShowGameOverMenu();
            yield break;
        }

        yield return new WaitForSeconds(10f);

        Transform spawnChild = null;
        foreach (Transform child in SpawnPoint.transform)
        {
            if (child.gameObject.activeSelf)
            {
                spawnChild = child;
                break;
            }
        }

        if (spawnChild == null)
        {
            UIManager.Instance.ShowGameOverMenu(); // جایگزین fallback قبلی
            yield break;
        }

        Vector3 spawnPosition = spawnChild.position;

        if (player1 != null) player1.position = new Vector2(spawnPosition.x , spawnPosition.y+2 );
        if (player2 != null) player2.position = new Vector2(spawnPosition.x , spawnPosition.y+2 );

        for (int i = 0; i < 2; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = true;
            if (colliders[i] != null) colliders[i].enabled = true;
            if (rbs[i] != null) rbs[i].bodyType = RigidbodyType2D.Dynamic;
        }

        health = 100f;
        isDead = false;
    }
    
    public void Heal(float amount)
    {
        if (health <= 0) return; // اگر مرده، هیچی
        health += amount;
        if (health > 100f) health = 100f; // محدود کردن به maxHealth
        Debug.Log($"✅ Healed! New health: {health}");
    }

    
    private bool IsGameOver()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in allPlayers)
        {
            if (obj.activeInHierarchy)
            {
                var col = obj.GetComponent<Collider2D>();
                var rend = obj.GetComponent<SpriteRenderer>();
                if (col != null && col.enabled && rend != null && rend.enabled)
                    return false; // حداقل یک پلیر هنوز فعاله
            }
        }
        return true; // هیچ‌کس فعال نیست => Game Over
    }
}
