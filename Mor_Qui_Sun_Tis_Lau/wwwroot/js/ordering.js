const tipButtons = document.querySelectorAll('#tipOptions button');
const customTipInput = document.querySelector('#tip');
const customTipContainer = document.querySelector('#customTip');

let tip = 10.00;

tipButtons.forEach(button => {
  button.addEventListener('click', () => {
    tipButtons.forEach(btn => btn.classList.remove('tipSelected'));
    button.classList.add('tipSelected');

    const dataTipValue = button.getAttribute('data-tip');

    if (dataTipValue === 'custom') {
      customTipInput.disabled = false;
      customTipContainer.classList.remove('inactive');
      customTipInput.value = 0.00;
      tip = customTipInput.value;
      customTipInput.value = "";
    }
    else{
      customTipInput.value = ""; 
      customTipInput.disabled = true;
      customTipContainer.classList.add('inactive');
      tip = parseFloat(dataTipValue).toFixed(2);
    };
  });
});

// onchange event for custom-tip
if(customTipInput)
{
  customTipInput.addEventListener('change', () => {
    const customTipValue = parseFloat(customTipInput.value);

    if (customTipValue >= 10.00 && customTipValue <= 150.00) {
      tip = customTipValue.toFixed(2);
    } 
    else{
      customTipInput.value = "";
    }
  });
}


function tipCourier(orderId, orderStatus) {
  $.ajax({
      type: 'POST',
      url: `/Ordering/${orderId}?handler=TipCourier`, 
      data: { orderStatus: orderStatus, tip: parseFloat(tip) },     
      headers: {
          'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() 
      },
      success: function (response) {
          if(response.success)
          {
            window.location.href = response.url;
          }
          else
          {
            alert(response.error);
          }
      },
      error: function (xhr, status, error) {
          console.log("Error while removing item: ", error);
      }
  });
};