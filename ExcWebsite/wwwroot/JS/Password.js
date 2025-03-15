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

    $("#password-btn").on("click", async (event) => {
        event.preventDefault();
        event.stopPropagation();

        const password = $("#newPassword").val();
        console.log("The new passwrod is: ", password);
        extractTokenFromURL(password);
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

        const responseEmail = await fetch(`https://localhost:7223/api/Password/Send-Email`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: data.userId, Email: email })
        });

        const dataEmail = await responseEmail.json();
        console.log("The user email is: ", dataEmail.email, "and data is: ", dataEmail);
        if (!responseEmail.ok) {
            alert(dataEmail.msg || "Error sending email.");
        } else {
            alert("Reset password email has been sent successfully!");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An unexpected error occurred while sending the email. Please try again.");
    }
};

const extractTokenFromURL = async (password) => {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get("token");

    console.log("Extracted Token:", token); // Debugging

    if (!token) {
        console.log("Invalid token");
        alert("Invalid reset link. Please request a new one.");
        window.location.href = "mainPage.html"; // Redirect to request a new reset email
    } else {
        resetToken = token;
        console.log("Token successfully extracted:", resetToken);

        if (!resetToken) {
            console.log("No valid token found.");
            alert("Invalid reset link. Please request a new one.");
            return; 
        }

        try {
            console.log("The password fro extractToken is: ", password);
            const resetPasswordAPI = await fetch('https://localhost:7223/api/Password/ResetPassword', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ NewPassword: password, Token: resetToken })
            });

            const resetPassword = await resetPasswordAPI.json();
            if (resetPasswordAPI.ok)
                alert("Password has been changed");
            else
                alert("Password did not change");
        }
        catch (error) {
            console.log(error); 
        }
    }
};

if (window.location.pathname.includes("ResetPassword.html")) {
    window.onload = extractTokenFromURL;
}

