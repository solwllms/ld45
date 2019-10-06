using OpenTK.Audio.OpenAL;
using System.Collections.Generic;

namespace ludumdare45
{
    public struct audioData
    {
        public bool loaded;
        public int channels;
        public int bits_per_sample;
        public int sample_rate;
        public ALFormat sound_format;
        public byte[] data;
    }

    public class cache
    {
        private static readonly Dictionary<string, texture> Texturecache = new Dictionary<string, texture>();
        private static readonly Dictionary<string, audioData> Soundcache = new Dictionary<string, audioData>();

        public static texture GetTexture(string name, bool cache = true)
        {
            if (Texturecache.ContainsKey(name) && cache) return Texturecache[name];

            var tex = new texture(name);
            if (cache) Texturecache.Add(name, tex);
            return tex;
        }
        public static void ClearAll()
        {
            Texturecache.Clear();
            ClearSounds();
        }

        
        public static audioData GetSound(string name, bool cache = true)
        {
            if (Soundcache.ContainsKey(name)) return Soundcache[name];

            try
            {
                int channels, bits_per_sample, sample_rate;
                byte[] sound_data = s_audio.LoadWave(filesystem.Open(name + ".wav"), out channels, out bits_per_sample, out sample_rate);

                audioData data = new audioData { loaded = true, channels = channels, bits_per_sample = bits_per_sample, sample_rate = sample_rate, data = sound_data, sound_format = s_audio.GetSoundFormat(channels, bits_per_sample) };
                if (cache) Soundcache.Add(name, data);
                return data;            
            }
            catch 
            {
                log.WriteLine("failed to load sound file: \"" + name + "\"", log.LogMessageType.Error);
                return new audioData { loaded = false };
            }
        }

        public static void ClearSounds()
        {
            Soundcache.Clear();
        }
    }
}