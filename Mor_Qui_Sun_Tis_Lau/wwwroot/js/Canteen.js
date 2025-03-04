function displayItem(itemId) {
    $.ajax({
        type: 'POST',
        url: "Canteen?handler=DisplayItem",
        data: { itemId: itemId }, 
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            $('#prodDisplayContainer').html(response);
        },
        error: function (xhr, status, error) {
            console.log("Error");
        }
    });
};

function updateItemCount(countAction) {
    const itemCountSpan = document.getElementById('countSpan');
    const itemCountHidden = document.getElementById('itemCount');

    let itemCount = parseInt(itemCountSpan.textContent) || 0;

    if (countAction === "add") {
        itemCount++;
    } else if (countAction === "rem" && itemCount > 0) {
        itemCount--;
    }

    itemCountSpan.textContent = itemCount;
    itemCountHidden.value = itemCount; 

    const addToCartBtn = document.getElementById('addToCartBtn');
    addToCartBtn.disabled = itemCount <= 0;
    addToCartBtn.classList.toggle("inactive", itemCount <= 0);
};

