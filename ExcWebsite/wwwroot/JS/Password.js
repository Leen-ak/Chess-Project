$(() => {


    $("#submit-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const email = $("#email").val().trim();
        if (!validateEmail(email)) {
            alert("Invalid email format. Please enter a vaild email.");
            return; 

        }

        await SendEmail(email);
    });

    $("#password-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();
        resetPasswordHandler();
    });
});

const SendEmail = async (email) => {

    try {
        console.log("From getUserId: ", email);
        const response = await fetch('https://localhost:7223/api/Password/VerifyEmail', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email })
        });

        if (!response.ok)
            throw new Error("Email verification failed."); 

        const data = await response.json();
        const responseEmail = await fetch(`https://localhost:7223/api/Password/Send-Email`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: data.userId, Email: email })
        });

        const dataEmail = await responseEmail.json();
        if (!responseEmail.ok) {
            alert(dataEmail.msg || "Error sending email.");
        } else {
            alert("Reset password email has been sent successfully!");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An unexpected error occurred while sending the email. Please try again.");
    }
};

const extractTokenFromURL = async () => {
    const urlParams = new URLSearchParams(window.location.search);     //window.location.search -> extract everything after ? marks 
    const token = urlParams.get("token");                              //URLSearchParams convert it into an object 
                                                                       //it will retrieves the value of token from the query string
    if (!token) {
        console.log("Invalid token");
        alert("Invalid reset link. Please request a new one.");
        window.location.href = "mainPage.html";
        return null;
    }
    return token; 
};

const resetPasswordHandler = async () => {
    const password = $("#newPassword").val().trim();
    const confirmPassword = $("#confirmPassword").val().trim();
    const token = await extractTokenFromURL();

    if (!token) {
        alert("Invalid reset link. Please Try agian");
        window.location.href = "mainPage.html"; 
        return;
    }

    if (!password || !confirmPassword) {
        alert("Both password fields are required.");
        return;
    }

    if (password !== confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    try {
        console.log("Resetting password with token:", token);
        const resetPasswordAPI = await fetch('https://localhost:7223/api/Password/ResetPassword', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ NewPassword: password, Token: token })
        });

        const resetPassword = await resetPasswordAPI.json();
        if (resetPasswordAPI.ok) {
            alert("Password has been changed successfully!");
            window.location.href = "mainPage.html"; 
        } else {
            alert(resetPassword.msg || "Password reset failed.");
        }
    } catch (error) {
        console.error("Error resetting password:", error);
        alert("An error occurred while resetting the password.");
    }
}

const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
};


if (window.location.pathname.includes("ResetPassword.html")) {
    extractTokenFromURL();
}

$.validator.addMethod("passwordCheck", function (password, element) {
    const passwordLength = password.length;
    const isNumber = /\d/.test(password);
    const isLetter = /[a-z]/.test(password);
    const isUpperCase = /[A-Z]/.test(password);
    const specialChars = /[!@#$%^&*_\.\-]/.test(password);

    if (passwordLength < 8)
        return false;
    else if (passwordLength >= 8 && passwordLength <= 12)
        return (isNumber || isLetter) && (isUpperCase || specialChars);
    else if (passwordLength > 12)
        return (isNumber && isLetter && isUpperCase && specialChars);
    return false;
});

$("#login-form").validate({
    rules: {
        email: { maxlength: 64, required: true, email: true },
        newPassword: { maxlength: 64, required: true, passwordCheck: true },
        confirmPassword: { maxlength: 64, required: true, equalTo: "#newPassword" }
    },

    messages: {
        email: {
            required: "Email is required",
            maxlength: "Email cannot be more than 64 characters",
            email: "Please enter a valid email"
        },
        newPassword: {
            required: "Password should be more than 8 chars",
            maxlength: "Password cannot be more than 64 characters",
            passwordCheck: "Password should be more than 8 length and contains spical characters"
        },
        confirmPassword: {
            required: "Passwords do not match",
            maxlength: "Passwords must match",
            equalTo: "Passwords do not match"
        }
    }
});

const togglePasswordVisibility = (inputSelector, toggleButton) => {
    const password = $(inputSelector);
    const passwordType = password.attr("type");

    if (passwordType === "password") {
        password.attr("type", "text");
        $(toggleButton).text("visibility_off");
    } else {
        password.attr("type", "password");
        $(toggleButton).text("visibility");
    }
};

$("#toggle-password").on("click", function () { togglePasswordVisibility("#newPassword", this) });
$("#toggle-confirmPassword").on("click", function () { togglePasswordVisibility("#confirmPassword", this) });

//TO FIX
//1. the messages is not showing good on the input filed here and fot the signup page
