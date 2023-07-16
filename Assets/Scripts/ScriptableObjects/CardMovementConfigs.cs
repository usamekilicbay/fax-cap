using Assets.Scripts.Common.Types;
using System.Collections.Generic;
using UnityEngine;

namespace FaxCap.Configs
{
    [CreateAssetMenu(fileName = "CardMovementConfigs", menuName = "Configs/Card Movement Configs")]
    public class CardMovementConfigs : ScriptableObject
    {
        // This script defines properties and variables to control an object's behavior.
        // The object is used as a card with specific ranges and limitations.

        [SerializeField, Tooltip("The minimum horizontal range of the card's movement.")]
        private float minHorizontalRange;

        [SerializeField, Tooltip("The maximum horizontal range of the card's movement.")]
        private float maxHorizontalRange;

        [SerializeField, Tooltip("The minimum vertical range of the card's movement.")]
        private float minVerticalRange;

        [SerializeField, Tooltip("The maximum vertical range of the card's movement.")]
        private float maxVerticalRange;

        [SerializeField, Tooltip("The threshold radius for triggering the card's direction.")]
        private float thresholdRadius;

        [SerializeField, Tooltip("The maximum rotation angle allowed for the card.")]
        private float maxRotationAngle;

        [SerializeField, Tooltip("Allowed movement directions for the card.")]
        private MovementDirection allowedDirections;

        // Properties for the range variables

        // The minimum horizontal range property of the card's movement.
        public float MinHorizontalRange => minHorizontalRange;

        // The maximum horizontal range property of the card's movement.
        public float MaxHorizontalRange => maxHorizontalRange;

        // The minimum vertical range property of the card's movement.
        public float MinVerticalRange => minVerticalRange;

        // The maximum vertical range property of the card's movement.
        public float MaxVerticalRange => maxVerticalRange;

        // Property for the limitation variables

        // The threshold radius property for triggering the card's direction.
        public float ThresholdRadius => thresholdRadius;

        // The maximum rotation angle property allowed for the card.
        public float MaxRotationAngle => maxRotationAngle;

        // The allowed movement directions for the card.
        public MovementDirection AllowedDirections => allowedDirections;

        // Custom method to update the range fields based on the allowed directions
        public void UpdateRangeFields()
        {
            // Set the range fields based on allowed directions
            if ((allowedDirections & MovementDirection.Left) != 0)
                minHorizontalRange = -200f;
            else
                minHorizontalRange = 0f;

            if ((allowedDirections & MovementDirection.Right) != 0)
                maxHorizontalRange = 200f;
            else
                maxHorizontalRange = 0f;

            if ((allowedDirections & MovementDirection.Bottom) != 0)
                minVerticalRange = -200f;
            else
                minVerticalRange = 0f;

            if ((allowedDirections & MovementDirection.Top) != 0)
                maxVerticalRange = 200f;
            else
                maxVerticalRange = 0f;
        }

        // Called automatically by Unity whenever the script object is modified in the inspector
        private void OnValidate()
        {
            UpdateRangeFields();
        }
    }
}
