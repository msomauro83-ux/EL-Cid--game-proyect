#define AppName "El Cantar de Mío Cid"
#define AppExe "El Cantar de Mio Cid.exe"
#define AppVersion "2.0"

[Setup]
AppId={{B5D0F0F1-8738-4C1D-AF2A-EBC12C1B7874}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
DefaultDirName={localappdata}\El Cantar de Mio Cid
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\publicar
OutputBaseFilename=Instalar El Cantar de Mio Cid
SetupIconFile=assets\Cid.ico
UninstallDisplayIcon={app}\Cid.ico
WizardStyle=modern dark polar includetitlebar
WizardSizePercent=110
Compression=lzma2/normal
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible
VersionInfoVersion=2.0.0.0
VersionInfoProductName={#AppName}
VersionInfoDescription=Instalador de El Cantar de Mío Cid
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Files]
Source: "..\publicar\Juego\El Cantar de Mio Cid.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publicar\Juego\decisiones.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publicar\Juego\Cid.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publicar\Juego\Cid.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publicar\Juego\imagenes\*"; DestDir: "{app}\imagenes"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"; IconFilename: "{app}\Cid.ico"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"; IconFilename: "{app}\Cid.ico"

[Run]
Filename: "{app}\{#AppExe}"; Description: "Iniciar {#AppName}"; Flags: nowait postinstall skipifsilent
