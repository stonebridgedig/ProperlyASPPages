# Test SQL Server Connection
$serverInstance = "10.0.0.102\SQLEXPRESS"
$database = "ProperlyIdentityDB"
$username = "ProperlyAdmin"
$password = "-Alek2025!"

Write-Host "Testing SQL Server connectivity..." -ForegroundColor Cyan

# Test 1: Ping the server
Write-Host "`n1. Testing network connectivity to 10.0.0.102..." -ForegroundColor Yellow
Test-Connection -ComputerName 10.0.0.102 -Count 2 -ErrorAction SilentlyContinue

# Test 2: Test TCP port 1433 (default SQL Server port)
Write-Host "`n2. Testing SQL Server port 1433..." -ForegroundColor Yellow
Test-NetConnection -ComputerName 10.0.0.102 -Port 1433 -WarningAction SilentlyContinue

# Test 3: Try to connect using SqlClient
Write-Host "`n3. Testing SQL Server authentication..." -ForegroundColor Yellow
$connectionString = "Server=$serverInstance;Database=$database;User ID=$username;Password=$password;TrustServerCertificate=True;Encrypt=False;Connection Timeout=5"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    Write-Host "? Connection successful!" -ForegroundColor Green
    Write-Host "Server Version: $($connection.ServerVersion)" -ForegroundColor Green
    Write-Host "Database: $($connection.Database)" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "? Connection failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nCommon issues:" -ForegroundColor Yellow
    Write-Host "  - SQL Server service not running"
    Write-Host "  - TCP/IP not enabled in SQL Server Configuration Manager"
    Write-Host "  - Windows Firewall blocking port 1433"
    Write-Host "  - SQL Server authentication not enabled"
    Write-Host "  - Incorrect credentials"
}
