$(document).ready(function (){
    $('#eye-btn').click(function (e){
        e.preventDefault();
        let password = $('#password');

        if (password.attr("type") === "password") {
            password.attr("type", "text");
        }
        else {
            password.attr("type", "password");
        }
    });

    $('#repeat-eye').click(function (e){
        e.preventDefault();
        let password = $('#repeat');

        if (password.attr("type") === "password") {
            password.attr("type", "text");
        }
        else {
            password.attr("type", "password");
        }
    });
    
    $('#new-password-eye').click(function (e){
        e.preventDefault();
        let password = $('#new-password');

        if (password.attr("type") === "password") {
            password.attr("type", "text");
        }
        else {
            password.attr("type", "password");
        }
    });
});