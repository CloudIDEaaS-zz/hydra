function openTab(evt, tabName) {
    // Declare all variables
    var i, tabcontent, tablinks;

    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tabcontent");

    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");

    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";

    setCookie("activeTab", btoa(tabName));
}

document.onreadystatechange = function () {

    if (document.readyState === 'complete') {

        var tabcontent, tablink, activeTabId;
        var activeTab = getCookie("activeTab");

        if (activeTab != "") {

            activeTabId = atob(activeTab);
            tabcontent = document.getElementById(activeTabId);
            tablink = document.getElementsByName(activeTabId)[0];

            if (!tabcontent) {
                tabcontent = document.getElementsByClassName("tabcontent")[0];
                tablink = document.getElementsByClassName("tablinks")[0];
            }
        }
        else {
            tabcontent = document.getElementsByClassName("tabcontent")[0];
            tablink = document.getElementsByClassName("tablinks")[0];
        }

        if (tabcontent) {

            tabcontent.style.display = "block";
            tablink.className += " active";
        }
    }
}

function setCookie(name, value) {

    var d = new Date();
    var expires

    d.setTime(d.getTime() + (365 * 24 * 60 * 60 * 1000));
    expires = "expires=" + d.toUTCString();

    if (localStorage) {
        localStorage.setItem(name, value);
    }
    else {
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    }
}

function getCookie(cname) {

    if (localStorage) {
        return localStorage.getItem(cname);
    }
    else {
        var name = cname + "=";
        var ca = document.cookie.split(';');

        for (var i = 0; i < ca.length; i++) {

            var c = ca[i];

            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }

            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
    }

    return "";
}
