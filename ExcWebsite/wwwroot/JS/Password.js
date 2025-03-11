$(() => {
    console.log("hello");

    $("#submit-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const email = $("#email").val();
        console.log("The user email is", email);
        //await getUserId(email); 
        await SendEmail(email); 
    });
});


const SendEmail = async (email) => {

    try {
        console.log("From getUserId: ", email);
        const response = await fetch('https://localhost:7223/api/Password/VerifyEmail', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email })
        });

        const data = await response.json();
        console.log("data from getUserId is: ", data);
        console.log("The user ID from JS IS: ", data.userId);

        const response2 = await fetch(`https://localhost:7223/api/Password/Send-Email`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: data.userId, Email: email })
        });

        const data2 = await response2.json();
        console.log("The user email is: ", data2.email, "and data is: ", data2);
        if (!response2.ok) {
            alert(data2.msg || "Error sending email.");
        } else {
            alert("Reset password email has been sent successfully!");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An unexpected error occurred while sending the email. Please try again.");
    }
};
