const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(()=> {
    console.log("SignalR Connected")
}).catch(err => console.error(err.toString()));


connection.on("NotifyClient", (message, link, nameOfLink) => {
    // Assign alert message
    const alertMessageEL =  document.getElementById("alertMessage");
    if(alertMessageEL)
    {
        alertMessageEL.textContent = message;
    }

    // Make the alert box visible
    const notifyBoxEL = document.getElementById("notifyBox");
    if(notifyBoxEL)
    {
        notifyBoxEL.style.display = "block";
    }

    const notifyLinkEL = document.getElementById("notifyLink");
    if(link && nameOfLink && notifyLinkEL)
    {  
        notifyLinkEL.style.display = "block";

        notifyLinkEL.href = link;
        notifyLinkEL.textContent = nameOfLink;
    }

    console.log("message:", message, "link", link, "nameOfLink", nameOfLink);
});

// These functions are used to send a user from to the link or force a reload of the current page
// and serve as a way to reload the page for everyone in the SignalR groups
connection.on("RedirectToLink", (link) => {
    window.location.href = link
});

// Pretty much serves the same purpose as above just by reloading the current page instead of redirecting
connection.on("ReloadPage", () => {
    window.location.reload();
});