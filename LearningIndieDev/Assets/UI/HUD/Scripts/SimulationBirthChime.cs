using UnityEngine;

namespace SaltyGame
{
    internal enum SimulationSound { Selection, Upgrade, Danger, Success, Failure }

    internal static class SimulationSoundPalette
    {
        const int SampleRate = 11025;
        static readonly AudioClip[] cues = new AudioClip[5];
        static readonly float[] lastPlayedAt = { -10f, -10f, -10f, -10f, -10f };
        static readonly float[] minimumSpacing = { 0.12f, 0.25f, 1.5f, 2f, 2f };
        static AudioClip ambience;
        static AudioClip music;

        public static void Play(GameObject owner, SimulationSound sound)
        {
            if (owner == null || !Application.isPlaying) return;
            var index = (int)sound;
            if (Time.unscaledTime - lastPlayedAt[index] < minimumSpacing[index]) return;
            var source = owner.GetComponent<AudioSource>();
            if (source == null)
            {
                source = owner.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.volume = 0.36f;
            }
            source.PlayOneShot(cues[index] ??= CreateCue(sound));
            lastPlayedAt[index] = Time.unscaledTime;
        }

        public static void StartBed(GameObject owner)
        {
            if (owner == null || !Application.isPlaying) return;
            if (owner.GetComponent<AudioSource>() == null)
            {
                var effects = owner.AddComponent<AudioSource>();
                effects.playOnAwake = false;
                effects.spatialBlend = 0f;
                effects.volume = 0.36f;
            }
            var bed = owner.transform.Find("Simulation Audio Bed");
            if (bed == null)
            {
                bed = new GameObject("Simulation Audio Bed").transform;
                bed.SetParent(owner.transform, false);
                StartLoop(bed.gameObject, ambience ??= CreateBed(false), 0.09f);
                StartLoop(bed.gameObject, music ??= Resources.Load<AudioClip>("Audio/CozyPond") ?? CreateBed(true), 0.32f);
            }
            foreach (var source in bed.GetComponents<AudioSource>())
                if (!source.isPlaying) source.Play();
        }

        public static void StopBed(GameObject owner)
        {
            var bed = owner == null ? null : owner.transform.Find("Simulation Audio Bed");
            if (bed == null) return;
            foreach (var source in bed.GetComponents<AudioSource>()) source.Stop();
        }

        static void StartLoop(GameObject owner, AudioClip clip, float volume)
        {
            var source = owner.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = true;
            source.volume = volume;
            source.clip = clip;
        }

        static AudioClip CreateCue(SimulationSound sound)
        {
            float[] notes;
            switch (sound)
            {
                case SimulationSound.Selection: notes = new[] { 880f }; break;
                case SimulationSound.Upgrade: notes = new[] { 523.25f, 659.25f, 783.99f }; break;
                case SimulationSound.Danger: notes = new[] { 392f, 293.66f }; break;
                case SimulationSound.Success: notes = new[] { 523.25f, 659.25f, 783.99f, 1046.5f }; break;
                default: notes = new[] { 392f, 329.63f, 261.63f }; break;
            }
            var noteLength = sound == SimulationSound.Selection ? 0.13f : 0.16f;
            var samples = new float[Mathf.CeilToInt(SampleRate * notes.Length * noteLength)];
            for (var i = 0; i < samples.Length; i++)
            {
                var time = i / (float)SampleRate;
                var note = Mathf.Min(notes.Length - 1, (int)(time / noteLength));
                var localTime = time - note * noteLength;
                var envelope = Mathf.Sin(Mathf.PI * localTime / noteLength);
                envelope *= envelope;
                samples[i] = envelope * (Mathf.Sin(2f * Mathf.PI * notes[note] * localTime) * 0.28f
                    + Mathf.Sin(4f * Mathf.PI * notes[note] * localTime) * 0.04f);
            }
            var clip = AudioClip.Create("Field station " + sound, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        static AudioClip CreateBed(bool melodic)
        {
            var samples = new float[SampleRate * 4];
            var notes = new[] { 261.63f, 329.63f, 392f, 329.63f, 293.66f, 349.23f, 392f, 523.25f };
            uint noise = 123456789;
            float wind = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var time = i / (float)SampleRate;
                if (melodic)
                {
                    var note = Mathf.Min(notes.Length - 1, (int)(time * 2f));
                    var localTime = time - note * 0.5f;
                    var envelope = Mathf.Exp(-7f * localTime) * Mathf.Sin(Mathf.PI * localTime / 0.5f);
                    samples[i] = envelope * Mathf.Sin(2f * Mathf.PI * notes[note] * localTime) * 0.22f;
                }
                else
                {
                    noise = unchecked(noise * 1664525u + 1013904223u);
                    wind = Mathf.Lerp(wind, ((noise >> 16) / 65535f) * 2f - 1f, 0.012f);
                    samples[i] = wind * (0.4f + 0.15f * Mathf.Sin(2f * Mathf.PI * time / 4f));
                }
            }
            var clip = AudioClip.Create(melodic ? "Forest Edge music" : "Forest Edge wind", samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }

    /// <summary>A short, deliberately composed two-note cue for a real litter arrival.</summary>
    internal static class SimulationBirthChime
    {
        const int SampleRate = 22050;
        const float Duration = 0.34f;
        static AudioClip clip;
        static float lastPlayedAt = -10f;

        public static void Play(GameObject owner)
        {
            if (owner == null || Time.unscaledTime - lastPlayedAt < 0.45f)
            {
                return;
            }

            var source = owner.GetComponent<AudioSource>();
            if (source == null)
            {
                source = owner.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.volume = 0.36f;
            }

            source.PlayOneShot(GetClip());
            lastPlayedAt = Time.unscaledTime;
        }

        static AudioClip GetClip()
        {
            if (clip != null)
            {
                return clip;
            }

            var samples = new float[Mathf.CeilToInt(SampleRate * Duration)];
            for (var index = 0; index < samples.Length; index++)
            {
                var time = index / (float)SampleRate;
                var noteTime = time < 0.15f ? time : time - 0.15f;
                var frequency = time < 0.15f ? 659.25f : 783.99f;
                var length = time < 0.15f ? 0.15f : Duration - 0.15f;
                var envelope = Mathf.Sin(Mathf.PI * noteTime / length);
                envelope *= envelope * Mathf.Exp(-3.4f * noteTime);
                var phase = 2f * Mathf.PI * frequency * noteTime;
                samples[index] = envelope * (Mathf.Sin(phase) * 0.24f
                    + Mathf.Sin(phase * 2f) * 0.045f);
            }

            clip = AudioClip.Create("Forest Edge birth chime", samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
