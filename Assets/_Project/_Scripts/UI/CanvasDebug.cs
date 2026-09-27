using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDebug : MonoBehaviour
{
    [SerializeField] private Button btnStartHost;
    [SerializeField] private Button btnStartClient;
    private void Awake()
    {
        btnStartHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });
        btnStartClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
    }
}
