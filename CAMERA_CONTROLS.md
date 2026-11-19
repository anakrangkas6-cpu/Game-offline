# Kamera GTA-style Bella

## Komponen
- `ThirdPersonCamera.cs`: follow, orbit 360°, pinch zoom, scroll zoom, dan collision sphere-cast.
- `VirtualJoystick.cs`: joystick layar sentuh kiri bawah.
- `BellaCharacterMotor.cs`: gerak karakter relatif terhadap arah kamera.

## Scene setup
Menu Unity `Bella > Create Starter Scene` membuat scene uji dengan kapsul placeholder, CharacterController, kamera, EventSystem, dan joystick.
Ganti kapsul dengan model Bella yang sudah memiliki rig/animator. Pastikan collider/CharacterController berada pada root karakter dan kamera tetap menargetkan root tersebut.

## Kontrol Android
- Joystick kiri bawah: bergerak.
- Geser satu jari di area kanan: orbit kamera.
- Cubit dua jari: zoom.
- Kamera menghindari collider lingkungan melalui SphereCast. Atur `Collision Layers` agar hanya mencakup dinding, lantai, dan furnitur; jangan masukkan layer UI.

## Catatan
Ini adalah implementasi kode, bukan hasil uji perangkat. Uji sensitivitas, safe area, dan performa pada perangkat target. Pastikan objek lingkungan memiliki Collider.
