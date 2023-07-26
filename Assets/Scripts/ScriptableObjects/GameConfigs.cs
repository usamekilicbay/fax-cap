using UnityEngine;

namespace FaxCap.Configs
{
    [CreateAssetMenu(fileName = "GameConfigs", menuName = "Configs/Game Configs")]
    public class GameConfigs : ScriptableObject
    {
        [Tooltip("The default time period (in seconds) given to answer a question.")]
        [SerializeField] private float defaultAnswerPeriod;

        [Tooltip("The milestone value at which the player's progress is updated.")]
        [SerializeField] private int progressMilestone;

        // Getter property for the default answer period.
        public float DefaultAnswerPeriod => defaultAnswerPeriod;

        // Getter property for the progress milestone.
        public int ProgressMilestone => progressMilestone;
    }
}
