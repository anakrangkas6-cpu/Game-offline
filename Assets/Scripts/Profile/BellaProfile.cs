using System;
using UnityEngine;

namespace BellaVirtualLife.Profile
{
    [Serializable]
    public class BellaProfileData
    {
        public string displayName = "Bella";
        public string personalityId = "bella_default";
        public string memoryDatabaseKey = "bella_memory_v1";
        public string faceReferenceFile = "BellaFace/face_reference.png";
        public float hunger = 20f;
        public float thirst = 15f;
        public float energy = 90f;
        public float happiness = 80f;
        public float hygiene = 90f;
        public long lastSimulationUnixTime;
    }

    public class BellaProfile : MonoBehaviour
    {
        public BellaProfileData Data { get; private set; }

        private const string SaveKey = "bella_profile_v1";

        private void Awake()
        {
            Load();
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SaveKey))
                Data = JsonUtility.FromJson<BellaProfileData>(PlayerPrefs.GetString(SaveKey));
            else
                Data = new BellaProfileData();

            if (Data == null) Data = new BellaProfileData();
        }

        public void Save()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Data));
            PlayerPrefs.Save();
        }

        public void ChangeAppearanceOnly(string faceFile)
        {
            // Keep name, memories, personality, house and life-state unchanged.
            Data.faceReferenceFile = faceFile;
            Save();
        }
    }
}
