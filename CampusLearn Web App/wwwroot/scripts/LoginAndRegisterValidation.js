function ValidForm(){
    const email = document.getElementById("email").value.trim();
    const requiredDomain = "@belgiumcampus.ac.za";

    if (!email.endsWith(requiredDomain)) {
        event.preventDefault();
        window.alert("Please use your Belgium Campus email address (must end with " + requiredDomain + ").");
    }
};