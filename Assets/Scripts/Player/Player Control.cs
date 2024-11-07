using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } }
    [SerializeField] private float moveSpeed = 1f;
    public static PlayerControl Instance;
    private PlayerMovement playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private bool facingLeft = false;
    private bool isDashing = false;
    public float dashDistance = 5f;
    public float dashDuration = 0.1f;
    public GameObject afterImagePrefab;
    public float numberOfAfterImages;


    private void Awake() {
        Instance = this;
        playerControls = new PlayerMovement();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }
    private void OnEnable() {
        playerControls.Enable();
    }

    private void Start(){
        playerControls.Combat.Dash.performed += _ => Dash();
    }

    private void Update() {
        PlayerInput();
    }

    private void FixedUpdate() {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput() {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move() {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x) {
            mySpriteRender.flipX = true;
            facingLeft = true;
        } else {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }

    private void AdjustAfterImageFacingDirection(GameObject afterImage)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        bool flipX = mousePos.x < playerScreenPoint.x;

        // Flip the after-image GameObject if necessary
        SpriteRenderer afterImageRenderer = afterImage.GetComponent<SpriteRenderer>();
        afterImageRenderer.flipX = flipX;
    }
    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            Vector2 dashDirection = playerControls.Movement.Move.ReadValue<Vector2>().normalized;
            StartCoroutine(PerformDash(dashDirection));
        }
    }

    private IEnumerator PerformDash(Vector2 direction)
    {
        float startTime = Time.time;
        float endTime = startTime + dashDuration;
        float timeBetweenAfterImages = dashDuration / (numberOfAfterImages + 1); // +1 to add an after-image at the end of the dash

        // While the dash is ongoing
        while (Time.time < endTime)
        {
            // Update position of the player using dashDistance
            transform.position += (Vector3)direction * dashDistance;

            // Wait for the specified time before instantiating the next after-image
            yield return new WaitForSeconds(timeBetweenAfterImages);

            // Instantiate after-image GameObject dynamically at the player's position
            GameObject afterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
            AdjustAfterImageFacingDirection(afterImage);

            // Destroy the after-image GameObject after a short delay
            Destroy(afterImage, 0.5f);
        }

        isDashing = false;
    }
}