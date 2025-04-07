$(() => {
    main();
    $("#following-btn").on("click", function () {
        $("#theModal .modal-title").text("Following");
        FollowStatus();
        let currentCount = parseInt($("#following-count").text()) || 0;
        if (currentCount === 0)
            $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
        $("#theModal").modal('show');
    });
});

async function main() {
    try {
        //Getting all user those are not with Accepted or Pending status. 
        const res = await fetch("https://localhost:7223/api/MainPage/Users", {
            method: "GET",
            credentials: "include"
        });

        const data = await res.json();
        const username = data.username;
        const userId = data.userId;
        const picture = await GetPhoto(username); //here is the default picture 
        $("#user-image").attr("src", picture);
        GetAllSuggestedFriend(userId);
    }
    catch (error) {
        console.log(error);
    }
}

function buildFollowingList(userId, username, profilePicture) {
    const followingItem = $(`
        <div class="following-item" id="following-${userId}">
            <img src="${profilePicture}" class="following-pic" alt="${username}" />
            <span class="following-username">${username}</span>
            <button class="btn-unfollow" data-id="${userId}">Unfollow</button>
        </div>
    `);

    $("#friend-list").append(followingItem);
}

const GetAllSuggestedFriend = async (userId) => {
    try {
        const response = await fetch(`https://localhost:7223/api/Network/GetUsers${userId}`, {
            method: "GET",
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        console.log("The data from API is: ", data);
        const followButton = `<button class="btn follow-btn" id="follow-btn-${userId}">Follow</button>`;

        if (response.ok) {
            data.forEach(user => {
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                const card = buildUserCard(".grid-container", user.id, user.username, profilePicture, followButton, "card network-card");
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
        if (response.ok) {
            const profilePicture = data.pictureBase64 ? `data:image/png;base64, ${data.pictureBase64}` : "../images/user.png";
            return profilePicture;
        }
        else
            console.log("not found pic");
    }
    catch (error) {
        console.log(error)
    }
};


const AddUser = async (userId) => {
    try {
        console.log("The user id from addUser API is: ", userId);
        const response = await fetch(`https://localhost:7223/api/Network/FollowRequest`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({ followingId: userId })

        });
        const data = await response.json();
        if (response.ok) {
            console.log("The data from followRequest API IS: ", data); 
        }
    }
    catch (error) {
        console.log(error); 
    }
}

const FollowStatus = async () => {
    try {
        const response = await fetch(`https://localhost:7223/api/Network/Status`, {
            method: "GET",
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        if (response.ok) {
            console.log("The data from GetFollowStatus is: ", data);

            const acceptedRequests = data.acceptedRequests;
            const pendingRequests = data.pendingRequests;
            pendingRequests.forEach(async request => {
                const userId = request.followingId;
                const infoResponse = await fetch(`https://localhost:7223/api/Network/GetUsers${userId}`, {
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                const userData = await infoResponse.json();
                const profilePicture = userData.pictureBase64 ? `data:image/png;base64, ${userData.pictureBase64}` : "../images/user.png";
                buildFollowingList(userData.id, userData.username, profilePicture);

            })
        }
    }
    catch (error) {
        console.log(error); 
    }
}

function buildUserCard(container, userId, username, profilePicture, button, customClass = "") {
    const card = $(`
        <div class="${customClass}" id="user-card-${userId}">
            <img src="${profilePicture}" alt="User Image" class="card-img-top user-image" />
            <h2 class="card-title username">${username}</h2>
            <div class="button-section">
                ${button}
            </div>
        </div>
    `);

    $(container).append(card);
    return card;
}

//when the user press follow button it will call the API sendrequest, call API that is counting for accepting and pending status
//press follow call API (AddUser)


