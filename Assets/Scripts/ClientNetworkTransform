using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // Otoriteyi sunucudan alıp, objenin sahibi olan Client'a veriyoruz.
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}