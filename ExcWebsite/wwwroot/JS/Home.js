$(() => {
    const username = getUsernameFromToken();

    if (username) {
        $("#header-name").html(`<h1>${username}</h1>`);
        GetPhoto(username);
    } else {
        console.log("The username not found");
    }
});

//Get JWT FROM HttpOnly Cookies 
const getTokenFromCookies = () => {
    return document.cookie //return the cookies as a single string
        .split('; ') //converts the string into an array
        .find(row => row.startsWith('AuthToken=')) //find the key that i created in the controller 
        ?.split('=')[1]; //extracts only the token value 
};

//Extract username from JWT (from cookie)
const getUsernameFromToken = () => {
    const token = getTokenFromCookies(); 
    if (!token) {
        console.log("No token found in cookies");
        return; 
    }

    try {
        const base64Url = token.split('.')[1]; //get payload
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = atob(base64);
        const decodedData = JSON.parse(jsonPayload);
        const usernameKey = Object.keys(decodedData).find(key => key.includes("name"));
        if (!usernameKey) {
            console.log("The name field is missing in JWT.");
            return;
        }
        console.log("The username is: ", decodedData[usernameKey]);
        return decodedData[usernameKey];
    }
    catch (error) {
        console.error("Error decoding JWT: ", error);
        return; 
    }
}

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/update-picture/profile-picture/${username}`, {
            method: 'GET',
            credentials: "include" //to make sure the cookies are sent with the request
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

    console.log("Selected File:", file); 
    console.log("Extracted Username:", username);

    if (!file) {
        console.error("No file selected");
        return;
    }

    if (!username) {
        console.error("Username is missing");
        return;
    }

    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = async () => {
        const base64Image = reader.result.split(",")[1]; 

        const payload = {
            username: username,
            pictureBase64: base64Image
        };

        console.log("Sending Payload:", JSON.stringify(payload)); 

        try {
            const response = await fetch("https://localhost:7223/update-picture/update-picture", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error("Error updating profile picture:", errorText);
                return;
            }

            const data = await response.json();
            console.log("Response:", data);

            alert("Profile picture updated");
            GetPhoto(username);
        } catch (error) {
            console.error("Network error while updating profile picture:", error);
        }
    };
};
document.getElementById("fileInput").addEventListener("change", UploadPhoto);

//TO DO
//1. User adding the picture in js is not working but it warks with swagger

//what has been done after making the code more secure
//1. Getting the usernmae from JWT locaStorage instead of setting and getting the username from cookies
//2. Getting the photo is working fine 
//3. The fucking phowo is uploading IN THE STUPID JS AFTER 10000000000 TRY 
