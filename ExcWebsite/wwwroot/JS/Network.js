$(() => {
    const username = getCookie("username");
    getPhotot(username); 
    GetUsernames();
    $("#following-btn").on("click", function () { 
        $("#theModal").modal('show'); 
    }); 

    let currentCount = parseInt($("#following-count").text()) || 0;
    if (currentCount === 0) 
        $("#friend-list").html('<p class="no-followers-text">You are not following any user yet</p>');
    
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
            const followingItem = $(`
                <div class="following-item" id="following-${data.id}">
                    <img src="${profilePicture}" class="following-pic" alt="${data.username}" />
                    <span class="following-username">${data.username}</span>
                    <button class="btn-unfollow" data-id="${data.id}">Unfollow</button>
                </div>
            `);

            $(`#user-card-${data.id}`).hide();
            $("#friend-list").append(followingItem);

            let currentCount = parseInt($("#following-count").text()) || 0;
            if (currentCount >= 0)
                $("#friend-list").find('.no-followers-text').remove();

            currentCount++;
            $("#following-count").text(currentCount);
        });

        div.appendTo($(".grid-container"));
    } catch (error) {
        console.log(error);
    }
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

