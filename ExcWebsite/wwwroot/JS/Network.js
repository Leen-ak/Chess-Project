//first we need all the username that is in the database

//what i will need

$(() => {
    const username = getCookie("username");
    getPhotot(username); 
    GetUsernames();
});

const followingCount = () => {
    
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
                console.log(`The username is: ${username} and the data.user is: ${user.username}`);
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

const buildUserCard = (data) => {
    const profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
    div = $(` 
              <div class="card network-card" id="user-card">
              <img src=${profilePicture} alt="Chess Game Image" class="card-img-top user-image" id="user-image"/>
              <h2 class="card-title username">${data.username}</h2>
              <div class="button-section">
              <button class="btn" id="follow-btn">Follow</button>
              </div>
              </div>`
    );

    div.find("#follow-btn").on("click", function () {
        console.log("FOllow button click", data.username);
        
    });

    div.appendTo($(".grid-container"));
}