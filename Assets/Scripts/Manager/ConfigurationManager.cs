using FaxCap.Configs;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FaxCap.Manager
{
    public class ConfigurationManager : MonoBehaviour
    {
        [SerializeField] private GameConfigs gameConfigs;

        public GameConfigs GameConfigs => gameConfigs;

        private const string FolderPath = "Assets/Configs"; // Specify the complete folder path containing the ScriptableObjects

        [ContextMenu("Load Configs")]
        private void LoadConfigs()
        {
            Type configurationManagerType = typeof(ConfigurationManager);

            // Get all non-public instance fields with [SerializeField] attribute
            FieldInfo[] fields = configurationManagerType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .ToArray();

            foreach (FieldInfo field in fields)
            {
                Debug.LogError(field.Name);

                string[] guids = AssetDatabase.FindAssets($"t:{field.FieldType.Name}", new string[] { FolderPath });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var config = AssetDatabase.LoadAssetAtPath(assetPath, field.FieldType);

                    field.SetValue(this, config);
                }
            }
        }
    }
}
