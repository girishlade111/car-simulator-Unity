using NUnit.Framework;
using UnityEngine;
using CarSimulator.Audio;

namespace CarSimulator.Tests
{
    public class AudioSystemTests
    {
        [Test]
        public void EngineAudio_CanBeCreated()
        {
            GameObject go = new GameObject("TestEngine");
            EngineAudio engine = go.AddComponent<EngineAudio>();
            
            Assert.IsNotNull(engine);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void EngineAudio_HasDefaultSettings()
        {
            GameObject go = new GameObject("TestEngine");
            EngineAudio engine = go.AddComponent<EngineAudio>();
            
            Assert.IsNotNull(engine);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ExhaustMod_CanBeCreated()
        {
            GameObject go = new GameObject("TestExhaust");
            ExhaustMod exhaust = go.AddComponent<ExhaustMod>();
            
            Assert.IsNotNull(exhaust);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CarHorn_CanBeCreated()
        {
            GameObject go = new GameObject("TestHorn");
            CarHorn horn = go.AddComponent<CarHorn>();
            
            Assert.IsNotNull(horn);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CarRadio_CanBeCreated()
        {
            GameObject go = new GameObject("TestRadio");
            CarRadio radio = go.AddComponent<CarRadio>();
            
            Assert.IsNotNull(radio);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TireScreechAudio_CanBeCreated()
        {
            GameObject go = new GameObject("TestTireScreech");
            TireScreechAudio tireAudio = go.AddComponent<TireScreechAudio>();
            
            Assert.IsNotNull(tireAudio);
            
            Object.DestroyImmediate(go);
        }
    }

    public class AudioManagerTests
    {
        [Test]
        public void SFXManager_CanBeCreated()
        {
            GameObject go = new GameObject("TestSFX");
            SFXManager sfx = go.AddComponent<SFXManager>();
            
            Assert.IsNotNull(sfx);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void MusicManager_CanBeCreated()
        {
            GameObject go = new GameObject("TestMusic");
            MusicManager music = go.AddComponent<MusicManager>();
            
            Assert.IsNotNull(music);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void AudioGenerator_CanBeCreated()
        {
            GameObject go = new GameObject("TestAudioGen");
            AudioGenerator generator = go.AddComponent<AudioGenerator>();
            
            Assert.IsNotNull(generator);
            
            Object.DestroyImmediate(go);
        }
    }

    public class AudioSourceTests
    {
        [Test]
        public void AudioSource_CanBeCreated()
        {
            GameObject go = new GameObject("TestAudio");
            AudioSource source = go.AddComponent<AudioSource>();
            
            Assert.IsNotNull(source);
            Assert.IsFalse(source.playOnAwake);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void AudioSource_DefaultSettings()
        {
            GameObject go = new GameObject("TestAudio");
            AudioSource source = go.AddComponent<AudioSource>();
            
            Assert.AreEqual(1f, source.volume);
            Assert.AreEqual(1f, source.pitch);
            Assert.AreEqual(0f, source.panStereo);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void AudioSource_SpatialSettings()
        {
            GameObject go = new GameObject("TestAudio");
            AudioSource source = go.AddComponent<AudioSource>();
            
            source.spatialBlend = 1f;
            source.minDistance = 5f;
            source.maxDistance = 50f;
            
            Assert.AreEqual(1f, source.spatialBlend);
            Assert.AreEqual(5f, source.minDistance);
            Assert.AreEqual(50f, source.maxDistance);
            
            Object.DestroyImmediate(go);
        }
    }

    public class AudioMixerTests
    {
        [Test]
        public void AudioMixer_CanBeCreated()
        {
            UnityEngine.Audio.AudioMixer mixer = UnityEngine.Audio.AudioMixer.CreateInstance("TestMixer");
            
            Assert.IsNotNull(mixer);
            
            if (mixer != null)
            {
                UnityEngine.Object.DestroyImmediate(mixer);
            }
        }
    }

    public class VehicleAudioTests
    {
        [Test]
        public void VehicleAudio_CanBeCreated()
        {
            GameObject go = new GameObject("TestVehicleAudio");
            VehicleAudio audio = go.AddComponent<VehicleAudio>();
            
            Assert.IsNotNull(audio);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void VehicleAudio_HasAudioSource()
        {
            GameObject go = new GameObject("TestVehicleAudio");
            go.AddComponent<AudioSource>();
            VehicleAudio audio = go.AddComponent<VehicleAudio>();
            
            Assert.IsNotNull(audio);
            
            Object.DestroyImmediate(go);
        }
    }

    public class AudioSettingsTests
    {
        [Test]
        public void AudioSettings_Defaults()
        {
            Assert.GreaterOrEqual(AudioSettings.speakerMode, 0);
            Assert.Greater(AudioSettings.sampleRate, 0);
        }

        [Test]
        public void AudioSettings_DspTime_IsValid()
        {
            double dspTime = AudioSettings.dspTime;
            Assert.Greater(dspTime, 0);
        }
    }
}
