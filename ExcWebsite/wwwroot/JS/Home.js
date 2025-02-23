$(() => {
    getUsernameFromServer();
    $("#logout").on("click", () => {
        logoutUser();
    });
});

const getUsernameFromServer = async () => {
    try {
        const response = await fetch("https://localhost:7223/update-picture/profile", {
            method: "GET",
            credentials: "include"  
        });

        if (!response.ok) {
            console.error("Failed to fetch user profile.");
            return;
        }

        const data = await response.json();
        console.log("Extracted Username:", data.username);

        if (data.username) {
            $("#header-name").html(`<h1>${data.username}</h1>`);
            GetPhoto(data.username);
        }
    } catch (error) {
        console.error("Error fetching user profile:", error);
    }
};

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/update-picture/profile-picture/${username}`, {
            method: 'GET',
            credentials: "include" 
        });

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

const logoutUser = async () => {
    await fetch("https://localhost:7223/logout", {
        method: "POST",
        credentials: "include"
    });

    alert("Logged out successfully!");
    window.location.href = "../HTML/mainPage.html";
};
