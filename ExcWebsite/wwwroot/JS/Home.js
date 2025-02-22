$(() => {
    const username = getUsernameFromToken();

    if (username) {
        $("#header-name").html(`<h1>${username}</h1>`);
        GetPhoto(username);
    } else {
        console.log("The username not found");
    }
    $("#fileInput").on("change", UploadPhoto); 
});

const getUsernameFromToken = () => {
    const token = localStorage.getItem("token");
    if (!token) {
        console.warn("No token found in localStorage.");
        return null;
    }

    try {
        const base64Url = token.split('.')[1];
        if (!base64Url) {
            console.error("Invalid JWT format.");
            return null;
        }

        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = atob(base64);
        const decodedData = JSON.parse(jsonPayload);

        const usernameKey = Object.keys(decodedData).find(key => key.includes("name"));
        if (!usernameKey) {
            console.warn("The 'name' field is missing in JWT.");
            return null;
        }

        console.log("Extracted username:", decodedData[usernameKey]);
        return decodedData[usernameKey];
    } catch (error) {
        console.error("Error decoding JWT:", error);
        return null;
    }
};

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/update-picture/profile-picture/${username}`, {
            method: 'GET'
        });

        console.log("Response Status:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("Parsed Data:", data);

            let profilePicture = data.pictureBase64 ? `data:image/png;base64,${data.pictureBase64}` : "../images/user.png";
            $("#user-image").attr("src", profilePicture);
        } else {
            console.warn("No profile picture found, using default image.");
            $("#user-image").attr("src", "../images/user.png");
        }
    } catch (error) {
        console.error("An error occurred while fetching profile picture:", error);
    }
};

const UploadPhoto = async (event) => {
    const file = event.target.files[0];
    const username = getUsernameFromToken(); 

    if (!file || !username) {
        console.error("No file selected or username missing.");
        return;
    }

    let formData = new FormData();
    formData.append("file", file);
    formData.append("username", username);

    try {
        const response = await fetch("https://localhost:7223/update-picture/update-picture", {
            method: "PUT",
            body: formData, 
        });

        const data = await response.json();

        if (response.ok) {
            alert("Profile picture updated!");
            GetPhoto(username);
        } else {
            console.error("Error updating profile picture:", data);
        }
    } catch (error) {
        console.error("Network error while updating profile picture:", error);
    }
};



//TO DO
//1. User adding the picture in js is not working but it warks with swagger

//what has been done after making the code more secure
//1. Getting the usernmae from JWT locaStorage instead of setting and getting the username from cookies
//2. Getting the photo is working fine 
