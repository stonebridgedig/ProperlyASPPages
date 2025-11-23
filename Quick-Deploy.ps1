# Quick Deploy Script - Stores credentials securely using Windows Credential Manager
# Install CredentialManager module: Install-Module -Name CredentialManager -Force

param(
    [string]$ServerIP = "10.0.0.102",
    [string]$CredentialName = "IIS-10.0.0.102-Deploy",
    [switch]$SaveCredentials = $false
)

$ErrorActionPreference = "Stop"

# Check if CredentialManager is available
$credModule = Get-Module -ListAvailable -Name CredentialManager
if (-not $credModule) {
    Write-Warning "CredentialManager module not installed. Will prompt for password each time."
    Write-Host "To install: Install-Module -Name CredentialManager -Force" -ForegroundColor Yellow
    Write-Host ""
}

$projectPath = "$PSScriptRoot\ProperlyASPPages\ProperlyASPPages.csproj"
$publishProfile = "IIS-Local"
$username = "Administrator"
$password = $null

# Try to get stored credentials
if ($credModule) {
    try {
        $cred = Get-StoredCredential -Target $CredentialName -ErrorAction SilentlyContinue
        if ($cred) {
            $username = $cred.UserName
            $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($cred.Password)
            $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
            Write-Host "Using stored credentials for: $username" -ForegroundColor Green
        }
    } catch {
        # Credentials not found
    }
}

# Prompt for credentials if not found
if (-not $password) {
    Write-Host "Enter credentials for IIS Server ($ServerIP)" -ForegroundColor Cyan
    $inputUsername = Read-Host "Username [$username]"
    if ($inputUsername) { $username = $inputUsername }
    
    $securePassword = Read-Host "Password" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    
    # Save credentials if requested
    if ($SaveCredentials -and $credModule) {
        try {
            $securePwd = ConvertTo-SecureString -String $password -AsPlainText -Force
            $credObject = New-Object System.Management.Automation.PSCredential($username, $securePwd)
            New-StoredCredential -Target $CredentialName -Credentials $credObject -Type Generic -Persist LocalMachine | Out-Null
            Write-Host "Credentials saved securely to Windows Credential Manager" -ForegroundColor Green
        } catch {
            Write-Warning "Failed to save credentials: $_"
        }
    }
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing to IIS Server" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Target: http://$ServerIP" -ForegroundColor Gray
Write-Host ""

try {
    & dotnet publish $projectPath `
        /p:DeployOnBuild=true `
        /p:PublishProfile=$publishProfile `
        /p:Configuration=Release `
        /p:MSDeployServiceURL=$ServerIP `
        /p:Username=$username `
        /p:Password=$password
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "? Deployment Successful!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "URL: http://$ServerIP/ProperlyASPPages" -ForegroundColor Green
    } else {
        Write-Error "Deployment failed with exit code: $LASTEXITCODE"
    }
} catch {
    Write-Error "Deployment error: $_"
    exit 1
}
