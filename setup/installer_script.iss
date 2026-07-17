[Setup]
AppId={{9F5B3A1C-2496-4EAF-8900-ABC585451781}
AppPublisher=sedecillion
UninstallDisplayIcon={app}\WindowsQOL.exe
AppName=Windows QOL
AppVersion=1.0
DefaultDirName={autopf}\Windows QOL
OutputBaseFilename=WindowsQOL_Installer
Compression=lzma2/ultra64
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
UsedUserAreasWarning=no
DisableProgramGroupPage=yes

[Files]
Source: "..\ahk_service\W_QOL.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ahk_service\SignTool.exe"; DestDir: "{app}"; Flags: ignoreversion deleteafterinstall
Source: "..\settings_UI\bin\win10-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{commonprograms}\Windows QOL"; Filename: "{app}\WindowsQOL.exe"
Name: "{commondesktop}\Windows QOL"; Filename: "{app}\WindowsQOL.exe"
Name: "{commonprograms}\Uninstall Windows QOL"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\SignTool.exe"; Flags: runhidden waituntilterminated
Filename: "{app}\WindowsQOL.exe"; Description: "Launch Windows QOL"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{cmd}"; Parameters: "/C taskkill /F /IM W_QOL.exe /T"; Flags: runhidden; RunOnceId: "KillAHK"
Filename: "{cmd}"; Parameters: "/C taskkill /F /IM WindowsQOL.exe /T"; Flags: runhidden; RunOnceId: "KillUI"
Filename: "certutil.exe"; Parameters: "-delstore Root ""WindowsQOL"""; Flags: runhidden; RunOnceId: "DelCert"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: none; ValueName: "W_QOL_Service"; Flags: uninsdeletevalue

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\WindowsQOL"