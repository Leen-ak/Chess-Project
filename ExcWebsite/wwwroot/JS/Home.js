$(() => {
    getUsernameFromServer();
    $("#logout").on("click", () => {
        logoutUser();
    });
});

const getUsernameFromServer = async () => {
    try {
        const response = await fetch("https://localhost:7223/api/User/profile", {
            method: "GET",
            credentials: "include"  
        });

        if (!response.ok) {
            console.error("Failed to fetch user profile.");
            return;
        }

        const data = await response.json();
        if (data.username) {
            $("#header-name").html(`<h1>${data.username}</h1>`);
            GetPhoto(data.username);
        }
        return data.username; 
    } catch (error) {
        console.error("Error fetching user profile:", error);
    }
};

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/api/User/profile-picture/${username}`, {
            method: 'GET',
            credentials: "include" 
        });

        if (response.ok) {
            const data = await response.json();
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

const logoutUser = async () => {
    await fetch("https://localhost:7223/api/MainPage/logout", {
        method: "POST",
        credentials: "include"
    });

    alert("Logged out successfully!");
    window.location.href = "../HTML/mainPage.html";
};

const UploadPhoto = async (event) => {
    const file = event.target.files[0];

    const username = await getUsernameFromServer(); 
    console.log("Selected File:", file);
    console.log("Extracted Username:", username);

    if (!file) {
        console.error("No file selected");
        return;
    }

    if (!username) {
        console.error("Username is missing from uploading the photo");
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

        try {
            const response = await fetch("https://localhost:7223/api/User/update-picture", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(payload)
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

//NOTE: for tomorrow i should start doing the logout 404 error

//what has been done after making the code more secure
//1. Getting the usernmae from JWT locaStorage instead of setting and getting the username from cookies
//2. Getting the photo is working fine
//3. The fucking phowo is uploading IN THE STUPID JS AFTER 10000000000 TRY 