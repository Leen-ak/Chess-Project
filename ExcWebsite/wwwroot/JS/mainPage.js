$(() => {
    $("#signup").on("click", () => {
        const $image = $(".image-section");
        const $btn = $(".btn-group");

        // Move image to the right and buttons to the left
        if (!$image.hasClass("right")) {
            $image.removeClass("left").addClass("right");
            $("#signup").addClass("active");
            $("#login").removeClass("active");
            $btn.removeClass("btn-right").addClass("btn-left");
            $("#login-hide").fadeOut();
            $(".signup-container").addClass("active");
        }
    });

    $("#login").on("click", () => {
        const $image = $(".image-section");
        const $btn = $(".btn-group");

        // Move image to the left and buttons to the right
        if (!$image.hasClass("left")) {
            $image.removeClass("right").addClass("left");
            $("#login").addClass("active");
            $("#signup").removeClass("active");
            $btn.removeClass("btn-left").addClass("btn-right");
            $(".signup-container").removeClass("active")
            $("#login-hide").fadeIn();
        }
    });
});

