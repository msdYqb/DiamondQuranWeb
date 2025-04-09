function isNullOrWhitespace(input) {
    if (typeof input === 'undefined' || input == null) return true;
    return input.replace(/\s/g, '').length < 1;
}

function cleanKeyword(keyword) {
    keyword = removeWhiteSpace(keyword)
    let quranTextSearchType = document.getElementById('quranTextSearchType').selectedIndex
    switch (quranTextSearchType) {
        case 0:
        case 3:
            keyword = removeAlefGrammers(keyword);
            keyword = removeGrammers(keyword);
        case 1:
        case 4:
            keyword = removeGrammers(keyword);
    }
    return keyword;
}

function removeWhiteSpace(keyword) {
    let cleanKeyword = "";
    for (var e of keyword.split(' ')) {
        if (!isNullOrWhitespace(e)) {
            cleanKeyword += e + ' ';
        }
    }
    cleanKeyword = cleanKeyword.trim();
    return cleanKeyword.trim();
}

function removeGrammers(keyword) {
    let cleanKeyword = "";
    let acceptedLetters = ["ئ", "ء", "ؤ", "ى", "ة", "ا", "ب", "ت", "ث", "ج", "ح", "خ", "د", "ذ", "ر", "ز", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ك", "ل", "م", "ن", "ه", "و", "ي", " "];
    let quranTextSearchType = document.getElementById('quranTextSearchType').selectedIndex
    if (quranTextSearchType == 1 || 4) {
        acceptedLetters.push("أ");
        acceptedLetters.push("آ");
        acceptedLetters.push("إ");
    }
    for (let letter of keyword.split('')) {
        if (acceptedLetters.includes(letter)) {
            cleanKeyword += letter;
        }
    }
    return cleanKeyword;
}

function removeAlefGrammers(keyword) {
    return keyword.replaceAll("أ", "ا").replaceAll("آ", "ا").replaceAll("إ", "ا");
}

$(function () {
    $('[data-toggle="popover"]').popover({
        sanitize: false,
    })
})

$(".btn").click(function () {
    $(this).trigger("blur");
});

async function isAuthenticated() {
    return $.ajax({
        url: "/Account/IsAuthenticated",
        type: "get"
    });
}

function isDeviceiPad() {
    return navigator.platform.match(/iPad/i);
}

if (isDeviceiPad()) {
    //$('#layoutHeader').css('height', '500px');
}

$("html").on("mouseup", function (e) {
    var l = $(e.target);
    if (l[0].className.indexOf("popover") == -1) {
        $(".popover").each(function () {
            $(this).popover("hide");
        });
    }
});