using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro; 

public class RelayManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TMP_Text joinCodeText; 
    public TMP_InputField joinCodeInput;

    private string targetRegion = "europe-west4"; 

    private async void Start()
    {
        Debug.Log("[START] RelayManager başlatıldı. UGS ayarları yapılıyor...");
        InitializationOptions options = new InitializationOptions();
        options.SetEnvironmentName("production"); 
        
        string randomProfile = System.Guid.NewGuid().ToString().Substring(0, 8);
        options.SetProfile(randomProfile);
        Debug.Log($"[START] Bu pencere için atanan rastgele profil: {randomProfile}");

        Debug.Log("[START] Unity Services (UGS) başlatılıyor...");
        await UnityServices.InitializeAsync(options);
        Debug.Log("[START] Unity Services başarıyla başlatıldı!");

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("[AUTH] Sisteme anonim giriş isteği gönderiliyor...");
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"[AUTH] GİRİŞ BAŞARILI! Player ID: {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateRelayHost()
    {
        Debug.Log("--- [HOST] HOST OLMA SÜRECİ BAŞLADI ---");
        try
        {
            Debug.Log($"[HOST] 1. Adım: Relay'den {targetRegion} bölgesinde alan (Allocation) isteniyor...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3, targetRegion);
            Debug.Log($"[HOST] Alan tahsis edildi! Allocation ID: {allocation.AllocationId}");

            Debug.Log("[HOST] 2. Adım: Odaya giriş için Join Code oluşturuluyor...");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[HOST] *** ODA HAZIR! Bölge: {allocation.Region} | Join Code: {joinCode} ***");
            
            if(joinCodeText != null) joinCodeText.text = "Join Code: " + joinCode;

            Debug.Log("[HOST] 3. Adım: Transport verileri aktarılıyor. (Şifreli bağlantı/DTLS: KAPALI)");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                false // <--- İŞTE O HATAYI ÇÖZEN KISIM (False yaptık)
            );

            Debug.Log("[HOST] 4. Adım: NetworkManager üzerinden Host başlatılıyor...");
            bool isHostStarted = NetworkManager.Singleton.StartHost();
            
            if(isHostStarted)
                Debug.Log("[HOST] SÜREÇ TAMAMLANDI: StartHost() başarıyla çalıştı ve karakter sahneye eklendi.");
            else
                Debug.LogError("[HOST] HATA: StartHost() komutu başarısız oldu!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[HOST] KRİTİK HATA: " + e.Message);
        }
    }

    public async void JoinRelayClient()
    {
        Debug.Log("--- [CLIENT] CLIENT OLARAK KATILMA SÜRECİ BAŞLADI ---");
        try
        {
            string joinCode = joinCodeInput.text.Trim(); 
            Debug.Log($"[CLIENT] 1. Adım: Input kutusundan okunan Kod: [{joinCode}]");

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("[CLIENT] HATA: Şifre kutusu boş! Lütfen kodu girin.");
                return;
            }

            Debug.Log($"[CLIENT] 2. Adım: {joinCode} kodu ile Relay odası aranıyor...");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log($"[CLIENT] ODA BULUNDU! Odadaki Host'un Bölgesi: {joinAllocation.Region}");

            Debug.Log("[CLIENT] 3. Adım: Transport verileri aktarılıyor. (Şifreli bağlantı/DTLS: KAPALI)");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                false // <--- İŞTE O HATAYI ÇÖZEN KISIM (False yaptık)
            );

            Debug.Log("[CLIENT] 4. Adım: NetworkManager üzerinden Client başlatılıyor...");
            bool isClientStarted = NetworkManager.Singleton.StartClient();
            
            if(isClientStarted)
                Debug.Log("[CLIENT] SÜREÇ TAMAMLANDI: StartClient() başarıyla çalıştı, karakter sahneye ekleniyor!");
            else
                Debug.LogError("[CLIENT] HATA: StartClient() komutu başarısız oldu!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[CLIENT] KRİTİK HATA: " + e.Message);
        }
    }
}