using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoFactory.Managers
{
    public class SoundManager
    {
        private Dictionary<string, Song> _songs;
        private string _currentSongName;

        private Dictionary<string, SoundEffect> _soundEffects;

        public SoundManager()
        {
            _songs = new Dictionary<string, Song>();
            _soundEffects = new Dictionary<string, SoundEffect>();

            MediaPlayer.Volume = 0.3f;
            MediaPlayer.IsRepeating = true;
            SoundEffect.MasterVolume = 0.5f;
        }

        public void RegisterSong(string name, Song song)
        {
            if (!_songs.ContainsKey(name))
            {
                _songs.Add(name, song);
            }
        }

        public void PlayMusic(string name)
        {
            if (!_songs.ContainsKey(name))
            {
                return;
            }

            if (_currentSongName == name && MediaPlayer.State == MediaState.Playing)
            {
                return;
            }

            MediaPlayer.Volume = 0.4f;

            try
            {
                MediaPlayer.Play(_songs[name]);
                _currentSongName = name;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Crash: {ex.Message}");
            }
        }

        public void RegisterSoundEffect(string name, SoundEffect effect)
        {
            if (!_soundEffects.ContainsKey(name))
            {
                _soundEffects.Add(name, effect);
            }
        }

        public void PlaySound(string name)
        {
            if (_soundEffects.ContainsKey(name))
            {
                _soundEffects[name].Play();
            }
        }
    }
}
