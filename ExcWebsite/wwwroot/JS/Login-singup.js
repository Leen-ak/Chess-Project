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
    passwordCheck();

    //Get the API for the username to compare 
    validateUsernameUniqueness();

    //Get the API for the email to compare
    validateEmailUniqueness(); 

    //Send data to the Server
    if (passwordCheck()) {
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
}


//password vildation - weak, meduim, strong
const passwordCheck = () => {
    let weak = false;
    let medium = false
    let strong = false;
    const password = $("#signup-password").val();
    const passwordLength = password.length;
    const isNumber = /\d/.test(password); //if nummber
    const isLetter = /[a-z]/.test(password); //if letter
    const isUpperCase = /[A-Z]/.test(password); //if uppercase
    const specialChars = /[!@#$%^&*_-]/.test(password);   

    if (passwordLength <= 8) {
        console.log("Password should be greater than 8 Characters");

        if (isNumber || isLetter || isUpperCase || specialChars) {
            console.log("one is true, so it is a weak password");
            weak = true;
            return false; 
        }
    }
    else if (passwordLength > 8 && passwordLength <= 12) {
        if ((isNumber || isLetter) && (isUpperCase || specialChars)) {
            console.log("Some is true, so it is a meduim password");
            medium = true;
            return true; 
        }    
    }
    else if (passwordLength > 8) {
        if (isNumber && isLetter && isUpperCase && specialChars) {
            console.log("all consitions is true so it is a strong password");
            strong = true;
            return true
        }
    }
    else {
        console.log("Aother");
        return false; 
    }
};

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
