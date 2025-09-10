# CampusLearn API Test Script
# Usage: .\test-api.ps1 [test-name]

param(
    [string]$TestName = "all"
)

$API_BASE = "http://localhost:5149/api/auth"
$headers = @{ "Content-Type" = "application/json" }

function Write-TestResult {
    param([string]$Message, [string]$Status = "INFO")
    $timestamp = Get-Date -Format "HH:mm:ss"
    $icon = switch ($Status) {
        "SUCCESS" { "✅" }
        "ERROR" { "❌" }
        "WARNING" { "⚠️" }
        default { "📋" }
    }
    Write-Host "[$timestamp] $icon $Message" -ForegroundColor $(
        switch ($Status) {
            "SUCCESS" { "Green" }
            "ERROR" { "Red" }
            "WARNING" { "Yellow" }
            default { "White" }
        }
    )
}

function Test-ApiEndpoint {
    param([string]$Endpoint, [hashtable]$Body = $null, [string]$Method = "GET")
    
    try {
        $params = @{
            Uri = "$API_BASE$Endpoint"
            Method = $Method
            Headers = $headers
        }
        
        if ($Body -and $Method -ne "GET") {
            $params.Body = $Body | ConvertTo-Json
        }
        
        $response = Invoke-RestMethod @params
        return @{ Success = $true; Data = $response }
    }
    catch {
        return @{ Success = $false; Error = $_.Exception.Message; StatusCode = $_.Exception.Response.StatusCode }
    }
}

function Test-UserRegistration {
    Write-TestResult "Testing user registration..." "INFO"
    
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    $testUser = @{
        firstName = "Test"
        lastName = "User"
        email = "test.user$timestamp@belgiumcampus.ac.za"
        password = "TestPass123!"
        role = "Student"
    }
    
    $result = Test-ApiEndpoint -Endpoint "/register" -Method "POST" -Body $testUser
    
    if ($result.Success) {
        Write-TestResult "Registration successful: $($result.Data | ConvertTo-Json -Compress)" "SUCCESS"
        return $testUser.email
    } else {
        Write-TestResult "Registration failed: $($result.Error)" "ERROR"
        return $null
    }
}

function Test-UserLogin {
    param([string]$Email = "admin@belgiumcampus.ac.za", [string]$Password = "Admin@2024!")
    
    Write-TestResult "Testing user login for: $Email" "INFO"
    
    $loginData = @{
        email = $Email
        password = $Password
    }
    
    $result = Test-ApiEndpoint -Endpoint "/login" -Method "POST" -Body $loginData
    
    if ($result.Success) {
        Write-TestResult "Login successful: $($result.Data | ConvertTo-Json -Compress)" "SUCCESS"
        return $true
    } else {
        Write-TestResult "Login failed: $($result.Error)" "ERROR"
        return $false
    }
}

function Test-SystemStatus {
    Write-TestResult "Checking system status..." "INFO"
    
    $result = Test-ApiEndpoint -Endpoint "/status"
    
    if ($result.Success) {
        Write-TestResult "System status: $($result.Data | ConvertTo-Json -Compress)" "SUCCESS"
    } else {
        Write-TestResult "System status check failed: $($result.Error)" "ERROR"
    }
}

function Test-UserCount {
    Write-TestResult "Getting user count..." "INFO"
    
    $result = Test-ApiEndpoint -Endpoint "/user-count"
    
    if ($result.Success) {
        Write-TestResult "User statistics: $($result.Data | ConvertTo-Json -Compress)" "SUCCESS"
        return $result.Data
    } else {
        Write-TestResult "User count failed: $($result.Error)" "ERROR"
        return $null
    }
}

function Test-DomainValidation {
    Write-TestResult "Testing domain validation..." "INFO"
    
    $invalidEmails = @(
        "test@gmail.com",
        "user@yahoo.com", 
        "admin@hotmail.com",
        "student@belgiumcampus.com"
    )
    
    $rejectedCount = 0
    
    foreach ($email in $invalidEmails) {
        $testUser = @{
            firstName = "Invalid"
            lastName = "User"
            email = $email
            password = "TestPass123!"
            role = "Student"
        }
        
        $result = Test-ApiEndpoint -Endpoint "/register" -Method "POST" -Body $testUser
        
        if (-not $result.Success) {
            Write-TestResult "✅ Correctly rejected: $email" "SUCCESS"
            $rejectedCount++
        } else {
            Write-TestResult "❌ Incorrectly accepted: $email" "ERROR"
        }
    }
    
    Write-TestResult "Domain validation test completed: $rejectedCount/$($invalidEmails.Count) invalid emails rejected" "INFO"
}

function Test-AdminPrevention {
    Write-TestResult "Testing admin role prevention..." "INFO"
    
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    $adminUser = @{
        firstName = "Admin"
        lastName = "Test"
        email = "admintest$timestamp@belgiumcampus.ac.za"
        password = "AdminTest123!"
        role = "Admin"
    }
    
    $result = Test-ApiEndpoint -Endpoint "/register" -Method "POST" -Body $adminUser
    
    if (-not $result.Success) {
        Write-TestResult "✅ Admin role creation correctly prevented" "SUCCESS"
    } else {
        Write-TestResult "❌ Admin role creation was allowed" "ERROR"
    }
}

function Run-StressTest {
    Write-TestResult "Running stress test with concurrent registrations..." "INFO"
    
    $jobs = @()
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    
    # Create 5 concurrent registration jobs
    for ($i = 1; $i -le 5; $i++) {
        $scriptBlock = {
            param($i, $timestamp, $API_BASE)
            
            $testUser = @{
                firstName = "Stress$i"
                lastName = "Test"
                email = "stress$i.$timestamp@belgiumcampus.ac.za"
                password = "StressTest123!"
                role = "Student"
            }
            
            try {
                $response = Invoke-RestMethod -Uri "$API_BASE/register" -Method "POST" -Body ($testUser | ConvertTo-Json) -ContentType "application/json"
                return @{ Success = $true; User = $i }
            }
            catch {
                return @{ Success = $false; User = $i; Error = $_.Exception.Message }
            }
        }
        
        $jobs += Start-Job -ScriptBlock $scriptBlock -ArgumentList $i, $timestamp, $API_BASE
    }
    
    # Wait for all jobs to complete
    $results = $jobs | Wait-Job | Receive-Job
    $jobs | Remove-Job
    
    $successCount = ($results | Where-Object { $_.Success }).Count
    Write-TestResult "Stress test completed: $successCount/5 registrations successful" "INFO"
}

# Main execution
Write-Host "🧪 CampusLearn API Test Suite" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

switch ($TestName.ToLower()) {
    "registration" { Test-UserRegistration | Out-Null }
    "login" { Test-UserLogin | Out-Null }
    "status" { Test-SystemStatus }
    "count" { Test-UserCount | Out-Null }
    "domain" { Test-DomainValidation }
    "admin" { Test-AdminPrevention }
    "stress" { Run-StressTest }
    "all" {
        Write-TestResult "Running comprehensive test suite..." "INFO"
        Test-SystemStatus
        $stats = Test-UserCount
        Test-UserLogin | Out-Null
        $newUser = Test-UserRegistration
        Test-DomainValidation
        Test-AdminPrevention
        
        if ($newUser) {
            Test-UserLogin -Email $newUser -Password "TestPass123!" | Out-Null
        }
        
        Write-TestResult "All tests completed!" "SUCCESS"
    }
    default {
        Write-Host "Available tests: registration, login, status, count, domain, admin, stress, all" -ForegroundColor Yellow
    }
}

Write-Host "`n✨ Test execution complete!" -ForegroundColor Green
