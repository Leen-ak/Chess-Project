$(() => {
    console.log("Testing..."); 
    $("#signup-btn").on("click", (event) => {
        event.preventDefault();
        console.log("Testing2..."); 
        setUpUserInfo(); 
        console.log("After the test.");
    });

})

const setUpUserInfo = () => {
    const userData = {
        FirstName: $("#firstname").val(),
        LastName: $("#lastname").val(),
        UserName: $("#username-signup").val(),
        Email: $("#email").val(),
        Password: $("#signup-password").val()
        //ConfirmPassword: $("#password-confirm").val()
    }; 
    console.log("User info is: ", userData); 

    //sessionStorage.setItem("userData", userData);
    //console.log("The data after setItem to the sessionStorage: ", userData); 
    //const storedData = JSON.parse(sessionStorage.getItem("userData"));
    //console.log("StoredData in sessionStorage is: ", storedData);
    //alert("User data saved to sessionStorage"); 

    //Send data to to the API 
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

//if they have an account
//login
//i will take the text info
//compare to database
//if it matach the user name and password
//then move to the next page

//if they did not have an account
//sign up
//take the info
//check if ther is a match in database for -> email!
//if it exsite your eamil is already exist
// if it is not
// add the info to the database