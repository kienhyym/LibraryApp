$(function () {

    const form = $("#residentForm");

    const sendOtpUrl = form.data("sendotp-url");

    const verifyOtpUrl = form.data("verifyotp-url");

    const resendOtpUrl = form.data("resendotp-url");

    const modal = new bootstrap.Modal(
        document.getElementById("otpModal"));

    let countdown = 300;

    let timer = null;

    //----------------------------------------------------
    // Gửi OTP
    //----------------------------------------------------

    $("#btnSendOtp").click(function () {

        if (!form.valid())
            return;

        $.ajax({

            url: sendOtpUrl,

            type: "POST",

            data: form.serialize(),

            success: function (res) {

                if (!res.success) {

                    Swal.fire({

                        icon: "error",

                        title: "Lỗi",

                        text: res.message

                    });

                    return;
                }

                $("#emailDisplay").text(
                    $("#Email").val());

                $("#OtpCode").val("");

                modal.show();

                startCountdown();

                Swal.fire({

                    icon: "success",

                    title: "Thành công",

                    text: res.message,

                    timer: 1500,

                    showConfirmButton: false

                });

            },

            error: function () {

                Swal.fire({

                    icon: "error",

                    title: "Lỗi",

                    text: "Không thể gửi OTP."

                });

            }

        });

    });

    //----------------------------------------------------
    // Xác nhận OTP
    //----------------------------------------------------

    $("#btnVerifyOtp").click(function () {

        const otp = $("#OtpCode").val();

        if (otp === "") {

            Swal.fire({

                icon: "warning",

                title: "Thông báo",

                text: "Vui lòng nhập OTP."

            });

            return;
        }

        let data = form.serialize();

        data += "&OtpCode=" + otp;

        $.ajax({

            url: verifyOtpUrl,

            type: "POST",

            data: data,

            success: function (res) {

                if (!res.success) {

                    Swal.fire({

                        icon: "error",

                        title: "Lỗi",

                        text: res.message

                    });

                    return;
                }

                Swal.fire({

                    icon: "success",

                    title: "Thành công",

                    text: "Tạo cư dân thành công.",

                    timer: 1500,

                    showConfirmButton: false

                }).then(() => {

                    window.location =
                        res.redirectUrl;

                });

            },

            error: function () {

                Swal.fire({

                    icon: "error",

                    title: "Lỗi",

                    text: "Không thể xác thực OTP."

                });

            }

        });

    });

    //----------------------------------------------------
    // Gửi lại OTP
    //----------------------------------------------------

    $("#btnResendOtp").click(function () {

        $.ajax({

            url:resendOtpUrl,

            type: "POST",

            data: {

                email: $("#Email").val()

            },

            success: function (res) {

                if (!res.success) {

                    Swal.fire({

                        icon: "error",

                        title: "Lỗi",

                        text: res.message

                    });

                    return;
                }

                Swal.fire({

                    icon: "success",

                    title: "Đã gửi",

                    text: res.message,

                    timer: 1500,

                    showConfirmButton: false

                });

                startCountdown();

            }

        });

    });

    //----------------------------------------------------
    // Countdown
    //----------------------------------------------------

    function startCountdown() {

        clearInterval(timer);

        countdown = 300;

        $("#btnResendOtp").prop("disabled", true);

        updateCountdown();

        timer = setInterval(function () {

            countdown--;

            updateCountdown();

            if (countdown <= 0) {

                clearInterval(timer);

                $("#btnResendOtp")
                    .prop("disabled", false);

            }

        }, 1000);

    }

    function updateCountdown() {

        const minute = Math.floor(countdown / 60);

        const second = countdown % 60;

        $("#countdown").text(

            minute.toString().padStart(2, '0')
            + ":"
            + second.toString().padStart(2, '0')

        );

    }

    //----------------------------------------------------
    // Đóng modal
    //----------------------------------------------------

    $('#otpModal').on(
        'hidden.bs.modal',
        function () {

            clearInterval(timer);

        });

});