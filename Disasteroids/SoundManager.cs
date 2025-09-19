using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Disasteroids
{
    public class SoundManager
    {
        private Dictionary<string, SoundEffect> soundEffects;
        private Random random;
        
        public SoundManager()
        {
            soundEffects = new Dictionary<string, SoundEffect>();
            random = new Random();
        }
        
        public void LoadContent(ContentManager content)
        {
            // Since we don't have actual sound files, we'll create procedural sounds
            CreateProceduralSounds();
        }
        
        private void CreateProceduralSounds()
        {
            // Create simple procedural sound effects
            soundEffects["shoot"] = CreateShootSound();
            soundEffects["asteroidHit"] = CreateAsteroidHitSound();
            soundEffects["playerHit"] = CreatePlayerHitSound();
            soundEffects["powerUp"] = CreatePowerUpSound();
            soundEffects["thrust"] = CreateThrustSound();
        }
        
        private SoundEffect CreateShootSound()
        {
            // Create a more dynamic laser-like shooting sound
            int sampleRate = 22050;
            int duration = (int)(0.15 * sampleRate); // Slightly longer
            byte[] samples = new byte[duration * 2]; // 16-bit samples
            
            for (int i = 0; i < duration; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = (float)Math.Exp(-t * 15); // Quick but not too sharp decay
                
                // Multiple frequency components for richer sound
                float freq1 = (float)Math.Sin(2 * Math.PI * 1200 * t) * 0.3f; // High frequency
                float freq2 = (float)Math.Sin(2 * Math.PI * 800 * t) * 0.2f; // Mid frequency
                float freq3 = (float)Math.Sin(2 * Math.PI * 400 * t) * 0.1f; // Lower harmonic
                
                // Add slight frequency sweep for laser effect
                float sweepFreq = 1200 - (t * 300); // Frequency drops slightly
                float sweep = (float)Math.Sin(2 * Math.PI * sweepFreq * t) * 0.25f;
                
                // Subtle noise for texture
                float noise = (float)(random.NextDouble() * 2 - 1) * 0.1f;
                
                float sample = (freq1 + freq2 + freq3 + sweep + noise) * envelope;
                short sampleValue = (short)(sample * short.MaxValue);
                
                samples[i * 2] = (byte)(sampleValue & 0xFF);
                samples[i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
            }
            
            return new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        }
        
        private SoundEffect CreateAsteroidHitSound()
        {
            // Create a more dramatic explosion-like sound
            int sampleRate = 22050;
            int duration = (int)(0.4 * sampleRate); // Longer duration
            byte[] samples = new byte[duration * 2];
            
            for (int i = 0; i < duration; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = (float)Math.Exp(-t * 6); // Slower decay for more impact
                
                // Multiple explosion components
                float noise = (float)(random.NextDouble() * 2 - 1) * 0.5f; // More intense noise
                float lowRumble = (float)Math.Sin(2 * Math.PI * 80 * t) * 0.4f; // Deep rumble
                float midCrack = (float)Math.Sin(2 * Math.PI * 200 * t) * 0.3f; // Mid crack
                float highCrack = (float)Math.Sin(2 * Math.PI * 600 * t) * 0.2f; // High crack
                
                // Add frequency sweep for dramatic effect
                float sweepFreq = 300 - (t * 200); // Frequency drops over time
                float sweep = (float)Math.Sin(2 * Math.PI * sweepFreq * t) * 0.25f;
                
                // Combine all components
                float sample = (noise + lowRumble + midCrack + highCrack + sweep) * envelope;
                short sampleValue = (short)(sample * short.MaxValue);
                
                samples[i * 2] = (byte)(sampleValue & 0xFF);
                samples[i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
            }
            
            return new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        }
        
        private SoundEffect CreatePlayerHitSound()
        {
            // Create a dramatic hit sound
            int sampleRate = 22050;
            int duration = (int)(0.5 * sampleRate); // 0.5 seconds
            byte[] samples = new byte[duration * 2];
            
            for (int i = 0; i < duration; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = (float)Math.Exp(-t * 5); // Medium decay
                float noise = (float)(random.NextDouble() * 2 - 1) * 0.5f;
                float sweep = (float)Math.Sin(2 * Math.PI * (200 - t * 150) * t) * 0.4f; // Frequency sweep down
                
                float sample = (noise + sweep) * envelope;
                short sampleValue = (short)(sample * short.MaxValue);
                
                samples[i * 2] = (byte)(sampleValue & 0xFF);
                samples[i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
            }
            
            return new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        }
        
        private SoundEffect CreatePowerUpSound()
        {
            // Create a more satisfying power-up sound with harmonics
            int sampleRate = 22050;
            int duration = (int)(0.6 * sampleRate); // Longer duration
            byte[] samples = new byte[duration * 2];
            
            for (int i = 0; i < duration; i++)
            {
                float t = (float)i / sampleRate;
                
                // Multi-stage envelope for more dynamic sound
                float envelope;
                if (t < 0.1f) // Attack phase
                {
                    envelope = t * 10f; // Quick rise
                }
                else if (t < 0.4f) // Sustain phase
                {
                    envelope = 1.0f;
                }
                else // Decay phase
                {
                    envelope = 1.0f - ((t - 0.4f) / 0.2f);
                }
                envelope = Math.Max(0, envelope);
                
                // Multiple ascending frequencies for rich harmonic content
                float baseFreq = 300 + t * 600; // Main ascending tone
                float harmonic1 = (float)Math.Sin(2 * Math.PI * baseFreq * t) * 0.4f;
                float harmonic2 = (float)Math.Sin(2 * Math.PI * baseFreq * 1.5f * t) * 0.2f; // Perfect fifth
                float harmonic3 = (float)Math.Sin(2 * Math.PI * baseFreq * 2f * t) * 0.15f; // Octave
                
                // Add sparkle effect with higher frequencies
                float sparkle = (float)Math.Sin(2 * Math.PI * (1200 + t * 400) * t) * 0.1f;
                
                // Combine all components
                float sample = (harmonic1 + harmonic2 + harmonic3 + sparkle) * envelope;
                short sampleValue = (short)(sample * short.MaxValue);
                
                samples[i * 2] = (byte)(sampleValue & 0xFF);
                samples[i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
            }
            
            return new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        }
        
        private SoundEffect CreateThrustSound()
        {
            // Create a continuous thrust sound (will be looped)
            int sampleRate = 22050;
            int duration = (int)(0.2 * sampleRate); // 0.2 seconds for looping
            byte[] samples = new byte[duration * 2];
            
            for (int i = 0; i < duration; i++)
            {
                float t = (float)i / sampleRate;
                float noise = (float)(random.NextDouble() * 2 - 1) * 0.2f;
                float lowRumble = (float)Math.Sin(2 * Math.PI * 60 * t) * 0.3f;
                float midRumble = (float)Math.Sin(2 * Math.PI * 120 * t) * 0.2f;
                
                float sample = noise + lowRumble + midRumble;
                short sampleValue = (short)(sample * short.MaxValue);
                
                samples[i * 2] = (byte)(sampleValue & 0xFF);
                samples[i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
            }
            
            return new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        }
        
        public void PlaySound(string soundName, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                try
                {
                    soundEffects[soundName].Play(volume, pitch, pan);
                }
                catch (Exception)
                {
                    // Ignore audio errors - game should continue without sound
                }
            }
        }
        
        public SoundEffectInstance CreateLoopingSound(string soundName, float volume = 1.0f)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                try
                {
                    var instance = soundEffects[soundName].CreateInstance();
                    instance.IsLooped = true;
                    instance.Volume = volume;
                    return instance;
                }
                catch (Exception)
                {
                    // Return null if audio fails
                    return null;
                }
            }
            return null;
        }
    }
}
