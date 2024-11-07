using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField]private Transform weaponCollider;
    [SerializeField] private FloatValue attackcooldown;
    
    private PlayerMovement playerControls;
    private Animator myAnimator;
    private PlayerControl playerController;
    private ActiveWeapon activeWeapon;
    private GameObject slashAnim;
    private bool attackButtonDown ,isAttacking =false;

    void Awake(){
        playerController=GetComponentInParent<PlayerControl>();
        activeWeapon=GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerMovement();
    }
    void OnEnable(){
        playerControls.Enable();
    }

    void Start(){
        playerControls.Combat.Attack.started += _ => startAttacking();
        playerControls.Combat.Attack.canceled += _ => stopAttacking();
    }

    void Update(){
        MouseFollowWithOffset();
        Attack();
    }

    private void startAttacking(){
        attackButtonDown=true;        
    }

    private void stopAttacking(){
        attackButtonDown=false;
    }

    void Attack(){
        if(!isAttacking && attackButtonDown){
            isAttacking=true;
            myAnimator.SetTrigger("Attack");
            weaponCollider.gameObject.SetActive(true);
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
            StartCoroutine(AttackCDCoroutine());
        }
    }

    private IEnumerator AttackCDCoroutine(){
        yield return new WaitForSeconds(attackcooldown.initialValue);
        isAttacking=false;
    }

    public void DoneAttackingEvent(){
        weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (playerController.FacingLeft) { 
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void MouseFollowWithOffset() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x) {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        } else {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
