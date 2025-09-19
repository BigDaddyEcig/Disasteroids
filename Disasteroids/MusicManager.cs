using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Disasteroids
{
    public class MusicManager
    {
        private List<string> musicFiles;
        private Random random;
        private string currentMusicFolder;
        private bool isMusicEnabled;
        private int currentTrackIndex;
        private Process currentMusicProcess;
        private DateTime trackStartTime;
        private bool isTrackPlaying;
        private string lastError;
        
        public bool IsMusicEnabled => isMusicEnabled;
        public string CurrentMusicFolder => currentMusicFolder;
        public int TrackCount => musicFiles?.Count ?? 0;
        public string LastError => lastError;
        
        public MusicManager()
        {
            musicFiles = new List<string>();
            random = new Random();
            currentMusicFolder = "";
            isMusicEnabled = false;
            currentTrackIndex = -1;
        }
        
        public bool SetMusicFolder(string folderPath)
        {
            lastError = "";
            
            try
            {
                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    lastError = "Empty folder path";
                    return false;
                }
                
                if (!Directory.Exists(folderPath))
                {
                    lastError = $"Folder does not exist: {folderPath}";
                    return false;
                }
                
                // Get all supported audio files in the folder
                var audioFiles = new List<string>();
                
                // Support multiple audio formats
                string[] extensions = { "*.mp3", "*.wav", "*.wma", "*.m4a" };
                foreach (string extension in extensions)
                {
                    var files = Directory.GetFiles(folderPath, extension, SearchOption.TopDirectoryOnly);
                    audioFiles.AddRange(files);
                }
                
                if (audioFiles.Count == 0)
                {
                    lastError = $"No audio files found in: {folderPath}";
                    return false;
                }
                
                musicFiles = audioFiles;
                currentMusicFolder = folderPath;
                isMusicEnabled = true;
                currentTrackIndex = -1;
                lastError = $"Success: Found {audioFiles.Count} audio files";
                
                return true;
            }
            catch (Exception ex)
            {
                lastError = $"Error: {ex.Message}";
                return false;
            }
        }
        
        public void DisableMusic()
        {
            try
            {
                // Stop any currently playing music process
                if (currentMusicProcess != null && !currentMusicProcess.HasExited)
                {
                    currentMusicProcess.Kill();
                    currentMusicProcess.Dispose();
                }
                currentMusicProcess = null;
            }
            catch (Exception)
            {
                // Ignore errors when stopping music
            }
            
            isMusicEnabled = false;
            musicFiles.Clear();
            currentMusicFolder = "";
            currentTrackIndex = -1;
            isTrackPlaying = false;
        }
        
        public void PlayRandomTrack()
        {
            if (!isMusicEnabled || musicFiles.Count == 0)
                return;
            
            try
            {
                // Stop any currently playing track
                if (currentMusicProcess != null && !currentMusicProcess.HasExited)
                {
                    currentMusicProcess.Kill();
                    currentMusicProcess.Dispose();
                }
                
                // Select a random track different from the current one
                int newIndex;
                if (musicFiles.Count == 1)
                {
                    newIndex = 0;
                }
                else
                {
                    do
                    {
                        newIndex = random.Next(musicFiles.Count);
                    } while (newIndex == currentTrackIndex && musicFiles.Count > 1);
                }
                
                currentTrackIndex = newIndex;
                string trackPath = musicFiles[currentTrackIndex];
                
                // Use the default system audio player
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = trackPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                
                currentMusicProcess = Process.Start(startInfo);
                trackStartTime = DateTime.Now;
                isTrackPlaying = true;
                
            }
            catch (Exception)
            {
                // If playback fails, disable music
                DisableMusic();
            }
        }
        
        public void Update()
        {
            if (!isMusicEnabled || !isTrackPlaying)
                return;
            
            try
            {
                // Check if current track has finished and play next random track
                if (currentMusicProcess == null || currentMusicProcess.HasExited)
                {
                    isTrackPlaying = false;
                    // Wait a moment before starting next track
                    if ((DateTime.Now - trackStartTime).TotalSeconds > 1)
                    {
                        PlayRandomTrack();
                    }
                }
            }
            catch (Exception)
            {
                // If there's an error, disable music
                DisableMusic();
            }
        }
        
        public string GetCurrentTrackName()
        {
            if (!isMusicEnabled || currentTrackIndex < 0 || currentTrackIndex >= musicFiles.Count)
                return "No music";
            
            return Path.GetFileNameWithoutExtension(musicFiles[currentTrackIndex]);
        }
        
        public void SetVolume(float volume)
        {
            try
            {
                MediaPlayer.Volume = Math.Max(0f, Math.Min(1f, volume));
            }
            catch (Exception)
            {
                // Ignore volume setting errors
            }
        }
    }
}
