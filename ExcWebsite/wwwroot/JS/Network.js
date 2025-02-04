//first we need all the username that is in the database

//what i will need

$(() => {
    GetUsernames();
});

const GetUsernames = async () => {
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
        data.forEach(user => console.log("Username:", user.username));
        data.forEach(user => console.log("Picture:", user.picture));

        if (response.ok) {
            data.forEach(user => buildUserCard(user)); 
        }        
    }
    catch (error) {
        console.log(error); 
    }
};

const buildUserCard = (data) => {
    const profilePicture = data.picture ? `data:image/png;base64,${data.picture}` : "../images/user.png";
    div = $(` 
              <div class="card network-card" id="user-card">
              <img src=${profilePicture} alt="Chess Game Image" class="card-img-top user-image" id="user-image"/>
              <h2 class="card-title username">${data.username}</h2>
              <div class="button-section">
              <button class="btn btn-primary">Follow</button>
              </div>
              </div>`
    );

    div.appendTo($(".grid-container"));
}