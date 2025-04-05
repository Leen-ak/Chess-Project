$(() => {
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
        const response = await fetch('https://localhost:7223/api/MainPage/signup', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });

        const data = await response.json();

        if (!response.ok) {
            showPopup("Error", "Invalid user Information", "error");
            return;
        }
        showPopup("Success", "User registered successfully!", "success");
    } catch (error) {
        console.log("Error signing up:", error);
    }
};

const loginUser = async () => {
    const userData = {
        UserName: $("#username").val(),
        Password: $("#login-password").val(),
    };

    try {
        const response = await fetch('https://localhost:7223/api/MainPage/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: "include", 
            body: JSON.stringify(userData)
        });

        const data = await response.json();
        if (response.ok) {
            showPopup("Success", "Login Successful!", "success").then(() => {
                window.location.href = "../HTML/Home.html";
            });
        } else {
            showPopup("Error", "Invalid Credential", "error")
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
            required: "   Email is required",
            maxlength: "   Email cannot be more than 64 characters",
            email: "    Please enter a valid email"
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
//3. forget password - DONE 
//4. hashing for the password in database - DONE
//TO FIX
//4. the messages is not showing good on the input filed here and for reset password

//What is working after the changes
//1. Sign-up working for both front and back end (after i changed the code to be more secure)
//2. Login is working + the JWT for sending the toking to a localStoarge


