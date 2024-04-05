#define Name "Ivs Calculator"
#define Version "1.0.0"
#define Publisher "Punk Srenk"
#define URL "https://ivs.fit.vutbr.cz"
#define Executable "IvsCalc.exe"
#define Output "IvsCalc_x64"

[Setup]
AppId={{2FBFD092-D718-457D-A04E-5BCBBD2E2606}
AppName={#Name}
AppVersion={#Version}
;AppVerName={#Name} {#Version}
AppPublisher={#Publisher}
AppPublisherURL={#URL}
AppSupportURL={#URL}
AppUpdatesURL={#URL}
DefaultDirName={commonpf64}\Punk Srenk\Ivs Calculator
DisableProgramGroupPage=yes

OutputDir=output
OutputBaseFilename={#Output}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\IvsCalc\bin\publish\win10-x64\{#Executable}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\IvsCalc\bin\publish\win10-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#Name}"; Filename: "{app}\{#Executable}"
Name: "{autodesktop}\{#Name}"; Filename: "{app}\{#Executable}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#Executable}"; Description: "{cm:LaunchProgram,{#StringChange(Name, '&', '&&')}}"; Flags: nowait postinstall skipifsilent