$(() => {
    $("#signup-btn").on("click", (event) => {
        event.preventDefault();
        setUpUserInfo(); 
    });

})

//if they did not have an account
//sign up
const setUpUserInfo = () => {
    const userData = {
        FirstName: $("#firstname").val(),
        LastName: $("#lastname").val(),
        UserName: $("#username-signup").val(),
        Email: $("#email").val(),
        Password: $("#signup-password").val()
        //ConfirmPassword: $("#password-confirm").val()
    }; 

    //Checking the strength of the password 
    //passwordCheck();

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
                alert("Error adding user: " + error.responseText);
                console.log("Error Details: ", error);
            }
        });
    }
    else 
        console.log("Validation failed. Please fix the errors and try again.");
    

}

const validateUsernameUniqueness = async () => {
    try {
        const username = $("#username-signup").val();
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
                console.log("The username is already exist.");
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
                console.log("The email already exists.");
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
    const specialChars = /[!@#$%^&*_-]/.test(password);

    if (passwordLength <= 8)
        return false;
    else if (passwordLength > 8 && passwordLength <= 12)
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
        passwordConfirm: { maxlength: 64, required: true, equalTo: "#signup-password" }
    },
    messages: {
        firstname: "Firstname is required", 
        lastname: "Lastname is required",
        usernameSignup: "Username is required",
        email: "Email is required",
        signupPassword: "Should be more than 8 chars",
        passwordConfirm: "Passwords do not match",
    }
});