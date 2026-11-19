# Integrasi Face Reconstruction Offline

## Tujuan
Foto referensi dari galeri → estimasi identitas/geometri wajah → deformasi mesh karakter → proyeksi tekstur → blendshape ekspresi.

## Pipeline yang disarankan
1. **Face detection & landmarks**: model landmark wajah yang berjalan lokal.
2. **Single-image 3D reconstruction**: model ONNX yang mengestimasi parameter 3DMM/FLAME atau representasi mesh setara.
3. **Identity fitting**: fitting parameter identitas ke mesh target Bella.
4. **Texture projection**: proyeksikan tekstur foto ke UV wajah; koreksi pencahayaan/occlusion.
5. **Expression rig**: ekspresi animasi harus tetap menggunakan blendshapes terpisah dari parameter identitas.
6. **Persist**: simpan parameter wajah dan tekstur secara lokal, bukan hanya foto sementara.

## Kontrak kode
Implementasikan `IFaceReconstructionBackend` di `Assets/Scripts/Face/IFaceReconstructionBackend.cs`.
`FaceCustomizationController` saat ini menyimpan foto sebagai referensi dan menampilkan status bahwa backend rekonstruksi belum dipasang.

## Model dan lisensi
Jangan mengunduh model acak dari internet ke build produksi. Pilih model yang:
- mendukung inferensi Android ARM64,
- kompatibel dengan Unity Sentis atau ONNX Runtime Mobile,
- mengizinkan penggunaan dan distribusi sesuai lisensinya,
- cukup ringan untuk RAM/GPU perangkat target.

Unity Sentis package telah dicantumkan sebagai dependensi, tetapi file model `.onnx`/`.sentis` tidak disertakan. Model harus dipilih, diuji, dan ditambahkan secara terpisah.

## Kualitas realistis
Satu foto frontal tidak menyediakan informasi penuh untuk sisi belakang kepala, profil hidung, telinga, atau area tertutup rambut. Untuk kualitas terbaik, sediakan beberapa sudut foto atau lakukan koreksi manual setelah rekonstruksi.
