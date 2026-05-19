
using Hotbar.Base;
using Hotbar.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotbar.Manager
{
    public partial class DataManager : SingletonMonobase<DataManager>
    {
        #region Variables
        [Header("Security")]
        [SerializeField] private SecurityConfig securityConfig;
        private static string EncryptionKey => Instance.securityConfig.EncryptionKey;
        private static string EncryptionIV => Instance.securityConfig.EncryptionIV;
        private static string Path => Application.persistentDataPath + "/user_gamedata.json";

        [Header("Cashing")]
        private Hotbar.Data.Data data = new Hotbar.Data.Data();
        public static Hotbar.Data.Data Data => Instance.data;
        #endregion
        public static void Initialize(System.Action onEnd = null)
        {
            #region Save & Load & Initialize
            LoadData();

            if (Instance.data == null)
            {
                Instance.data = new Data.Data();
                Instance.data.Initialize();
                SaveData(true);
            }
            #endregion

            IsInitialize = true;

            if (onEnd != null)
            {
                onEnd();
            }
        }

        #region Save & Load
        public static void SaveData(bool isForce = true)
        {
            #region Exception
            if (Instance.data == null)
            {
                Debug.LogWarning("[DataManager] => Data is Null or Empty!!");

                Debug.Log("Create New Data!!");
                Instance.data = new Data.Data();
                Instance.data.Initialize();
                Debug.Log("Create New Data Finished!");
            }
            #endregion

            Data.timeLog = GetCurrentTimeStr();

            string json = JsonUtility.ToJson(Instance.data);
            string encrypted = Encrypt(json);

            void WriteSafe()
            {
                string tempPath = Path + ".tmp";
                string directoryPath = System.IO.Path.GetDirectoryName(Path);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(tempPath, encrypted);

                if (File.Exists(Path))
                {
                    File.Replace(tempPath, Path, null);
                }
                else
                {
                    File.Move(tempPath, Path);
                }
            }

            WriteSafe();
        }
        public async static void SaveDataAsync(System.Action onEnd,  bool isForce = true)
        {
            #region Exception
            if (Instance.data == null)
            {
                Debug.LogWarning("[DataManager] => Data is Null or Empty!!");
                Debug.Log("Create New Data!!");
                Instance.data = new Data.Data();
                Instance.data.Initialize();
                Debug.Log("Create New Data Finished!");
            }
            #endregion

            Data.timeLog = GetCurrentTimeStr();

            string json = JsonUtility.ToJson(Instance.data);
            string encrypted = Encrypt(json);

            void WriteSafe()
            {
                string tempPath = Path + ".tmp";
                string directoryPath = System.IO.Path.GetDirectoryName(Path);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(tempPath, encrypted);

                if (File.Exists(Path))
                {
                    File.Replace(tempPath, Path, null);
                }
                else
                {
                    File.Move(tempPath, Path);
                }
            }

            await Task.Run(() =>
            {
                try
                {
                    WriteSafe();
                    onEnd();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[SaveData] Save Failed!! => " + e);
                }
            });
        }
        public static void LoadData()
        {
            string tempPath = Path + ".tmp";
            if (File.Exists(tempPath))
            {
                if (!File.Exists(Path))
                {
                    File.Move(tempPath, Path);
                }
                else
                {
                    File.Delete(tempPath);
                }
            }

            if (File.Exists(Path))
            {
                string encrypted = File.ReadAllText(Path);
                string json = Decrypt(encrypted);
                Instance.data =  JsonUtility.FromJson<Hotbar.Data.Data>(json);
            }
            else
            {
                Instance.data = null;
            }
        }

        #endregion

        #region Private
        public static string GetCurrentTimeStr() => DateTime.Now.ToString("O");
        #endregion

        #region Encrypt & Decrypt
        private static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = Encoding.UTF8.GetBytes(EncryptionIV);

                var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }
        private static string Decrypt(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = Encoding.UTF8.GetBytes(EncryptionIV);

                var decryptor = aes.CreateDecryptor();
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
        }
        #endregion
    }
}
