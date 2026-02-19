using UnityEngine;

public class CanvasToggler : MonoBehaviour
{
    [Header("UI Ayarları")]
    [Tooltip("Gizleyip göstermek istediğiniz Canvas objesini buraya sürükleyin.")]
    public GameObject myCanvas;

    private void Update()
    {
        // 'P' tuşuna basılıp basılmadığını dinliyoruz
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Eğer Inspector'dan Canvas'ı atamayı unutmadıysak işlem yap
            if (myCanvas != null)
            {
                // Canvas'ın aktiflik durumunu tam tersine çevir (Açıksa kapat, kapalıysa aç)
                myCanvas.SetActive(!myCanvas.activeSelf);
                
                Debug.Log($"[UI] Menü görünürlüğü değiştirildi. Yeni durum: {myCanvas.activeSelf}");
            }
            else
            {
                Debug.LogWarning("[UI] HATA: Canvas objesi script içine sürüklenmemiş!");
            }
        }
    }
}