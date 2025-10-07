// Campus Learn Console Logging System
console.log("🎓 CampusLearn: Validation script loaded successfully");

function ValidForm(){
    console.log("🔍 CampusLearn: Form validation started");
    
    const email = document.getElementById("email").value.trim();
    const requiredDomain = "@belgiumcampus.ac.za";
    
    console.log("📧 CampusLearn: Email entered:", email);
    console.log("🏫 CampusLearn: Required domain:", requiredDomain);
    console.log("✅ CampusLearn: Email validation check:", email.endsWith(requiredDomain));

    if (!email.endsWith(requiredDomain)) {
        console.error("❌ CampusLearn: Email validation FAILED - Invalid domain");
        console.log("🚫 CampusLearn: Expected domain:", requiredDomain);
        console.log("🚫 CampusLearn: Provided email:", email);
        event.preventDefault();
        window.alert("Please use your Belgium Campus email address (must end with " + requiredDomain + ").");
        return false;
    }
    
    console.log("✅ CampusLearn: Email validation PASSED");
    return true;
};

// Add page load confirmation
document.addEventListener('DOMContentLoaded', function() {
    console.log("🚀 CampusLearn: Page loaded successfully");
    console.log("🔧 CampusLearn: Current page:", window.location.pathname);
    console.log("⏰ CampusLearn: Timestamp:", new Date().toISOString());
    
    // Check if we're on login or register page
    if (window.location.pathname.includes('Login') || window.location.pathname.includes('Register')) {
        console.log("🔐 CampusLearn: Authentication page detected");
        
        // Log form elements
        const emailField = document.getElementById("email");
        const passwordField = document.getElementById("password");
        
        if (emailField) {
            console.log("📧 CampusLearn: Email field found");
            emailField.addEventListener('input', function() {
                console.log("⌨️ CampusLearn: Email field updated:", this.value);
            });
        }
        
        if (passwordField) {
            console.log("🔒 CampusLearn: Password field found");
            passwordField.addEventListener('input', function() {
                console.log("⌨️ CampusLearn: Password field updated (length):", this.value.length);
            });
        }
    }
});

// Add form submission logging
document.addEventListener('submit', function(e) {
    const form = e.target;
    console.log("📝 CampusLearn: Form submission detected");
    console.log("📋 CampusLearn: Form action:", form.action);
    console.log("📋 CampusLearn: Form method:", form.method);
    
    // Log form data (excluding sensitive password)
    const formData = new FormData(form);
    for (let [key, value] of formData.entries()) {
        if (key.toLowerCase().includes('password')) {
            console.log(`📋 CampusLearn: Form data - ${key}: [HIDDEN] (length: ${value.length})`);
        } else {
            console.log(`📋 CampusLearn: Form data - ${key}: ${value}`);
        }
    }
});