# Enable TCP/IP for SQL Server SQLEXPRESS

## Method 1: Using SQL Server Configuration Manager (Recommended)

This is the primary tool for managing SQL Server protocols.

### Steps:

1. **Open SQL Server Configuration Manager:**
   - Press `Win + R`
   - Type: `SQLServerManager15.msc` (for SQL Server 2019)
      - Or: `SQLServerManager16.msc` (for SQL Server 2022)
      - Or: `SQLServerManager17.msc` (for SQL Server 2024)
   - Click OK

2. **Navigate to Network Protocols:**
   - In the left pane, expand: **SQL Server Network Configuration**
   - Click on: **Protocols for SQLEXPRESS**

3. **Enable TCP/IP:**
   - In the right pane, you'll see several protocols
   - Right-click on **TCP/IP**
   - Select **Enable**
   - You should see the status change to **Enabled** (green icon)

4. **Configure TCP/IP Port:**
   - Right-click on **TCP/IP** again
   - Select **Properties**
   - Go to the **IP Addresses** tab
   - Scroll down to **IPAll** section
   - Find **TCP Port** field
   - Verify it's set to: **1433** (standard SQL Server port)
   - If blank or different, set it to **1433**
   - Click **OK**

5. **Restart SQL Server Service:**
   - In SQL Server Configuration Manager, click **SQL Server Services** (in left pane)
   - Right-click **SQL Server (SQLEXPRESS)**
   - Click **Restart**
   - Wait for the service to restart (status will show "Running")

---

## Method 2: Alternative - Using Services (Windows Services)

If you just need to restart SQL Server:

1. Press `Win + R`
2. Type: `services.msc`
3. Find: **SQL Server (SQLEXPRESS)**
4. Right-click ? **Restart**

---

## Method 3: Using PowerShell (For Remote Management)

```powershell
# Enable TCP/IP via WMI (must run as Administrator)
$smo = 'Microsoft.SqlServer.Management.Smo.'
$wmi = new-object ($smo + 'Wmi.ManagedComputer')
$tcp = $wmi.ServerInstances['SQLEXPRESS'].ServerProtocols['Tcp']
$tcp.IsEnabled = $true
$tcp.Alter()
```

---

## Verification: Test Connection

After enabling TCP/IP and restarting, test the connection:

### From PowerShell:
```powershell
Test-NetConnection -ComputerName 10.0.0.102 -Port 1433 -InformationLevel Detailed
```

Expected output: `TcpTestSucceeded: True`

### Using SQLCMD:
```cmd
sqlcmd -S 10.0.0.102\SQLEXPRESS -U ProperlyAdmin -P "-Alek2025!"
1> SELECT @@VERSION
2> GO
```

### From Your .NET Application:
Navigate to: `https://localhost:xxxx/Diagnostics`

Should now show: ? Connected successfully!

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| TCP/IP still disabled after restart | Check if you have permissions (run as Administrator) |
| Port 1433 not responding | Check Windows Firewall - allow SQL Server or port 1433 |
| Service won't restart | Check Event Viewer for SQL Server errors |
| Still can't connect | Verify ProperlyAdmin user exists and database is created |

---

## Quick Checklist

- [ ] SQL Server Configuration Manager opened
- [ ] TCP/IP protocol is **Enabled** (green icon)
- [ ] TCP Port set to **1433** in IPAll section
- [ ] SQL Server (SQLEXPRESS) service **Restarted**
- [ ] Windows Firewall allows port 1433
- [ ] Test connection successful in diagnostics page

