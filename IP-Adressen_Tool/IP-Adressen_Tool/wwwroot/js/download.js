
window.dateiHerunterladen = function (dateiname, inhalt, mime) {
    const blob = new Blob([inhalt], { type: mime });
    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = dateiname;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
};
