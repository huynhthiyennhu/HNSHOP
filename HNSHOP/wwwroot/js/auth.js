<script>
    function togglePassword(inputId, iconId) {
        let input = document.getElementById(inputId);
    let icon = document.getElementById(iconId);
    if (input.type === "password") {
        input.type = "text";
    icon.classList.replace("fa-eye", "fa-eye-slash");
        } else {
        input.type = "password";
    icon.classList.replace("fa-eye-slash", "fa-eye");
        }
    }
</script>