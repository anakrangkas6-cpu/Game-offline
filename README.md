# Bella Virtual Life — Unity Android Starter

Fondasi proyek Android Unity untuk simulasi kehidupan karakter Bella.

## Target
- Unity 6 (6000.0 LTS recommended)
- Android ARM64
- GTA-style third-person camera: follow, 360° touch orbit, pinch zoom, wall/furniture collision, and left virtual joystick
- Import foto wajah dari galeri
- Penyimpanan profil dan kebutuhan karakter secara lokal
- Arsitektur adapter untuk rekonstruksi wajah offline
- GitHub Actions Android APK build

## Status penting
Ini adalah **starter project / fondasi teknis**, bukan game ultra-realistic yang sudah selesai.
Foto galeri dapat dipilih dan disimpan sebagai tekstur. Agar foto mengubah geometri wajah 3D secara meyakinkan, proyek masih memerlukan:
1. Model karakter manusia 3D ber-topologi wajah yang mendukung blendshapes/shape keys.
2. Model rekonstruksi wajah 2D-ke-3D yang kompatibel dengan Unity Sentis/ONNX Runtime, beserta lisensi dan file modelnya.
3. Pipeline retopology, UV projection, material kulit, dan pengujian pada perangkat Android.

Tanpa model rekonstruksi dan karakter target tersebut, sistem sengaja tidak berpura-pura menghasilkan face swap 3D sempurna. Adapter `IFaceReconstructionBackend` disediakan untuk integrasi model offline.

## Isi proyek
- `Assets/Scripts/`: sistem simulasi, UI, kamera, galeri, profil, dan adapter wajah.
- `Assets/Editor/`: membuat scene starter dan Build Settings saat build.
- `Packages/manifest.json`: paket Unity dan NativeGallery.
- `.github/workflows/android-build.yml`: build APK menggunakan GameCI.
- `FACE_AI_INTEGRATION.md`: kontrak integrasi model AI offline.

## Kontrol kamera dan karakter
- Joystick transparan di kiri bawah: gerakkan Bella.
- Geser area kanan layar: putar kamera 360°.
- Cubit dua jari: zoom in/out.
- Kamera menggunakan SphereCast agar tidak menembus dinding/perabot.
- Di Unity Editor: tahan klik kanan dan gerakkan mouse untuk orbit; scroll untuk zoom; WASD untuk gerak jika joystick tidak dipakai.
- Pengaturan kamera dapat diubah pada komponen `ThirdPersonCamera` di Main Camera.

Kontrol ini adalah fondasi yang perlu diuji pada perangkat Android sasaran. Sistem input proyek menggunakan Unity Input Manager lama untuk kompatibilitas starter; bila proyek diubah ke Input System saja, aktifkan `Both` pada Player Settings atau sesuaikan input.

## Membuka proyek
1. Pasang Unity Hub dan Unity 6 dengan Android Build Support (SDK, NDK, OpenJDK).
2. Buka folder ini di Unity Hub.
3. Tunggu paket diunduh.
4. Buka `Assets/Scenes/BellaHome.unity` jika scene sudah dibuat oleh editor, atau jalankan menu `Bella > Create Starter Scene`.
5. Tambahkan aset karakter 3D yang memiliki rig humanoid dan blendshapes.
6. Atur Android sebagai Build Target.

## Build melalui GitHub Actions
Workflow memerlukan GitHub repository secrets:
- `UNITY_LICENSE` — file/serial license Unity yang valid dalam format yang didukung GameCI.
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

Buka Actions → Android APK Build → Run workflow.
Artifact bernama `BellaVirtualLife-Android` akan berisi APK jika build berhasil.

Catatan: penggunaan lisensi Unity Personal/Pro harus sesuai ketentuan Unity. Jangan menaruh kredensial di source code.

## Catatan Android
NativeGallery dipakai untuk pemilih foto. Pertama kali, Android akan meminta akses sesuai mekanisme Photo Picker/izin yang berlaku.
Foto diproses lokal oleh aplikasi setelah dipilih. Jangan unggah foto wajah ke server tanpa persetujuan eksplisit pengguna.
