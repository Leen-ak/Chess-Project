$(() => {
    console.log("hello");

    $("#submit-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const email = $("#email").val();
        console.log("The user email is", email);

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
                alert("Email verified! Sending password reset link...");
                await SendEmail(email); 
            }
        } catch (error) {
            console.error("Error:", error);
            alert("An unexpected error occurred. Please try again.");
        }
    });
});


const SendEmail = async (email) => {
    try {
        const response = await fetch(`https://localhost:7223/api/Password/Send-Email`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email })
        });

        let data;
        const contentType = response.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            data = await response.json();
        } else {
            data = await response.text();  
        }

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
