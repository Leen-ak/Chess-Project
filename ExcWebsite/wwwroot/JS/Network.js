$(() => {
    main();
});

async function main() {
    try {
        const res = await fetch("https://localhost:7223/api/MainPage/Users", {
            method: "GET",
            credentials: "include"
        });

        const data = await res.json();
        const username = data.username;
        const userId = data.userId;
        const picture = await GetPhoto(username); //here is the default picture 
        console.log("The picture is: ", picture);
        $("#user-image").attr("src", picture);
        GetAllSuggestedFriend(userId, username, picture);
    }
    catch (error) {
        console.log(error);
    }
}

const GetAllSuggestedFriend = async (userId, username, picture) => {
    try {
        const response = await fetch(`https://localhost:7223/api/Network/GetUsers${userId}`, {
            method: "GET",
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        console.log("The data from API is: ", data); 
        if (response.ok) {
            data.forEach(user => {
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                buildTheCard(user.userId, user.username, profilePicture); 

            });
        }
        else
            console.log("NO");
    }
    catch (error) {
        console.log(error);
    }
};

const GetPhoto = async (username) => {
    try {
        const response = await fetch(`https://localhost:7223/api/User/profile-picture/${username}`, {
            method: "GET",
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        if (response.ok)
            console.log("The data from getting the profile picture api is: ", data); //here there is the right picture 
        else
            console.log("not found pic"); 

        const profilePicture = data.pictureBase64 ? `data:image/png;base64, ${data.pictureBase64}` : "../images/user.png";
        return profilePicture; 
    }
    catch (error) {
        console.log(error)
    }

};

//const buildUserCard = async (data) => {
//    const profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
//    buildTheCard(data.id, data.username, profilePicture);
//};

//cards 
function buildTheCard(userId, username, profilePicture) {
    const div = $(`
        <div class="card network-card" id="user-card-${userId}">
            <img src="${profilePicture}" alt="Chess Game Image" class="card-img-top user-image" id="user-image-${userId}" />
            <h2 class="card-title username">${username}</h2>
            <div class="button-section">
                <button class="btn" id="follow-btn-${userId}">Follow</button>
            </div>
        </div>
    `);

    div.appendTo($(".grid-container"));
    //buildFollowList(div, userId, username, profilePicture);
}

const buildFollowList = async (div, userId, username, profilePicture) => {
    div.find(`#follow-btn-${userId}`).on("click", async function () {
        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${getCookie("username")}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const userData = await response.json();
        const followData = {
            followerId: userData.id,
            followingId: userId,
            status: "Pending"
        };
        console.log("Sending follow request:", followData);

        try {
            const followResponse = await fetch('https://localhost:7223/api/Network/AddFollowing', {
                method: 'POST',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(followData)
            });

            if (followResponse.ok) {
                console.log("Follow request sent successfully");
            }
        } catch (error) {
            console.log("Error following user: ", error);
        }

        // Move the card to the modal list
        const followingItem = $(`
            <div class="following-item" id="following-${userId}">
                <img src="${profilePicture}" class="following-pic" alt="${username}" />
                <span class="following-username">${username}</span>
                <button class="btn-unfollow" data-id="${userId}">Unfollow</button>
            </div>
        `);
        $("#friend-list").append(followingItem);
        $(`#user-card-${userId}`).hide();

        let currentCount = parseInt($("#following-count").text()) || 0;
        if (currentCount >= 0) $("#friend-list").find('.no-followers-text').remove();
        currentCount++;
        $("#following-count").text(currentCount);
    });
};

const buildRequestList = async (acceptButton, rejectButton) => {
    try {
        const response = await fetch(`https://localhost:7223/api/LoginPage/username/${getCookie("username")}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const userData = await response.json();
        const statusResponse = await fetch(`https://localhost:7223/api/Network/Status/${userData.id}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        });

        if (statusResponse.ok) {
            const statusData = await statusResponse.json();
            const resultArray = statusData.result;

            if (Array.isArray(resultArray) && resultArray.length > 0) {
                resultArray.forEach(async request => {

                    const getInfo = await fetch(`https://localhost:7223/api/Network/GetUserName/${request.followerId}`, {
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });

                    const getUsername = await getInfo.json();
                    let profilePicture = getUsername.vm.picture ? `data:image/png;base64,${getUsername.vm.picture}` : "../images/user.png";

                    if (request.status === "Pending") {
                        const requestItem = $(`
                                <div class="request-item" id="request-${request.followerId}">
                                    <img src="${profilePicture}" class="request-pic" alt="${getUsername.vm.username}" />
                                    <span class="request-username">${getUsername.vm.username}</span>
                                  <button class="btn-accept" data-id="${request.id}">
                                        <svg width="24" height="24" viewBox="0 0 24 24" fill="green" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M9 16.2L4.8 12L3.4 13.4L9 19L21 7L19.6 5.6L9 16.2Z"/>
                                        </svg>
                                    </button>
                                    <button class="btn-reject" data-id="${request.id}">
                                        <svg width="24" height="24" viewBox="0 0 24 24" fill="red" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M6 6L18 18M6 18L18 6" stroke="red" stroke-width="2"/>
                                        </svg>
                                    </button>
                                </div>
                            `);
                        $("#friend-list").append(requestItem);
                    }
                });
            } else {
                console.log("No status data available.");
                $("#friend-list").html('<p class="no-followers-text">You do not have any friend requests.</p>');
            }
        }
    } catch (error) {
        console.log("An error occurred:", error);
    }
    $("#theModal").modal('show');
};
