using System.IO;
using UnityEngine;
using UnityEngine.UI;
using BellaVirtualLife.Profile;

namespace BellaVirtualLife.Face
{
    public class FaceCustomizationController : MonoBehaviour
    {
        [Header("Optional UI")]
        [SerializeField] private RawImage preview;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Renderer faceRenderer;

        private Texture2D selectedPhoto;
        private IFaceReconstructionBackend reconstructionBackend;

        public void ChooseFacePhoto()
        {
#if UNITY_EDITOR
            SetStatus("Di Android, gunakan tombol Pilih Foto untuk membuka galeri.");
#else
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery(
                path => {
                    if (string.IsNullOrEmpty(path))
                    {
                        SetStatus("Pemilihan foto dibatalkan.");
                        return;
                    }

                    Texture2D image = NativeGallery.LoadImageAtPath(path, 2048, false, false);
                    if (image == null)
                    {
                        SetStatus("Foto tidak dapat dibaca.");
                        return;
                    }

                    ApplyReferencePhoto(image);
                },
                "Pilih foto wajah Bella",
                "image/*",
                2048
            );

            SetStatus("Izin galeri: " + permission);
#endif
        }

        private void ApplyReferencePhoto(Texture2D image)
        {
            if (selectedPhoto != null) Destroy(selectedPhoto);
            selectedPhoto = image;

            if (preview != null) preview.texture = selectedPhoto;

            // Persist a local copy. This is a reference image, not yet a reconstructed 3D face.
            string folder = Path.Combine(Application.persistentDataPath, "BellaFace");
            Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, "face_reference.png"), selectedPhoto.EncodeToPNG());

            if (reconstructionBackend != null && reconstructionBackend.IsReady)
            {
                SetStatus("Foto dipilih. Memulai rekonstruksi wajah offline...");
                reconstructionBackend.Reconstruct(selectedPhoto, (result, error) =>
                {
                    if (!string.IsNullOrEmpty(error) || result == null)
                    {
                        SetStatus("Rekonstruksi gagal: " + error);
                        return;
                    }

                    if (faceRenderer != null && result.projectedFaceTexture != null)
                        faceRenderer.material.mainTexture = result.projectedFaceTexture;

                    SetStatus("Hasil backend diterapkan. Periksa kecocokan mesh dan UV karakter.");
                });
            }
            else
            {
                SetStatus("Foto tersimpan secara lokal. Backend rekonstruksi 3D offline belum dipasang.");
            }
        }

        public void SetBackend(IFaceReconstructionBackend backend)
        {
            reconstructionBackend = backend;
        }

        private void SetStatus(string message)
        {
            Debug.Log("[Bella Face] " + message);
            if (statusLabel != null) statusLabel.text = message;
        }

        private void OnDestroy()
        {
            if (selectedPhoto != null) Destroy(selectedPhoto);
        }
    }
}
