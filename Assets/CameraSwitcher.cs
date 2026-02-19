using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Kamera Bakış Açıları")]
    [Tooltip("Sahnede oluşturduğunuz boş (Empty) objeleri buraya sürükleyin")]
    public Transform[] viewpoints; 
    
    private int currentIndex = 0;

    private void Start()
    {
        // Oyun başladığında kamera otomatik olarak listedeki İLK noktaya gitsin
        if (viewpoints.Length > 0 && viewpoints[0] != null)
        {
            transform.position = viewpoints[0].position;
            transform.rotation = viewpoints[0].rotation;
        }
    }

    private void Update()
    {
        // Eğer viewpoint listesi boşsa hata vermemesi için kontrol ediyoruz
        if (viewpoints == null || viewpoints.Length == 0) return;

        // Space (Boşluk) tuşuna basıldığında
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Listede bir sonraki noktaya geç (Listenin sonuna gelirse başa döner)
            currentIndex = (currentIndex + 1) % viewpoints.Length;
            
            // Kameranın pozisyonunu ve açısını yeni noktaya eşitle
            transform.position = viewpoints[currentIndex].position;
            transform.rotation = viewpoints[currentIndex].rotation;
            
            Debug.Log($"[KAMERA] Görüş açısı değiştirildi. Aktif kamera noktası: {viewpoints[currentIndex].name}");
        }
    }
}