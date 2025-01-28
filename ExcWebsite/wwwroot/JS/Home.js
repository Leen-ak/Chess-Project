$(() => {
    const username = getCookie("username");
    if (username) {
        $("#header-name").html(`<h2>${username}</h2>`);
    } else {
        console.log("The username not found"); 
    }
});

const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(";").shift();
    }
    return null;
};