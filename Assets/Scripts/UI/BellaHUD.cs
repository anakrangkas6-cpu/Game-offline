using UnityEngine;
using UnityEngine.UI;
using BellaVirtualLife.Life;
using BellaVirtualLife.Profile;
using BellaVirtualLife.Face;

namespace BellaVirtualLife.UI
{
    public class BellaHUD : MonoBehaviour
    {
        [SerializeField] private BellaProfile profile;
        [SerializeField] private LifeSimulation life;
        [SerializeField] private FaceCustomizationController face;
        [SerializeField] private Text statusText;

        private void Start()
        {
            if (profile == null) profile = FindObjectOfType<BellaProfile>();
            if (life == null) life = FindObjectOfType<LifeSimulation>();
            if (life != null) life.OnNeedsChanged += Refresh;
            Refresh();
        }

        public void OnChooseFacePressed() => face?.ChooseFacePhoto();
        public void OnFeedPressed() => life?.Feed();
        public void OnDrinkPressed() => life?.GiveDrink();
        public void OnSleepPressed() => life?.Sleep();

        private void Refresh()
        {
            if (statusText == null || profile == null || profile.Data == null) return;
            var d = profile.Data;
            statusText.text =
                $"Bella  |  Lapar {d.hunger:0}%  |  Haus {d.thirst:0}%\\n" +
                $"Energi {d.energy:0}%  |  Bahagia {d.happiness:0}%";
        }

        private void OnDestroy()
        {
            if (life != null) life.OnNeedsChanged -= Refresh;
        }
    }
}
