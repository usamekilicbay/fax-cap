using System.Collections.Generic;
using UnityEngine;

namespace FaxCap.Manager
{
    public class AudioManager : MonoBehaviour
    {
        [field: SerializeField]
        [Tooltip("A list of audio clips used for scoring events.")]
        public List<AudioClip> ScoreSfxs { get; private set; }

        [field: SerializeField]
        [Tooltip("A list of audio clips used for card-related events.")]
        public List<AudioClip> CardSfxs { get; private set; }

        [field: SerializeField]
        [Tooltip("A list of audio clips used for UI sound effects.")]
        public List<AudioClip> UISfxs { get; private set; }

        [SerializeField]
        [Tooltip("The AudioSource component to play the audio clips.")]
        private AudioSource audioSource;

        private void Awake()
        {
            // Get the AudioSource component attached to this GameObject.
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays a single audio clip.
        /// </summary>
        /// <param name="clip">The audio clip to be played.</param>
        public void PlayAudio(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Plays a random audio clip from a list of audio clips.
        /// </summary>
        /// <param name="clips">The list of audio clips to choose from.</param>
        public void PlayAudio(IReadOnlyList<AudioClip> clips)
        {
            var randomIndex = Random.Range(0, clips.Count);
            var clip = clips[randomIndex];
            audioSource.PlayOneShot(clip);
        }
    }
}
