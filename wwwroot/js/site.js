// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener("scroll", function () {

    const navbar = document.querySelector(".navbar");

    if (window.scrollY > 20) {

        navbar.classList.add("navbar-scrolled");

    }
    else {

        navbar.classList.remove("navbar-scrolled");

    }

});