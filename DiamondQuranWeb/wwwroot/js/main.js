let rotated = false;
function rotate() {
    if (!rotated) {
        $('#arrow').css("transform", "rotate(180deg)");
        rotated = true;
    }
    else {
        $('#arrow').css("transform", "rotate(360deg)");
        rotated = false;
    }
}

function pagetextSearchTypeChanged() {
    $.cookie('mainQuranTextSearchType', document.getElementById('quranTextSearchType').selectedIndex);
}

if (window.performance && window.performance.navigation.type === window.performance.navigation.TYPE_BACK_FORWARD) {
    document.getElementById("quranTextSearchType").selectedIndex = $.cookie('mainQuranTextSearchType');
}