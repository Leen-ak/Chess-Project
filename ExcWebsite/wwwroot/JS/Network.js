let followingCount = 0; 
let requestCount = 0;
let followerCount = 0; 

//for reminder: The following list is fine for the person who does the follow but not right for the other user...
//and it looks like this is a backend problem not from frontend
$(() => {
    $("#following-count").text(followingCount);
    $("#request-count").text(requestCount);
    $("follower-count").text(followerCount);

    main();
    $("#following-btn").on("click", async function () {
        followingList(); 
    });
    $("#follower-btn").on("click", async function () {
        followerList();
    });
    $("#request-btn").on("click", async function () {
        requestList();
    });
});

$(document).on("click", ".btn-unfollow", async function () {
    const followingId = $(this).data("id");
    await UnfollowUser(followingId);

    //Get username and profile picture from the current card
    const card = $(`#following-${followingId}`);
    const username = card.find(".following-username").text();
    const profilePicture = card.find("img").attr("src");
    card.remove();
    buildUserCard(".grid-container", followingId, username, profilePicture, "card network-card");
    followingCount--;
    $("#following-count").text(followingCount);

});

$(document).on("click", ".follow-btn", async function () {
    const button = $(this);
    const userId = button.attr("id").split("-")[2];
    const username = button.data("username");
    const profilePicture = button.data("picture");

    await AddUser(userId);
    followingCount++;
    $("#following-count").text(followingCount);
    const card = buildUserCard("#friend-list", userId, username, profilePicture, "")
    $(`.grid-container #user-card-${userId}`).remove();
});

$(document).on("click", ".btn-accept", async function () {
    const userId = $(this).data("id");

    const response = await fetch(`https://localhost:7223/api/Network/Status`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    });

    const data = await response.json();
    const array = data.pendingReceived;

    for (const r of array) {
        console.log("The data from followerList *** API is: ", data);
        console.log("The id is: ", r.id);
        console.log("The followerId is: ", r.followerId);
        console.log("The followingId is: ", r.followingId);

        const response = await fetch('https://localhost:7223/api/Network/UpdateStatus', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                followerId: r.followerId,
                followingId: r.followingId,
                status: "Accepted"
            })
        });

        if (response.ok) {
            const data = await response.json();
            
        }
    }
});

$(document).on("click", ".btn-reject", async function () {
    const userId = $(this).data("id");
    const response = await fetch(`https://localhost:7223/api/Network/Status`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    });

    const data = await response.json();
    const array = data.pendingReceived;

    for (const r of array) {
        console.log("The data from followerList *** API is: ", data);
        console.log("The id is: ", r.id);
        console.log("The followerId is: ", r.followerId);
        console.log("The followingId is: ", r.followingId);

        const response = await fetch('https://localhost:7223/api/Network/UpdateStatus', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                followerId: r.followerId,
                followingId: r.followingId,
                status: "Rejected"
            })
        });

        if (response.ok) {
            const data = await response.json();
        }
    }
});

async function followerList() {
    $("#theModal .modal-title").text("Followers");
    $("#friend-list").empty();

    try {
        const response = await fetch(`https://localhost:7223/api/Network/Status`, {
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            const data = await response.json();
            const acceptRequest = data.acceptedRequest;
            if (acceptRequest.lenght === 0) {
                $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
                return;
            }

            $("#following-count").text(acceptRequest.length);

            for (const request of acceptRequest) {
                const userId = request.followerId;
                const userResponse = await fetch(`https://localhost:7223/api/Network/GetUserById/${userId}`, {
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                });

                if (!userResponse.ok) {
                    console.log(`Failed to fetch user with ID ${userId}`);
                    continue;
                }

                const user = await userResponse.json();
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                const followerItem = $(`
                       <div class="following-item" id="following-${user.id}">
                           <img src="${profilePicture}" class="following-pic" alt="${user.userName}" />
                           <span class="following-username">${user.userName}</span>
                           <button class="btn-follow-back" data-id="${user.id}">Follow Back</button>
                       </div>
                   `);
                $("#friend-list").append(followerItem);
            }
        }
    } catch (error) {
        console.log(error);
    }
    
    $("#follower-list").empty();
    $("#theModal").modal('show');

}

async function requestList() {
    $("#theModal .modal-title").text("Friend Requests");
    $("#friend-list").empty();

    try {
        const response = await fetch(`https://localhost:7223/api/Network/Status`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            const data = await response.json();
            const pendingRecive = data.pendingReceived;

            for (const request of pendingRecive) {
                const followerId = request.followerId;
                const userResponse = await fetch(`https://localhost:7223/api/Network/GetUserById/${followerId}`, {
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                });

                if (!userResponse.ok) {
                    console.log(`Failed to fetch user with ID ${followerId}`);
                    continue;
                }

                const user = await userResponse.json();
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                const followerItem = $(`
                     <div class="following-item" id="following-${user.id}">
                            <img src="${profilePicture}" class="following-pic" alt="${user.userName}" />
                            <span class="following-username">${user.userName}</span>
                          <button class="btn-action btn-accept" data-id="${user.id}">
                            <svg class="icon-accept" width="24" height="24" viewBox="0 0 24 24" fill="green" xmlns="http://www.w3.org/2000/svg">
                                <path d="M9 16.2L4.8 12L3.4 13.4L9 19L21 7L19.6 5.6L9 16.2Z"/>
                            </svg>
                          </button>

                            <button class="btn-action btn-reject" data-id="${user.id}">
                                <svg class="icon-reject" width="24" height="24" viewBox="0 0 24 24" fill="red" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M6 6L18 18M6 18L18 6" stroke="red" stroke-width="2"/>
                                </svg>
                            </button>
                      </div>
                 `);
                $("#friend-list").append(followerItem);
            }
        }
    }
    catch (error) {
        console.log(error); 
    }

    $("#theModal").modal('show');
};

async function followingList() {
    $("#theModal .modal-title").text("Following");
    $("#friend-list").empty();

    try {
        const response = await fetch(`https://localhost:7223/api/Network/Status`, {
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            const data = await response.json();
            const pendingRequests = data.pendingSent;
            if (pendingRequests.lenght === 0) {
                $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
                return;
            }

            $("#following-count").text(pendingRequests.length);

            for (const request of pendingRequests) {
                const userId = request.followingId;
                const userResponse = await fetch(`https://localhost:7223/api/Network/GetUserById/${userId}`, {
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                });

                if (!userResponse.ok) {
                    console.log(`Failed to fetch user with ID ${userId}`);
                    continue;
                }

                const user = await userResponse.json();
                const profilePicture = user.picture ? `data:image/png;base64, ${user.picture}` : "../images/user.png";
                const followingItem = $(`
                        <div class="following-item" id="following-${user.id}">
                            <img src="${profilePicture}" class="following-pic" alt="${user.userName}" />
                            <span class="following-username">${user.userName}</span>
                            <button class="btn-unfollow" data-id="${user.id}">Unfollow</button>
                        </div>
                    `);
                $("#friend-list").append(followingItem);
            }
        }
    } catch (error) {
        console.log("Error loading following list: ", error);
    }
    $("#theModal").modal('show');
}

async function main() {
    try {
        const res = await fetch("https://localhost:7223/api/MainPage/Users", {
            method: "GET",
            credentials: "include"
        });

        const data = await res.json();
        const username = data.username;
        const userId = data.userId;
        const picture = await GetPhoto(username);
        $("#user-image").attr("src", picture);
        GetAllSuggestedFriend(userId);

        // 🟢 Add this
        const statusResponse = await fetch(`https://localhost:7223/api/Network/Status`, {
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        if (statusResponse.ok) {
            const statusData = await statusResponse.json();
            
            followingCount = statusData.pendingSent.length;
            requestCount = statusData.pendingReceived;
            //console.log("The aray size is: ", requestCount.length);
            //console.log("followingCount is: ", followingCount);
            //console.log("request count is: ", requestCount);
            $("#follower-count").text(followerCount);

        }

    } catch (error) {
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
            $("#following-count").text(data.pendingSent);
        }
    }
    catch (error) {
        console.log(error); 
    }
}

const UnfollowUser = async (followingId) => {
    const userInfo = await fetch("https://localhost:7223/api/MainPage/Users", {
        method: "GET",
        credentials: "include"
    });

    if (!userInfo.ok) {
        console.log("Failed to fetch current user info");
        return;
    }

    const data = await userInfo.json();
    const followerId = data.userId;
    console.log("Unfollow btn followerId is: ", followerId);
    console.log("Unfollow btn followingId is: ", followingId);
    try {
        const response = await fetch('https://localhost:7223/api/Network/UnfollowUser', {
            method: "DELETE",
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({

                followerId: followerId,
                followingId: followingId
            })
        });
        const data = await response.json();
        console.log(data);
    }
    catch (error) {
        console.log(error);
    }
};

function buildUserCard(container, userId, username, profilePicture, customClass = "") {
    const card = $(`
        <div class="${customClass}" id="user-card-${userId}">
            <img src="${profilePicture}" alt="User Image" class="card-img-top user-image following-pic" id="user-image-${userId}" />
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

//TODO FOR TODAY
//1. CALLING Friend Request and showing the request in the request list
//2. accept the user or decline the request
//3. what happen when i accept or reject the friend request
//4. when there is not following to show or followers it should print that
