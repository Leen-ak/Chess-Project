$(() => {
    $("#submit-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const email = $("#email").val().trim();
        if (!validateEmail(email)) {
            showPopup("Error", "Invalid Email Format. Please Enter a Valid Email", "error")
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

const showPopup = (title, message, icon = "info") => {
    return Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: "Ok",
        background: "#232323",
        color: "#bbb4aa",
        customClass: {
            confirmButton: "custom-swal-button",
        }
    });
};

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
            showPopup("Error", "Error Sending Email! Try Again", "error");
        } else {
            showPopup("Success", "Reset Password Email Has Been Sent Successfully", "success");
        }
    } catch (error) {
        console.error("Error:", error);
        showPopup("Error", "An Unexpected Error Occurred While Sending The Email. Please Try Again", "error");
    }
};

const extractTokenFromURL = async () => {
    const urlParams = new URLSearchParams(window.location.search);     //window.location.search -> extract everything after ? marks 
    const token = urlParams.get("token");                              //URLSearchParams convert it into an object 
                                                                       //it will retrieves the value of token from the query string
    if (!token) {
        showPopup("Error", "Invalid Reset Link. Please Request A New One By Entering An Email Again", "error").then(() => {
            window.location.href = "mainPage.html";
        });
        return null;
    }
    return token;
};

const resetPasswordHandler = async () => {
    const password = $("#newPassword").val().trim();
    const confirmPassword = $("#confirmPassword").val().trim();
    const token = await extractTokenFromURL();

    if (!token) {
        showPopup("Error", "Invalid Reset Link. Please Try Again", "error").then(() => {
            window.location.href = "mainPage.html";
        });
        return;
    }

    if (!password || !confirmPassword) {
        showPopup("Error", "Both Password Fields Are Required", "error");
        return;
    }

    if (password !== confirmPassword) {
        showPopup("Error", "Passwords Do Not Match", "error"); 
        return;
    }

    try {
        console.log("Resetting password with token:", token);
        const resetPasswordAPI = await fetch('https://localhost:7223/api/Password/ResetPassword', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ NewPassword: password, ConfirmPassword: confirmPassword, Token: token })
        });

        const resetPassword = await resetPasswordAPI.json();
        if (resetPasswordAPI.ok) {
            showPopup("Success", "Password Has Been Changed Successfully", "success").then(() => {
                window.location.href = "mainPage.html";
            });
        } else {
            showPopup("Error", "Password Reset Failed", "error"); 
        }
    } catch (error) {
        console.error("Error resetting password:", error);
        showPopup("Error", "An Error Occurred While Resetting The Password", "error");
    }
}

const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
};

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

if (window.location.pathname.includes("ResetPassword.html")) {
    extractTokenFromURL();
}
//TO FIX
//1. the messages is not showing good on the input filed here and fot the signup page
//2. autofill change the background color so i turned it off for now
