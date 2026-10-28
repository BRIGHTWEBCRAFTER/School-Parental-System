(function () {

    "use strict";


    // =====================================================
    // ELEMENTS
    // =====================================================

    const container =
        document.getElementById(
            "authContainer"
        );

    const forgotLink =
        document.querySelector(
            ".ForgotLink"
        );

    const backLink =
        document.querySelector(
            ".BackToLoginLink"
        );

    const passwordInput =
        document.getElementById(
            "loginPassword"
        );

    const passwordToggle =
        document.getElementById(
            "passwordToggle"
        );


    // =====================================================
    // LOGIN -> FORGOT PASSWORD
    // =====================================================

    if (container && forgotLink) {

        forgotLink.addEventListener(
            "click",
            function (event) {

                event.preventDefault();

                container.classList.add(
                    "active"
                );

                window.history.replaceState(
                    null,
                    "",
                    "#forgot"
                );


                const forgotEmail =
                    document.getElementById(
                        "forgotEmail"
                    );


                // If the user already entered their
                // login email, copy it to reset email.

                const loginEmail =
                    document.getElementById(
                        "Email"
                    );


                if (
                    forgotEmail &&
                    loginEmail &&
                    loginEmail.value.trim() !== ""
                ) {

                    forgotEmail.value =
                        loginEmail.value.trim();

                }


                // Focus reset email after animation begins.

                if (forgotEmail) {

                    window.setTimeout(
                        function () {

                            forgotEmail.focus();

                        },
                        500
                    );

                }

            }
        );

    }


    // =====================================================
    // FORGOT PASSWORD -> LOGIN
    // =====================================================

    if (container && backLink) {

        backLink.addEventListener(
            "click",
            function (event) {

                event.preventDefault();

                container.classList.remove(
                    "active"
                );

                window.history.replaceState(
                    null,
                    "",
                    window.location.pathname +
                    window.location.search
                );


                const loginEmail =
                    document.getElementById(
                        "Email"
                    );


                if (loginEmail) {

                    window.setTimeout(
                        function () {

                            loginEmail.focus();

                        },
                        500
                    );

                }

            }
        );

    }


    // =====================================================
    // OPEN FORGOT PASSWORD DIRECTLY
    // /Auth/Login#forgot
    // =====================================================

    if (
        container &&
        window.location.hash === "#forgot"
    ) {

        container.classList.add(
            "active"
        );

    }


    // =====================================================
    // SHOW / HIDE PASSWORD
    // =====================================================

    if (
        passwordInput &&
        passwordToggle
    ) {

        passwordToggle.addEventListener(
            "click",
            function () {

                const isVisible =
                    passwordInput.type ===
                    "text";


                passwordInput.type =
                    isVisible
                        ? "password"
                        : "text";


                passwordToggle.innerHTML =
                    isVisible
                        ? '<i class="bi bi-eye-fill"></i>'
                        : '<i class="bi bi-eye-slash-fill"></i>';


                passwordToggle.setAttribute(
                    "aria-label",
                    isVisible
                        ? "Show password"
                        : "Hide password"
                );


                passwordToggle.setAttribute(
                    "title",
                    isVisible
                        ? "Show password"
                        : "Hide password"
                );


                // Return focus to password input.

                passwordInput.focus();

            }
        );

    }


    // =====================================================
    // HANDLE BROWSER HASH CHANGES
    // =====================================================

    window.addEventListener(
        "hashchange",
        function () {

            if (!container)
                return;


            if (
                window.location.hash ===
                "#forgot"
            ) {

                container.classList.add(
                    "active"
                );

            }
            else {

                container.classList.remove(
                    "active"
                );

            }

        }
    );


})();