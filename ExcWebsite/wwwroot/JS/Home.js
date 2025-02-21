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

        // ✅ Find the correct key for username
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

const UploadPhoto = (event) => {
    let file = event.target.files[0];
    const username = getCookie("username");

    if (!username) {
        console.error("Username is missing from cookies.");
        return;
    }

    if (file) {
        let reader = new FileReader();
        reader.onload = function (e) {
            $("#user-image").attr("src", e.target.result);
        };
        reader.readAsDataURL(file);

        //Create FormData to send the file to the server
        let formData = new FormData();
        formData.append("file", file);
        formData.append("username", username);
       

        $.ajax({
            url: 'https://localhost:7223/update-picture',
            method: 'PUT',
            processData: false,
            contentType: false,
            data: formData,
            success: (response) => {
                alert("The picture uploaded successfully to the database " + response.msg);
                console.log("Response from the server ", response);
            },
            error: (error) => {
                console.log("Error adding user ", error.responseText);
                console.log("Error Details: ", error);
            }
        });
    }
}

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/update-picture/username/${username}`, {
            method: 'GET'
        });

        console.log("Response Status:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("Parsed Data:", data);
            let profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
            $("#user-image").attr("src", profilePicture);
        } else {
            console.log("Server Error:", response.status);
        }
    } catch (error) {
        console.error("An error occurred:", error);
    }
};

//TO DO
//1. User adding the picture in js is not working but it warks with swagger

//what has been done after making the code more secure
//1. Getting the usernmae from JWT locaStorage instead of setting and getting the username from cookies
