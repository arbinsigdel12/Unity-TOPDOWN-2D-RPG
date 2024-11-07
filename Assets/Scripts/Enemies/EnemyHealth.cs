using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private FloatValue startingHealth;
    [SerializeField]GameObject deathVfx;
    private float currentHealth;
    private Knockback knockback;
    private Flash flash;

    void Awake(){
        flash=GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }
    private void Start() {
        currentHealth = startingHealth.initialValue;
    }
    public void TakeDamage(float damage)
    {
        currentHealth-=damage;
        knockback.GetKnockedBack(PlayerControl.Instance.transform,15f);
        StartCoroutine(flash.FlashRoutine());
        DetectDeath();
    }

    private void DetectDeath(){
        if(currentHealth <= 0){
            Instantiate(deathVfx,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
