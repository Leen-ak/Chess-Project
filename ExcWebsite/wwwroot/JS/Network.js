$(() => {
    const username = getCookie("username");
    getPhotot(username); 
    GetUsernames(username);

    $("#following-btn").on("click", function () { 
        $("#theModal").modal('show'); 
    }); 
    $("#follower-btn").on("click", function () {
        $("#theModal").modal('show'); 
    });
    $("#request-btn").on("click", function () {
        $("#theModal").modal('show'); 
    });

    let currentCount = parseInt($("#following-count").text()) || 0;
    if (currentCount === 0) 
        $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
    
});

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

            const StatusResponse = await fetch(`https://localhost:7223/api/Network/Status/${userData.id}`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            });

            if (StatusResponse.ok) {
                const statusData = await StatusResponse.json();
                console.log("User status:", statusData);

                if (statusData.status === "Pending") {
                    console.log(statusData.id, " is pending so reutrn");
                    return; 
                }
            }
        }
        catch (error) {
            console.log("An error occurred:", error);
        }


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
                <button class="btn-unfollow" data-id="${userId}">Unfollow</button>
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