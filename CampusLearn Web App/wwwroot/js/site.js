// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// CampusLearn Application Console Logging
console.log("🎓 CampusLearn Web App: Application initialized");
console.log("🌐 CampusLearn: Browser:", navigator.userAgent);
console.log("📱 CampusLearn: Screen size:", window.screen.width + "x" + window.screen.height);
console.log("🔗 CampusLearn: Current URL:", window.location.href);

// Session and Authentication Status Checker
function checkAuthStatus() {
    console.log("🔐 CampusLearn: Checking authentication status...");
    
    // This will be populated by the server-side code
    const userInfo = window.campusLearnUser || null;
    
    if (userInfo) {
        console.log("✅ CampusLearn: User authenticated");
        console.log("👤 CampusLearn: User role:", userInfo.role);
        console.log("📧 CampusLearn: User email:", userInfo.email);
    } else {
        console.log("❌ CampusLearn: User not authenticated");
    }
    
    return userInfo;
}

// Database Connection Status
function logDatabaseStatus() {
    console.log("💾 CampusLearn: Database connection status will be checked by server");
    console.log("🏫 CampusLearn: Belgium Campus domain validation active");
}

// Page Navigation Logger
function logPageNavigation() {
    console.log("🧭 CampusLearn: Page navigation detected");
    console.log("📍 CampusLearn: From:", document.referrer || "Direct access");
    console.log("📍 CampusLearn: To:", window.location.href);
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log("🚀 CampusLearn: DOM content loaded");
    checkAuthStatus();
    logDatabaseStatus();
    
    // Log any error messages on the page
    const errorMessages = document.querySelectorAll('.alert-danger, .validation-summary-errors, .field-validation-error');
    if (errorMessages.length > 0) {
        console.warn("⚠️ CampusLearn: Error messages found on page:");
        errorMessages.forEach((msg, index) => {
            console.warn(`❌ Error ${index + 1}:`, msg.textContent.trim());
        });
    }
    
    // Log success messages
    const successMessages = document.querySelectorAll('.alert-success');
    if (successMessages.length > 0) {
        console.log("✅ CampusLearn: Success messages found on page:");
        successMessages.forEach((msg, index) => {
            console.log(`✅ Success ${index + 1}:`, msg.textContent.trim());
        });
    }
});

// Log page unload
window.addEventListener('beforeunload', function() {
    console.log("👋 CampusLearn: Leaving page");
});

// Network request logger (for fetch requests)
const originalFetch = window.fetch;
window.fetch = function(...args) {
    console.log("🌐 CampusLearn: Network request:", args[0]);
    return originalFetch.apply(this, args)
        .then(response => {
            console.log("📡 CampusLearn: Network response:", response.status, response.statusText);
            return response;
        })
        .catch(error => {
            console.error("🚨 CampusLearn: Network error:", error);
            throw error;
        });
};

// Write your JavaScript code below this line.
