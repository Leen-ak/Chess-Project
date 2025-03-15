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
        resetPasswordHandler();
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

const extractTokenFromURL = async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get("token");

    console.log("Extracted Token:", token); // Debugging

    if (!token) {
        console.log("Invalid token");
        alert("Invalid reset link. Please request a new one.");
        window.location.href = "mainPage.html"; // Redirect to request a new reset email
        return null; 
    } else {
        resetToken = token;
        console.log("Token successfully extracted:", resetToken);
        return resetToken; 
        if (!resetToken) {
            console.log("No valid token found.");
            alert("Invalid reset link. Please request a new one.");
            return; 
        }
    }
};

const resetPasswordHandler = async () => {
    const password = $("#newPassword").val();
    const confirmPassword = $("#confirmPassword").val();
    const token = await extractTokenFromURL();

    if (!token) {
        alert("Invalid reset link. Please request a new one.");
        return;
    }

    if (!password || !confirmPassword) {
        alert("Both password fields are required.");
        return;
    }

    if (password !== confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    try {
        console.log("Resetting password with token:", token);
        const resetPasswordAPI = await fetch('https://localhost:7223/api/Password/ResetPassword', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ NewPassword: password, Token: token })
        });

        const resetPassword = await resetPasswordAPI.json();
        if (resetPasswordAPI.ok) {
            alert("Password has been changed successfully!");
            window.location.href = "mainPage.html"; // Redirect to login after success
        } else {
            alert(resetPassword.msg || "Password reset failed.");
        }
    } catch (error) {
        console.error("Error resetting password:", error);
        alert("An error occurred while resetting the password.");
    }
}

if (window.location.pathname.includes("ResetPassword.html")) {
    extractTokenFromURL();
}

