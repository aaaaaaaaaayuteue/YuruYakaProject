using UnityEngine;

[CreateAssetMenu(fileName = "SecurityConfig", menuName = "Config/SecurityConfig")]
public class SecurityConfig : ScriptableObject
{
    [SerializeField] private string encryptionKey;
    [SerializeField] private string encryptionIV;

    public string EncryptionKey => encryptionKey;
    public string EncryptionIV => encryptionIV;
}