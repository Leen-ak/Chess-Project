$(() => {
    $("#signup-btn").on("click", () => {
        setUpUserInfo(); 
    });

})

const setUpUserInfo = () => {
    const userData = {
        FirstName: $("firstname").val(),
        LastName: $("#lastname").val(),
        UserName: $("#username-signup").val(),
        Email: $("#email").val(),
        Password: $("#password").val(),
        ConfirmPassword: $("#password-confirm").val()
    }; 

    //Send data to to the API 
    $.ajax({
        url: 'api/LoginPage',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(userData), //convert js object to json
        success: (response) => {
            alert("User added successfully: " + response.msg);
            console.log(response); 
        },

        error: (error) => {
            alert("Error adding user: " + error.responseText);
            console.log(error); 
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