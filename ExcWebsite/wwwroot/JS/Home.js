$(() => {
    const username = getCookie("username");
    if (username) {
        $("#header-name").html(`<h1>${username}</h1>`);
    } else {
        console.log("The username not found"); 
    }

    $("#fileInput").on("change", UploadPhoto); 
});


const UploadPhoto = (event) => {
    let file = event.target.files[0];
    console.log(file); 

    if (file) {
        let reader = new FileReader();
        reader.onload = function (e) {
            $("#user-image").attr("src", e.target.result);
        };
        reader.readAsDataURL(file); 
    }
}

const GetPhoto = async () => {
    try {
        const username = getCookie("username");
        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${username}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        const data = await response.json();

        if (response.ok) {
            console.log("Profile Picture URL:", data.picture);

            // If picture is null, use default image
            const profilePicture = data.picture ? data.picture : "../images/default-user.png";

            // Update the image source dynamically
            $("#user-image img").attr("src", profilePicture);
        } else {
            console.error("Error fetching profile picture:", response.statusText);
        }
    } catch (error) {
        console.error("An error occurred:", error);
    }
}

const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(";").shift();
    }
    return null;
};