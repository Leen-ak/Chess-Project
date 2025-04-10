$(() => {
    main();
    $("#following-btn").on("click", async function () {
        $("#theModal .modal-title").text("Following");

        // Clear old list first to avoid duplicates
        $("#friend-list").empty();



        //Fetching the status and just the pending ones for the pendingRequests like i want
        try {
            const response = await fetch(`https://localhost:7223/api/Network/Status`, {
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.ok) {
                const data = await response.json();
                console.log("The data from list is: ", data); 
                const pendingRequests = data.pendingRequests;

                if (pendingRequests.length === 0) {
                    $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
                } else {
                    for (const request of pendingRequests) {
                        const userResponse = await fetch(`https://localhost:7223/api/Network/GetUsers${request.followerId}`, {
                            method: "GET",
                            headers: { 'Content-Type': 'application/json' }
                        });

                        //Here is giving undifind but there is data in the userData i cann see it 
                        const userData = await userResponse.json();
                        const user = userData[0];
                        console.log("Fetching the user information to display to the list: ", user);

                        //but from here all the data pring as undefined 
                        const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                        console.log("The picture is: ", profilePicture);
                        console.log("user name is: ", user.username);


                        const followingItem = $(`
                        <div class="following-item" id="following-${user.id}">
                            <img src="${profilePicture}" class="following-pic" alt="${user.username}" />
                            <span class="following-username">${user.username}</span>
                            <button class="btn-unfollow" data-id="${user.id}">Unfollow</button>
                        </div>
                    `);
                        $("#friend-list").append(followingItem);
                    }
                }
            }
        } catch (error) {
            console.log("Error loading following list: ", error);
        }

        $("#theModal").modal('show');
    });

});

$(document).on("click", ".follow-btn", async function () {
    const button = $(this);
    const userId = button.attr("id").split("-")[2];
    const username = button.data("username");
    const profilePicture = button.data("picture");

    console.log("The userId from clicking the follow button is: ", userId);
    console.log("The username from clicking the username is: ", username);
    console.log("The profilepicture is: ", profilePicture);

    await AddUser(userId);

    const followingItem = $(`
        <div class="following-item" id="following-${userId}">
            <img src="${profilePicture}" class="following-pic" alt="${username}" />
            <span class="following-username">${username}</span>
            <button class="btn-unfollow" data-id="${userId}">Unfollow</button>
        </div>
    `);

    $("#friend-list").append(followingItem);
    $(`#user-card-${userId}`).remove();
    console.log("user moved to following list");
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

        if (response.ok) {
            data.forEach(user => {
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                const card = buildUserCard(".grid-container", user.id, user.username, profilePicture, "card network-card");
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

//username is good in console.log, id is good, but the picture is null.
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
                const userId = request.followerId;
                const infoResponse = await fetch(`https://localhost:7223/api/Network/GetUsers${userId}`, {
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                const userData = await infoResponse.json();
                console.log("From FollowStatus method: ", userData);
                const profilePicture = userData.pictureBase64 ? `data:image/png;base64, ${userData.pictureBase64}` : "../images/user.png";
                const unfollowButton = `<button class="btn-unfollow" data-id="${userId}">Unfollow</button>`;

                console.log("The username from followStatus is: ", userData.username);
                console.log("The ProfilePicture from followStatus is: ", profilePicture);
                console.log("The followerId from followStatus is: ", data.followerId);
                buildUserCard("#friend-list", data.followerId, userData.username, profilePicture, "following-item")
            })
        }
    }
    catch (error) {
        console.log(error); 
    }
}

function buildUserCard(container, userId, username, profilePicture, customClass = "") {
    const card = $(`
        <div class="${customClass}" id="user-card-${userId}">
            <img src="${profilePicture}" alt="User Image" class="card-img-top user-image" id="user-image-${userId}" />
            <h2 class="card-title username">${username}</h2>
            <div class="button-section">
                <button class="btn follow-btn"
                        id="follow-btn-${userId}" 
                        data-username="${username}" 
                        data-picture="${profilePicture}">
                    Follow
                </button>
            </div>
        </div>
    `);

    $(container).append(card);
    return card;
}

//when the user press follow button it will call the API sendrequest, call API that is counting for accepting and pending status
//press follow call API (AddUser)


//The problem i have now is when i press the modal i have an empty list 
