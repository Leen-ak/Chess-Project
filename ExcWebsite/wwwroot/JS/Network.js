let followingCount = 0; 

//for reminder: The following list is fine for the person who does the follow but not right for the other user...
//and it looks like this is a backend problem not from frontend
$(() => {
    console.log(followingCount);
    $("#following-count").text(followingCount);

    main();
    $("#following-btn").on("click", async function () {
        $("#theModal .modal-title").text("Following");
        $("#friend-list").empty();

        try {
            const response = await fetch(`https://localhost:7223/api/Network/Status`, {
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.ok) {
                const data = await response.json();
                console.log("The data that i wanna se is: ", data);
                const pendingRequests = data.pendingSent;
                const pendingRecives = data.pendingRecives;
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
    });

});

$(document).on("click", ".follow-btn", async function () {
    const button = $(this);
    const userId = button.attr("id").split("-")[2];
    const username = button.data("username");
    const profilePicture = button.data("picture");

    await AddUser(userId);
    followingCount++;
    const followingItem = $(`
        <div class="following-item" id="following-${userId}">
            <img src="${profilePicture}" class="following-pic" alt="${username}" />
            <span class="following-username">${username}</span>
            <button class="btn-unfollow" data-id="${userId}">Unfollow</button>
        </div>
    `);
    $("#friend-list").append(followingItem);
    $(`#user-card-${userId}`).remove();
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
            const followingCount = statusData.pendingSent.length;
            $("#following-count").text(followingCount);
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
            console.log("The length of pending sent is: ", data[0].pendingSent.lenght);
            console.log("The length of pending recived is: ", data[0].pendingRecives.lenght);
            console.log("The data of pending requests?? ", data[0].pendingRequests.lenght)
            $("#following-count").text(data.pendingSent);

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
