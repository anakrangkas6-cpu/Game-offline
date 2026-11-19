using System;
using UnityEngine;
using BellaVirtualLife.Profile;

namespace BellaVirtualLife.Life
{
    public class LifeSimulation : MonoBehaviour
    {
        [SerializeField] private BellaProfile profile;
        [SerializeField] private float hungerPerGameHour = 7f;
        [SerializeField] private float thirstPerGameHour = 9f;
        [SerializeField] private float energyDrainPerGameHour = 4f;

        public event Action OnNeedsChanged;

        private void Start()
        {
            if (profile == null) profile = FindObjectOfType<BellaProfile>();
            ApplyOfflineProgress();
            InvokeRepeating(nameof(Tick), 1f, 1f);
        }

        private void Tick()
        {
            if (profile == null || profile.Data == null) return;
            // One simulation minute per real minute; needs update gradually.
            float perSecond = 1f / 60f;
            profile.Data.hunger = Mathf.Clamp(profile.Data.hunger + hungerPerGameHour / 60f * perSecond, 0, 100);
            profile.Data.thirst = Mathf.Clamp(profile.Data.thirst + thirstPerGameHour / 60f * perSecond, 0, 100);
            profile.Data.energy = Mathf.Clamp(profile.Data.energy - energyDrainPerGameHour / 60f * perSecond, 0, 100);
            OnNeedsChanged?.Invoke();
        }

        public void Feed(float amount = 35f)
        {
            profile.Data.hunger = Mathf.Clamp(profile.Data.hunger - amount, 0, 100);
            profile.Save();
            OnNeedsChanged?.Invoke();
        }

        public void GiveDrink(float amount = 30f)
        {
            profile.Data.thirst = Mathf.Clamp(profile.Data.thirst - amount, 0, 100);
            profile.Save();
            OnNeedsChanged?.Invoke();
        }

        public void Sleep(float hours = 8f)
        {
            profile.Data.energy = Mathf.Clamp(profile.Data.energy + hours * 12f, 0, 100);
            profile.Save();
            OnNeedsChanged?.Invoke();
        }

        private void ApplyOfflineProgress()
        {
            if (profile == null || profile.Data == null) return;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (profile.Data.lastSimulationUnixTime > 0)
            {
                float elapsedHours = Mathf.Clamp((now - profile.Data.lastSimulationUnixTime) / 3600f, 0, 24 * 7);
                profile.Data.hunger = Mathf.Clamp(profile.Data.hunger + elapsedHours * hungerPerGameHour, 0, 100);
                profile.Data.thirst = Mathf.Clamp(profile.Data.thirst + elapsedHours * thirstPerGameHour, 0, 100);
                profile.Data.energy = Mathf.Clamp(profile.Data.energy - elapsedHours * energyDrainPerGameHour, 0, 100);
            }
            profile.Data.lastSimulationUnixTime = now;
            profile.Save();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && profile != null && profile.Data != null)
            {
                profile.Data.lastSimulationUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                profile.Save();
            }
        }

        private void OnApplicationQuit()
        {
            if (profile != null && profile.Data != null)
            {
                profile.Data.lastSimulationUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                profile.Save();
            }
        }
    }
}
