using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ludumdare45
{
    class s_audio
    {
        static AudioContext context;
        public static int source;

        public static audioPlayer sfxPlayer;
        public static audioPlayer musicPlayer;
        public static audioPlayer ambientPlayer;

        public static void Init()
        {
            context = new AudioContext();
            context.MakeCurrent();

            var device = Alc.OpenDevice(null);

            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);
            log.WriteLine("starting audio.. (" + version + ", " + vendor + ", " + renderer + ")");

            sfxPlayer = new audioPlayer();
            musicPlayer = new audioPlayer();
            ambientPlayer = new audioPlayer();

            source = AL.GenSource();
        }

        public static void Shutdown()
        {
            log.WriteLine("shutting down audio!");

            AL.SourceStop(source);
            AL.DeleteSource(source);

            sfxPlayer.Shutdown();
            musicPlayer.Shutdown();
            ambientPlayer.Shutdown();

            context.Dispose();
        }

        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }

    class audioPlayer
    {
        int buffer;

        public audioPlayer()
        {
            buffer = AL.GenBuffer();
        }

        public void PlaySFX(string name)
        {
            Stop();
            AL.DeleteBuffer(buffer);
            buffer = AL.GenBuffer();

            audioData data = cache.GetSound(name);
            if (!data.loaded) {
                log.WriteLine("audio file not loaded ("+name+")");
                return;
            }
            AL.BufferData(buffer, data.sound_format, data.data, data.data.Length, data.sample_rate);

            AL.Source(s_audio.source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(s_audio.source);

            log.WriteLine("playing sfx (" + name + ")");
        }

        public void Stop()
        {
            AL.SourceStop(s_audio.source);
        }

        public ALSourceState GetState()
        {
            int state;
            AL.GetSource(s_audio.source, ALGetSourcei.SourceState, out state);
            return (ALSourceState) state;
        }

        public void Shutdown()
        {            
            AL.DeleteBuffer(buffer);
        }
    }
}
