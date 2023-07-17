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

        private const string _folderPath = "Assets/Configs"; // Specify the complete folder path containing the ScriptableObjects

        [ContextMenu("Load Configs")]
        private void LoadConfigs()
        {
            var configurationManagerType = typeof(ConfigurationManager);

            // Get all non-public instance fields with [SerializeField] attribute
            var fields = configurationManagerType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .ToArray();

            foreach (var field in fields)
            {
                Debug.LogError(field.Name);

                var guids = AssetDatabase.FindAssets($"t:{field.FieldType.Name}", new string[] { _folderPath });

                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var config = AssetDatabase.LoadAssetAtPath(assetPath, field.FieldType);

                    field.SetValue(this, config);
                }
            }
        }
    }
}
