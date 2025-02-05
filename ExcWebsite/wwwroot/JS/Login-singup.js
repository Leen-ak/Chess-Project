$(() => {
    $("#signup-btn").on("click", (event) => {
        event.preventDefault();
        setUpUserInfo(); 
        clearSignupInput();
    });

    $("#login-btn").on("click", (event) => {
        event.preventDefault(); 
        usernameLogin();
        clearLoginInput(); 
    });
})

//if they did not have an account
//sign up
const setUpUserInfo = () => {
    const userData = {
        FirstName: $("#firstname").val(),
        LastName: $("#lastname").val(),
        UserName: $("#usernameSignup").val(),
        Email: $("#email").val(),
        Password: $("#signupPassword").val(),
        ConfirmPassword: $("#passwordConfirm").val(),
    };

    //Get the API for the username to compare 
    validateUsernameUniqueness();

    //Get the API for the email to compare
    validateEmailUniqueness(); 

    //Send data to the Server
    if ($("#signup-form").valid()) {
        $.ajax({
            url: 'https://localhost:7223/api/LoginPage',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(userData), //convert js object to json
            success: (response) => {
                alert("User added successfully: " + response.msg);
                console.log("Response from the server: ", response);
            },

            error: (error) => {
                console.log("Error adding user", error.responseText);
                console.log("Error Details: ", error);
            }
        });
    }
    else 
        console.log("Validation failed. Please fix the errors and try again.");
}

const clearSignupInput = () => {
    $("#firstname").val("");
    $("#lastname").val("");
    $("#usernameSignup").val ("");
    $("#email").val("");
    $("#signupPassword").val("");
    $("#passwordConfirm").val("");
}

const clearLoginInput = () => {
    $("#username").val("");
    $("#login-password").val("");
}

const validateUsernameUniqueness = async () => {
    try {
        const username = $("#usernameSignup").val();
        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${username}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        const data = await response.json();
        console.log("The data from username api: ", data);

        if (response.ok) {
            if (data.userName === username)
                alert("The username is already exist.");
            else
                console.log("The username is not exist."); 
        }
    }
    catch (error) {
        console.log('Error: ', error);
    }
};

const validateEmailUniqueness = async () => {
    try {
        const email = $("#email").val();
        const response = await fetch(`https://localhost:7223/api/LoginPage/email/${email}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        const data = await response.json();
        console.log("The data from email api: ", data);

        if (response.ok) {
            if (data.email === email)
                alert("The email already exists.");
            else
                console.log("The email does not exist.");
        }
    }
    catch (error) {
        console.log('Error: ', error);
    }
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

//login page
const usernameLogin = async () => {
    try {
        const username = $("#username").val();
        const password = $("#login-password").val();
        console.log("The username: ", username);
        console.log("The password is: ", password);

        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${username}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        const data = await response.json();
        console.log("The data from username api: ", data);

        if (response.ok) {
            if (data.userName === username) {
                alert("The username exists in the database");

                if (data.password === password) {
                    alert("The user password matches the database password");
                    setCookie("username", username, 1);
                    window.location.href = "..//HTML/Home.html"; 
                }
                else
                    alert("Incorrect password");
            }
            else
                alert("The username does not exist");
        }
    }
    catch (error) {
        console.log('Error: ', error);
    }   
}; 

const setCookie = (name, value, days) => {
    const expires = new Date();
    expires.setTime(expires.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = `${name} = ${value}; expires = ${expires.toUTCString()}; path=/`;
}


//TO DO
//1. the password visibility does not go off after the signup or login
//2. The eamil authonication
//3. forget password
//4. hashing for the password in database
