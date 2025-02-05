$(() => {
    const username = getCookie("username");
    getPhotot(username); 
    GetUsernames();
    $("#following-btn").on("click", function () { 
        $("#theModal").modal('show'); 
    }); 
});

//adding friends 
const followingCount = async () => {
    try {
        const response = await fetch('https://localhost:7223/api/Network', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }
    catch (erro) {
        console.log(error); 
    }
}

const GetUsernames = async () => {
    const username = getCookie("username");
    try {
        const response = await fetch('https://localhost:7223/api/Network', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        let profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
        //$("#user-image").attr("src", profilePicture)
        //data.forEach(user => console.log("Username:", user.username));
        //data.forEach(user => console.log("Picture:", user.picture));

        if (response.ok) {
            data.forEach(user => {
                if (username === user.username)
                    return;
                buildUserCard(user)
            });
        }        
    }
    catch (error) {
        console.log(error); 
    }
};

const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(";").shift();
    }
    return null;

};

const getPhotot = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/update-picture/username/${username}`, {
            method: 'GET'
        });

        console.log("Response Status:", response.status); 

        if (response.ok) {
            const data = await response.json();
            let profilePicture = data.picture ? `data:image/png;base64, ${data.picture}` : "../images/user.png";
            $("#user-image").attr("src", profilePicture);
        }
    }
    catch (error) {
        console.log("An error occured", error);
    }
}

const buildUserCard = async (data) => {
    const profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
    const username = getCookie("username");

    const div = $(`
        <div class="card network-card" id="user-card-${data.id}">
            <img src="${profilePicture}" alt="Chess Game Image" class="card-img-top user-image" id="user-image-${data.id}" />
            <h2 class="card-title username">${data.username}</h2>
            <div class="button-section">
                <button class="btn" id="follow-btn-${data.id}">Follow</button>
            </div>
        </div>
    `);

    try {
        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${username}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const userData = await response.json(); 

        div.find(`#follow-btn-${data.id}`).on("click", function () {
            console.log("Follow button clicked:", data.username);
            console.log("The user ID is:", data.id);
            console.log("The picture is:", data.picture);
            console.log(`The followerID is: ${userData.id} and the followingId is ${data.id}`);
            //userData.id is me the follower and the data.id is the other user that i want to follow

            //remve the div card that the user follow 
            $(`#user-card-${data.id}`).fadeOut(300, function () {
                $(this).appendTo("#friend-list").fadeIn(300);
            });

            let currentCount = parseInt($("following-count").text()) || 0;
            currentCount++;
            console.log(currentCount);
            $("#following-count").text(currentCount);
        });

        div.appendTo($(".grid-container"));

    } catch (error) {
        console.log(error);
    }
};


