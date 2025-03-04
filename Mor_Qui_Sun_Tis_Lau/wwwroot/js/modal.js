document.addEventListener("DOMContentLoaded", function () {
    const confirmCancelOrderModal = document.getElementById("confirmCancelOrderModal");
    const cancelOrderButtons = document.querySelectorAll(".cancelOrderBtn");
    const confirmCancelBtn = document.getElementById("confirmCancelBtn");
    const revertCancelBtn = document.getElementById("revertCancelBtn");
    const body = document.body;

    let formToSubmit = null;

    cancelOrderButtons.forEach(cancelOrderBtn => {
        cancelOrderBtn.addEventListener("click", function (event) {
            event.preventDefault(); 
            formToSubmit = this.closest("form"); 
            confirmCancelOrderModal.classList.remove("hide"); 
            body.classList.add("noScroll");
        });
    });

    confirmCancelBtn.addEventListener("click", function () {
        if (formToSubmit) {
            formToSubmit.submit();
        }
        confirmCancelOrderModal.classList.add("hide");
        body.classList.remove("noScroll");
    });

    revertCancelBtn.addEventListener("click", function () {
        confirmCancelOrderModal.classList.add("hide");
        body.classList.remove("noScroll");
        formToSubmit = null;
    });
});