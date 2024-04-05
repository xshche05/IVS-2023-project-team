#define Name "Ivs Calculator"
#define Version "1.0.0"
#define Publisher "Punk Srenk"
#define URL "https://ivs.fit.vutbr.cz"
#define Executable "IvsCalc.exe"
#define Output "IvsCalc_x86"

[Setup]
AppId={{A30212CD-CD6A-45E3-BC82-C710C3269897}
AppName={#Name}
AppVersion={#Version}
;AppVerName={#Name} {#Version}
AppPublisher={#Publisher}
AppPublisherURL={#URL}
AppSupportURL={#URL}
AppUpdatesURL={#URL}
DefaultDirName={commonpf32}\Punk Srenk\Ivs Calculator
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
Source: "..\IvsCalc\bin\publish\win10-x86\{#Executable}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\IvsCalc\bin\publish\win10-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#Name}"; Filename: "{app}\{#Executable}"
Name: "{autodesktop}\{#Name}"; Filename: "{app}\{#Executable}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#Executable}"; Description: "{cm:LaunchProgram,{#StringChange(Name, '&', '&&')}}"; Flags: nowait postinstall skipifsilent