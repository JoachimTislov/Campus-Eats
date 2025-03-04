const EL = (id) => document.getElementById(id);

const countElement = (id) => EL(`countSpan-${id}`);

function incrementCartItemCount(id) {
    updateBackend(id, '/Cart?handler=IncrementCartItemCount')
};

function decrementCartItemCountOrDeleteIfZero(id) {
    updateBackend(id, '/Cart?handler=DecrementCartItemCountOrDeleteIfZero')
};

const updateBackend = (id, url) => {
    $.ajax({
        type: 'POST',
        url: url, 
        data: { itemId: id },     
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() 
        },
        success: function (response) {
            location.reload();
        },
        error: function (xhr, status, error) {
            console.log("Error while removing item: ", error);
        }
    });
}   