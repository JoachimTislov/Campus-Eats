const EL = (Id) => document.getElementById(Id);

const loginOptBtn = EL("loginOpt");
const registerOptBtn = EL("registerOpt");

const loginForm = EL("loginForm");
const registerForm = EL("registerForm");

if(loginOptBtn && registerOptBtn)
{
    loginOptBtn.addEventListener('click', function () {
        adjustFormVisibility(loginForm, registerForm)
        adjustActiveButtons(loginOptBtn, registerOptBtn)
    });

    registerOptBtn.addEventListener('click', function () {
        adjustFormVisibility(registerForm, loginForm)
        adjustActiveButtons(registerOptBtn, loginOptBtn)
    });
}

const adjustActiveButtons = (bToActivate, bToDisabled) => manipulateClasslist("active", bToDisabled, bToActivate);
const adjustFormVisibility = (formToShow, formToHide) => manipulateClasslist("hide", formToShow, formToHide);

const manipulateClasslist = (className, divToRemoveClass, divToAddClass) =>
{
    if (divToRemoveClass.classList.contains(className))
    {
        divToRemoveClass.classList.remove(className);
    } 

    divToAddClass.classList.add(className);
}



