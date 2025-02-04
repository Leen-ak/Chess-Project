$(() => {
    const username = getCookie("username");
    if (username) {
        $("#header-name").html(`<h1>${username}</h1>`);
            GetPhoto();
    } else {
        console.log("The username not found");
    }
    $("#fileInput").on("change", UploadPhoto);

});

const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(";").shift();
    }
    return null;
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

const GetPhoto = async () => {
    try {
        const username = getCookie("username");
        const response = await fetch(`https://localhost:7223/update-picture/username/${username}`, {
            method: 'GET'
        });

        console.log("Response Status:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("Parsed Data:", data);
            let profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/default-user.png";
            $("#user-image").attr("src", profilePicture);
        } else {
            console.log("Server Error:", response.status);
        }
    } catch (error) {
        console.error("An error occurred:", error);
    }
};