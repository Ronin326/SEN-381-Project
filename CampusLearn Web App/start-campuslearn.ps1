# PostgreSQL Quick Start Script
# Run this to start PostgreSQL and your CampusLearn application

Write-Host "Starting CampusLearn System..." -ForegroundColor Cyan

# PostgreSQL paths
$pgBinPath = "C:\Program Files\PostgreSQL\17\bin"
$dataPath = "C:\Users\schol\PostgreSQL\data"
$logPath = "C:\Users\schol\PostgreSQL\logfile"

# Check if PostgreSQL is already running
Write-Host "Checking PostgreSQL status..." -ForegroundColor Yellow
$pgRunning = Get-Process -Name "postgres" -ErrorAction SilentlyContinue

if ($pgRunning) {
    Write-Host "PostgreSQL is already running!" -ForegroundColor Green
} else {
    Write-Host "Starting PostgreSQL..." -ForegroundColor Yellow
    
    # Start PostgreSQL with correct data directory
    & "$pgBinPath\pg_ctl.exe" -D "$dataPath" -l "$logPath" start
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL started successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to start PostgreSQL" -ForegroundColor Red
        pause
        exit 1
    }
}

# Wait a moment for PostgreSQL to fully start
Start-Sleep -Seconds 2

# Test connection
Write-Host "Testing database connection..." -ForegroundColor Yellow
$env:PGPASSWORD = "password"
$testResult = & "$pgBinPath\psql.exe" -U postgres -c "SELECT 'Connection successful!' as status;" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database connection successful!" -ForegroundColor Green
} else {
    Write-Host "Database connection failed" -ForegroundColor Red
    pause
    exit 1
}

# Check if campuslearn database exists
Write-Host "Checking for campuslearn database..." -ForegroundColor Yellow
$dbExists = & "$pgBinPath\psql.exe" -U postgres -lqt | Select-String "campuslearn"

if ($dbExists) {
    Write-Host "campuslearn database found!" -ForegroundColor Green
} else {
    Write-Host "Creating campuslearn database..." -ForegroundColor Yellow
    & "$pgBinPath\createdb.exe" -U postgres campuslearn
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "campuslearn database created!" -ForegroundColor Green
        
        # Apply schema if it exists
        $schemaPath = "wwwroot\db\schema.sql"
        if (Test-Path $schemaPath) {
            Write-Host "Applying database schema..." -ForegroundColor Yellow
            & "$pgBinPath\psql.exe" -U postgres -d campuslearn -f $schemaPath
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Database schema applied!" -ForegroundColor Green
            }
        }
    } else {
        Write-Host "Failed to create database" -ForegroundColor Red
    }
}

# Start the ASP.NET Core application
Write-Host "Starting CampusLearn Web Application..." -ForegroundColor Cyan
Write-Host "Application will be available at: http://localhost:5149" -ForegroundColor Green
Write-Host "Admin login: admin@belgiumcampus.ac.za / Admin@123456" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Red
Write-Host "======================================" -ForegroundColor Cyan

# Run the application
dotnet run
