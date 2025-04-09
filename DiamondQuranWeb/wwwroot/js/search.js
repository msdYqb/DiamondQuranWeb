window.onscroll = function () { searchDiv() };
var header = document.getElementById("searchDiv");
var sticky = header.offsetTop;
function searchDiv() {
    if (window.pageYOffset > sticky) {
        header.classList.add("stickySearchInput");
    } else {
        header.classList.remove("stickySearchInput");
    }
}

function pagetextSearchTypeChanged() {
    $.cookie('searchQuranTextSearchType', document.getElementById('quranTextSearchType').selectedIndex);
}

if (window.performance && window.performance.navigation.type === window.performance.navigation.TYPE_BACK_FORWARD) {
    document.getElementById("quranTextSearchType").selectedIndex = $.cookie('searchQuranTextSearchType');
}