// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener("scroll", function () {

    const navbar =
        document.querySelector(".navbar");

    if (!navbar) {
        return;
    }

    if (window.scrollY > 20) {

        navbar.classList.add(
            "navbar-scrolled"
        );

    }
    else {

        navbar.classList.remove(
            "navbar-scrolled"
        );

    }

});

// ==========================================
// LibraryApp Notification
// ==========================================

document.addEventListener("DOMContentLoaded", function () {

    if (!window.appNotification) {
        return;
    }

    const notification =
        window.appNotification;

    Swal.fire({

        toast: true,

        position: "top-end",

        icon: notification.type,

        title: notification.message,

        showConfirmButton: false,

        timer: 2500,

        timerProgressBar: true,

        showClass: {
            popup: "swal2-show"
        },

        hideClass: {
            popup: "swal2-hide"
        }

    });

});