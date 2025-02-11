$(() => {
    const username = getCookie("username");
    getPhotot(username);
    GetUsernames(username);

    $("#following-btn").on("click", function () {
        $("#theModal .modal-title").text("Following");
        let currentCount = parseInt($("#following-count").text()) || 0;
        if (currentCount === 0)
            $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
        $("#theModal").modal('show');
    });
    $("#follower-btn").on("click", function () {
        $("#theModal .modal-title").text("Followers");
        let currentCount = parseInt($("#following-count").text()) || 0;
        if (currentCount === 0)
            $("#friend-list").html('<p class="no-followers-text">You do not have any followers yet</p>');
        $("#theModal").modal('show');
    });
    $("#request-btn").on("click", async function () {
        $("#theModal .modal-title").text("Requests");
        $("#friend-list").empty(); // Clear old data

        try {
            const response = await fetch(`https://localhost:7223/api/LoginPage/username/${getCookie("username")}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const userData = await response.json();

            console.log("print", userData);
            console.log("user ID is: ", userData.id);

            const statusResponse = await fetch(`https://localhost:7223/api/Network/Status/${userData.id}`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            });



            if (statusResponse.ok) {
                const statusData = await statusResponse.json();
                console.log("The response is from the status fetch:", statusData);

                // Extract the array from the result object
                const resultArray = statusData.result;

                // Check if the array has data
                if (Array.isArray(resultArray) && resultArray.length > 0) {
                    resultArray.forEach(async request => {
                        console.log("Status:", request.status);

                            const getInfo= await fetch(`https://localhost:7223/api/Network/GetUserName/${request.followerId}`, {
                                method: 'GET',
                                headers: {
                                    'Content-Type': 'application/json'
                                }
                            });

                            const getUsername = await getInfo.json();
                            console.log("The info that came from the getUsernaem api is: ", getUsername);
                        console.log("The username that came from the getUsernaem api is: ", getUsername.vm.username);
                        let profilePicture = getUsername.vm.picture ? `data:image/png;base64,${getUsername.vm.picture}` : "../images/user.png";


                        if (request.status === "Pending") {
                            console.log("is pending", request.followerId);
                            console.log("The username after the try and catch ", getUsername.vm.username);

                            //i will need to create gettheusernamebyid
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

                    let currentCount = parseInt($("#request-count").text()) || 0;
                    $("#request-count").text(resultArray.length);
                } else {
                    console.log("No status data available.");
                    $("#friend-list").html('<p class="no-followers-text">You do not have any friend requests.</p>');
                }
            }
        } catch (error) {
            console.log("An error occurred:", error);
        }

        // Show the modal after data is loaded
        $("#theModal").modal('show');
    });
})


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

const GetUsernames = async (username) => {
    try {
        const response = await fetch('https://localhost:7223/api/Network', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        let profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";

        if (response.ok) {
            data.forEach(user => {
                //if the user login in it will not show the name of the user in the friend suggestions
                if (username === user.username)
                    return;
                buildUserCard(user, username);
                //id, followerId, followingId, status, username, picture
            });
        }        
    }
    catch (error) {
        console.log(error); 
    }
};

const buildUserCard = async (data) => {
    const profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
    buildTheCard(data.id, data.username, profilePicture);
};

$(document).on("click", ".btn-unfollow", function () {
    const userId = $(this).data("id");

    $(`#user-card-${userId}`).fadeIn().appendTo(".grid-container");
    $(`#following-${userId}`).remove();

    let currentCount = parseInt($("#following-count").text()) || 0;
    if (currentCount > 0) 
        currentCount--;
    
    $("#following-count").text(currentCount);

    if (currentCount === 0) {
        $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
    }
});

const GetRequestStatus = async (username) => {
    console.log("Fetching user ID for username:", username);
};

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
    buildFollowList(div, userId, username, profilePicture);

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
            followerId: userData.id,  //me
            followingId: userId,
            status: "Pending"
        };
        console.log("Sending follow request:", followData);

        try {
            const followResponse = await fetch('https://localhost:7223/api/Network', {
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


function buildRequestList(div, userId, username_, profilePicture) {
    div.find(`#follow-btn-${userId}`).on("click", async function () {
        const RequestItem = $(`
            <div class="following-item" id="following-${userId}">
                <img src="${profilePicture}" class="following-pic" alt="${username_}" />
                <span class="following-username">${username_}</span>
                <button class="btn-unfollow" data-id="${userId}">Accept</button>
            </div>
        `);

        // Move the card to the modal list
        $("#friend-list").append(followingItem);

        // Hide the original card from the grid
        $(`#user-card-${userId}`).hide();

        let currentCount = parseInt($("#following-count").text()) || 0;
        if (currentCount >= 0)
            $("#friend-list").find('.no-followers-text').remove();

        currentCount++;
        $("#following-count").text(currentCount);
    });
 }


//TO DO
//1. adding the following to the database
//2. followers logic
//3. adding the followers to the database
//4. Handle the status when there is not picture 
/*

Sumarry of the steps of sending a request: 

1️ Update Database → Add a Status column to track follow requests. DONE
2️ Modify Follow Action → Store requests as "Pending". DONE
3️ Create a Requests Modal → Show pending follow requests.
4️ Implement Accept/Reject → Users can approve or deny requests.
5️ Notify the User → Optional notification when accepted.

*/ 