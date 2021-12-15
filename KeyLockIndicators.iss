#define AppName "KeyLockIndicators"
#define AppVersionMajor "1"
#define AppVersionMinor "1"
#define AppLogFile "KeyLockIndicators.log"
#define AppServiceName "KeyLockIndicatorsSvc"
#define AppServiceFile "KeyLockIndicators.Launcher.exe"
#define AppServiceDisplay "KeyLockIndicators Launcher"
#define AppServiceDescription "Launches the KeyLockIndicators notifier."

[Setup]
AppId={{E660C891-802B-415A-8919-64840187DF9C}
SetupMutex=Global\E660C891-802B-415A-8919-64840187DF9C
AppCopyright=Copyright (c) 2021 Philippe Coulombe
AppPublisher=Philippe Coulombe
AppVersion={#AppVersionMajor}.{#AppVersionMinor}.0.0
VersionInfoVersion={#AppVersionMajor}.{#AppVersionMinor}.0.0
AppVerName={#AppName} {#AppVersionMajor}.{#AppVersionMinor}
AppName={#AppName}
DefaultDirName={commonpf}\{#AppName}
UninstallDisplayIcon={app}\KeyLockIndicators.Notifier.exe
OutputBaseFilename={#AppName}Setup
OutputDir=.
LicenseFile=LICENSE.TXT
DisableProgramGroupPage=yes
DisableDirPage=yes
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
MinVersion=10.0.17763
WizardSizePercent=120,100

[Files]
Source: "LICENSE.TXT"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "Publish\*"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "Publish\Resources\*"; DestDir: {app}\Resources; Flags: restartreplace uninsrestartdelete ignoreversion

[Run]
Filename: {sys}\sc.exe; Parameters: "create {#AppServiceName} displayname= ""{#AppServiceDisplay}"" start= auto depend= SENS binpath= ""{app}\{#AppServiceFile}"""; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "description {#AppServiceName} ""{#AppServiceDescription}"""; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start {#AppServiceName}"; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop {#AppServiceName}"; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete {#AppServiceName}"; Flags: runhidden

[UninstallDelete]
Type: files; Name: "{app}\{#AppLogFile}"

[Code]
procedure InitializeWizard();
begin
  WizardForm.LicenseMemo.Font.Name := 'Consolas';
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Exec('sc.exe', 'stop {#AppServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Exec('sc.exe', 'delete {#AppServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := '';
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then begin
    Exec('sc.exe', 'stop {#AppServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Exec('sc.exe', 'delete {#AppServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
