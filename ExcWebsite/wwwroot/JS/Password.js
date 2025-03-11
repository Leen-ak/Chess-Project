$(() => {
    console.log("hello");

    $("#submit-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const email = $("#email").val();
        console.log("The user email is", email);
        await SendEmail(email); 
    });
});



const SendEmail = async (email) => {
    try {
        const response = await fetch(`https://localhost:7223/api/Password/Send-Email`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: null, Email: email })
        });

        const data = await response.json();
        console.log("The user email is: ", data.email, "and data is: ", data);
        if (!response.ok) {
            alert(data.msg || "Error sending email.");
        } else {
            alert("Reset password email has been sent successfully!");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An unexpected error occurred while sending the email. Please try again.");
    }
};
