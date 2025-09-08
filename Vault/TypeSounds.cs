using System.Collections.Generic;
using UnityEngine;

namespace Vault {
    public class TypeSounds {
        private List<AudioClip> _audioClips;
        private AudioSource _audioSource;

        public TypeSounds(List<AudioClip> audioClips, AudioSource audioSource) {
            _audioClips = audioClips;
            _audioSource = audioSource;
        }

        public void PlaySound() {
            _audioSource.PlayOneShot(_audioClips.GetRandomItem(), Random.Range(0.6f, 1f));
        }
    }
}