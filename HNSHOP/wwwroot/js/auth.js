
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

function toggleForm(formId) {
    var form = document.getElementById(formId);
    form.style.display = (form.style.display === "none") ? "block" : "none";
}
function uploadAvatar(input) {
    var file = input.files[0];
    if (file) {
        var allowedExtensions = ["image/jpeg", "image/png", "image/jpg"];
        if (!allowedExtensions.includes(file.type)) {
            alert("Vui lòng chọn ảnh có định dạng JPG, JPEG hoặc PNG.");
            return;
        }

        var formData = new FormData();
        formData.append("Avatar", file);

        fetch('/Accounts/UpdateAvatar', {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    let avatarPreview = document.getElementById("avatarPreview");
                    let newSrc = "/images/hnshop/avatar/" + data.imageName + "?t=" + new Date().getTime();
                    if (avatarPreview) {
                        avatarPreview.src = newSrc;
                        alert("Cập nhật ảnh thành công!");
                    } else {
                        location.reload();
                    }
                } else {
                    alert("Cập nhật ảnh thất bại: " + data.message);
                }
            })
            .catch(error => {
                alert("Có lỗi xảy ra, vui lòng thử lại!");
                console.error("Lỗi upload:", error);
            });
    }
}



