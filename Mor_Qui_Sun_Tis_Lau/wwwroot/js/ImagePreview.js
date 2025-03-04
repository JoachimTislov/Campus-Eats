document.getElementById('ImageFile').addEventListener('change', function (event) {
    const fileInput = event.target;
    const file = fileInput.files[0];
    const preview = document.getElementById('ImagePreview');

    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block'; // Show the image if it's hidden
        };

        reader.readAsDataURL(file);
    } else {
        preview.src = '/img/filler.jpg'; // Reset to default if no file is selected
    }
});