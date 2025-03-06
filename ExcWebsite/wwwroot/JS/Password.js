$(() => {
    console.log("hello");
    $("#submit-btn").on("click", (event) => {
        event.preventDefault(); 
        const email = $("#email").val();
        console.log("The user email is", email);
        getEmail(email); 
    });
});

const getEmail = async (email) => {
    try {
        const response = await fetch(`https://localhost:7223/api/Password/VerifyEmail`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email })  
        });

        const data = await response.json(); 

        if (!response.ok) {
            alert(data.msg || "Error verifying email.");
        } else if (!data.userId) {
            alert("Email not found in the database.");
        } else {
            alert("Reset password link has been sent to your email!");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An unexpected error occurred. Please try again.");
    }
};
