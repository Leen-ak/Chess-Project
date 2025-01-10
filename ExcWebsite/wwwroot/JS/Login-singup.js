$(() => {

})

//so i have
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

const setUpUserInfo = () => {
    const name = $("#firstname").val();
    const lastname = $("#lastname").val();
    const username = $("#username-signup").val();
    const email = $("#email").val();
    const password = $("#password").val();
    const passwordConf = $("#password-confirm").val();
}