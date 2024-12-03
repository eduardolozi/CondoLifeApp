function getToken() {
    return localStorage.getItem('accessToken');
}

function getTimeZone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}