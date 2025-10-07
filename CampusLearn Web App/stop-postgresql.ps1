# Stop CampusLearn System
# Run this to stop PostgreSQL cleanly

Write-Host "🛑 Stopping CampusLearn System..." -ForegroundColor Red

# PostgreSQL paths
$pgBinPath = "C:\Program Files\PostgreSQL\17\bin"
$dataPath = "C:\Users\schol\PostgreSQL\data"

# Stop PostgreSQL
Write-Host "🔄 Stopping PostgreSQL..." -ForegroundColor Yellow
& "$pgBinPath\pg_ctl.exe" -D "$dataPath" stop

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ PostgreSQL stopped successfully!" -ForegroundColor Green
} else {
    Write-Host "⚠️  PostgreSQL may already be stopped" -ForegroundColor Yellow
}

Write-Host "✅ CampusLearn system stopped!" -ForegroundColor Green
pause
