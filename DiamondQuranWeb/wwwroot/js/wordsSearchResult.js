function searchByWords() {
    let quranTextSearchType = getParameterByName("QuranTextSearchType", window.location.search);
    let checkedList = [];
    for (let e of document.getElementsByClassName('word-switch')) {
        if (e.checked) {
            checkedList.push({ word: e.id });
        }
    }
    if (checkedList.length == 0) { return; }
    $.ajax({
        url: "/Home/SearchBySelectedWords",
        type: "POST",
        dataType: "json",
        data: { quranTextSearchType: quranTextSearchType, selectedWords: checkedList },
        success: function (data) {
            window.location.href = data.redirectToUrl;
        }
    });
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

$('#selectAllWords').on('change.bootstrapSwitch', function (e) {
    for (let wordSwitch of document.getElementsByClassName('word-switch')) {
        wordSwitch.checked = e.target.checked;
    }
});

$('.word-switch').on('change.bootstrapSwitch', function (e) {
    if (!e.target.checked) {
        document.getElementById("selectAllWords").checked = false;
    }
});
