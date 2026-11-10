# El Cantar de Mío Cid para Android

Versión móvil sin conexión del juego de 100 decisiones. Conserva la regla de
volver al principio al equivocarse y contiene una imagen por pregunta, además
de la portada y el final.

El APK instalable se entrega en `../publicar/El Cantar de Mio Cid - Android.apk`.
Está firmado con la clave de depuración local para instalación directa; para
publicarlo en Google Play se necesitará una clave de publicación propia.

Para reconstruirlo en este equipo, ejecuta `build-android.ps1`. El script usa
Java 17, Gradle 8.11.1 y Android SDK Platform 35 bajo `android/.tools/`, y
regenera los recursos desde el proyecto principal.
