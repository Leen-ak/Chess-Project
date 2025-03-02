﻿$(() => {
    $("#signup-btn").on("click", (event) => {
        event.preventDefault();
        signUpUser(); 
        clearSignupInput();
    });

    $("#login-btn").on("click", (event) => {
        event.preventDefault(); 
        loginUser(); 
        clearLoginInput(); 
    });
})

const signUpUser = async () => {
    const userData = {
        FirstName: $("#firstname").val(),
        LastName: $("#lastname").val(),
        UserName: $("#usernameSignup").val(),
        Email: $("#email").val(),
        Password: $("#signupPassword").val(),
        PasswordConfiguration: $("#passwordConfirm").val()
    };

    try {
        const response = await fetch('https://localhost:7223/api/LoginPage/signup', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });

        const data = await response.json();

        if (!response.ok) {
            alert(data.msg || "Invalid user Information.");
            return;
        }

        alert("User added"); 
    } catch (error) {
        console.log("Error signing up:", error);
    }
};

const loginUser = async () => {
    const userData = {
        UserName: $("#username").val(),
        Password: $("#login-password").val(),
    };

    console.log("Sending login request with: ", userData);
    try {
        const response = await fetch('https://localhost:7223/api/LoginPage/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: "include", 
            body: JSON.stringify(userData)
        });

        const data = await response.json();
        console.log("Login response:", data);

        if (response.ok) {
            alert("Login successful");
            window.location.href = "../HTML/Home.html";
        } else {
            alert(data.msg || "Invalid Credential");
        }
    } catch (error) {
        console.log("Error logging in:", error);
    }
};

const clearSignupInput = () => {
    $("#firstname").val("");
    $("#lastname").val("");
    $("#usernameSignup").val("");
    $("#email").val("");
    $("#signupPassword").val("");
    $("#passwordConfirm").val("");
}

const clearLoginInput = () => {
    $("#username").val("");
    $("#login-password").val("");
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

$("#signup-form").validate({
    rules: {
        firstname: { maxlength: 30, required: true },
        lastname: { maxlength: 30, required: true },
        usernameSignup: { maxlength: 30, required: true },
        email: { maxlength: 64, required: true, email: true },
        signupPassword: { maxlength: 64, required: true, passwordCheck: true },
        passwordConfirm: { maxlength: 64, required: true, equalTo: "#signupPassword" }
    },
    messages: {
        firstname: {
            required: "Firstname is required",
            maxlength: "Firstname cannot be more than 30 characters"
        },
        lastname: {
            required: "Lastname is required",
            maxlength: "Lastname cannot be more than 30 characters"
        },
        usernameSignup: {
            required: "Username is required",
            maxlength: "Username cannot be more than 30 characters"
        },
        email: {
            required: "Email is required",
            maxlength: "Email cannot be more than 64 characters",
            email: "Please enter a valid email"
        },
        signupPassword: {
            required: "Password should be more than 8 chars",
            maxlength: "Password cannot be more than 64 characters",
            passwordCheck: "Password should be more than 8 chars"
        },
        passwordConfirm: {
            required: "Passwords do not match",
            maxlength: "Passwords do not match",
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
        console.log("it is here"); 
        password.attr("type", "password");
        $(toggleButton).text("visibility");
    }
};

$("#toggle-password").on("click", function () { togglePasswordVisibility("#signupPassword", this) });
$("#toggle-confirmPassword").on("click", function () { togglePasswordVisibility("#passwordConfirm", this) });
$("#password-icon").on("click", function () { togglePasswordVisibility("#login-password", this) });

//TO DO
//1. the password visibility does not go off after the signup or login
//2. The eamil authonication
//3. forget password
//4. hashing for the password in database

//What is working after the changes
//1. Sign-up working for both front and back end (after i changed the code to be more secure)
//2. Login is working + the JWT for sending the toking to a localStoarge 
