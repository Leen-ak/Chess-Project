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
    const response = await fetch(`https://localhost:7223/api/Password/VerifyEmail`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({Email: email})
    });

    if (response.ok) {
        const data = await response.json();
        console.log("Server Response:", data);
    }
}