document.addEventListener("DOMContentLoaded", function () {
    var passwordInput = document.getElementById("Password");
    var strengthOutput = document.createElement('div');
    passwordInput.parentElement.appendChild(strengthOutput);

    passwordInput.addEventListener("input", function () {
        var strength = checkPasswordStrength(passwordInput.value);
        strengthOutput.textContent = "Password Strength: " + strength;
        strengthOutput.style.color = getStrengthColor(strength);
    });

    function checkPasswordStrength(password) {
        var strength = "Weak";
        var regex = [
            "[A-Z]",  // Uppercase
            "[a-z]",  // Lowercase
            "[0-9]",  // Numbers
            "[$@$!%*?&#]"  // Special characters
        ];

        var passedTests = regex.reduce(function (count, exp) {
            return count + (new RegExp(exp).test(password) ? 1 : 0);
        }, 0);

        if (passedTests > 2 && password.length > 7) {
            strength = "Strong";
        } else if (passedTests > 1) {
            strength = "Medium";
        }
        return strength;
    }

    function getStrengthColor(strength) {
        switch (strength) {
            case "Weak":
                return "red";
            case "Medium":
                return "orange";
            case "Strong":
                return "green";
        }
    }


    document.addEventListener('DOMContentLoaded', function () {
        var passwordInput = document.getElementById('password');
        var togglePasswordButton = document.getElementById('toggle-password');
        var passwordIcon = togglePasswordButton.querySelector('i');

        togglePasswordButton.addEventListener('click', function () {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                passwordIcon.classList.remove('bi-eye-slash');
                passwordIcon.classList.add('bi-eye');
            } else {
                passwordInput.type = 'password';
                passwordIcon.classList.remove('bi-eye');
                passwordIcon.classList.add('bi-eye-slash');
            }
        });
    });

    









        
});
   





