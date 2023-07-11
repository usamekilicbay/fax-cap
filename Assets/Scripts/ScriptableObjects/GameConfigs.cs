using UnityEngine;

namespace FaxCap.Configs
{
    [CreateAssetMenu(fileName = "GameConfigs", menuName = "Configs/Game Configs")]
    public class GameConfigs : ScriptableObject
    {
        [SerializeField] private int progressMilestone;

        public int ProgressMilestone => progressMilestone;
    }
}
