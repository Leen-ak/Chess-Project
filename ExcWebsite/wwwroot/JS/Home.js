$(() => {
    const username = getCookie("username");
    if (username) {
        $("#header-name").html(`<h1>${username}</h1>`);
    } else {
        console.log("The username not found"); 
    }
    $("#fileInput").on("change", UploadPhoto); 

    GetPhoto();
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
    console.log(file); 

    if (file) {
        let reader = new FileReader();
        reader.onload = function (e) {
            $("#user-image").attr("src", e.target.result);
        };
        reader.readAsDataURL(file); 

        //Create FormData to send the file to the server
        let formData = new FormData();
        formData.append("username", "testUser");
        formData.append("file", file); 

        $.ajax({
            url: 'https://localhost:7223/api/update-picture',
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
        const response = await fetch(`https://localhost:7223/api/LoginPage/${username}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        const data = await response.json();
        if (response.ok) {
            console.log("profile picture: ", data.picture);

            let profilePicture;

            if (data.picture) {
                profilePicture = `data:image/png;base46,${data.picture}`;
            }
            else {
                console.log("default user picture");
            }

            $("#user-image").attr("src", profilePicture);
        }
        else {
            console.log("Error fetching profile picture", response.status);
        }
    }
    catch (error) {
        console.log("An rror occurred: ", error);
    }
}