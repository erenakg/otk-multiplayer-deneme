using UnityEngine;
using Unity.Netcode;

// Bu satır, scripti eklediğinde objede Rigidbody yoksa otomatik olarak eklemesini sağlar.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 inputVector;

    private void Start()
    {
        // Başlangıçta Rigidbody component'ini alıyoruz
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // KONTROL: Eğer bu karakter bizim değilse, klavye girdilerini DİNLEME!
        if (!IsOwner) return;

        // WASD veya Ok tuşlarından girdileri alıyoruz
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Çapraz gidişlerde hızın artmasını engellemek için normalize ediyoruz
        inputVector = new Vector2(moveX, moveZ).normalized;
    }

    private void FixedUpdate()
    {
        // KONTROL: Fizik motoru hareketi de sadece objenin sahibinde çalışmalı
        if (!IsOwner) return;

        // Girdiyi 3D uzaya (X ve Z eksenine) uyarlıyoruz. Y ekseni (yukarı/aşağı) sabit kalıyor.
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        // Rigidbody'nin hızını (velocity) değiştirerek karakteri hareket ettiriyoruz.
        // Y eksenindeki mevcut hızı (rb.velocity.y) koruyoruz ki yerçekimi düzgün çalışsın.
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
}