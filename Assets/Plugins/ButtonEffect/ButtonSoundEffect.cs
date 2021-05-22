using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonEffect
{
    public enum EButtonSoundEffectType
    {
        EDefaultAudio,
    }

    [RequireComponent(typeof(AudioSource), typeof(Button)), DisallowMultipleComponent]
    public class ButtonSoundEffect : MonoBehaviour
    {
        [SerializeField] EButtonSoundEffectType _effectType = EButtonSoundEffectType.EDefaultAudio;
        private const float _volume = 0.65f;
        private AudioSource _audioSource = null;
        private static Dictionary<string, AudioClip> _audioClipDict = new Dictionary<string, AudioClip>();
        private Button _button = null;
        public void zzPlayAudio()
        {
            if (!_audioSource) _audioSource = gameObject.AddComponent<AudioSource>();
            if (!_audioClipDict.ContainsKey(_effectType.ToString()))
            {
                _audioClipDict[_effectType.ToString()] = Resources.Load<AudioClip>("ButtonEffect/" + _effectType.ToString());
            }

            if (_audioClipDict[_effectType.ToString()] != null)
            {
                _audioSource.clip = _audioClipDict[_effectType.ToString()];
                _audioSource.playOnAwake = false;
                _audioSource.volume = _volume;
                _audioSource.Play();
            }
        }

        private void Awake()
        {
            if (!_button) _button = GetComponent<Button>();
            _button.onClick.AddListener(() => this.zzPlayAudio());
        }
    }
}
