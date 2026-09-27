using UnityEngine;

namespace SaltyGame
{
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
