function updateAvatar(event) {
    // Lấy ra tệp tin được chọn
    var file = event.target.files[0];

    // Tạo một đối tượng FileReader để đọc dữ liệu từ tệp tin
    var reader = new FileReader();

    // Xác định hành động khi đọc tệp tin thành công
    reader.onload = function (e) {
        // Lấy URL của ảnh đã chọn và hiển thị nó trực tiếp trên trang
        var avatarPreview = document.getElementById("avatarPreview");
        avatarPreview.src = e.target.result;

        // Lưu đường dẫn ảnh vào input ẩn
        var avatarPath = document.getElementById("avatarPath");
        avatarPath.value = e.target.result;
    };

    // Đọc tệp tin ảnh
    reader.readAsDataURL(file);
}