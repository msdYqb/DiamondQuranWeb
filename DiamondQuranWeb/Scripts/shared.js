function isNullOrWhitespace(input) {
    if (typeof input === 'undefined' || input == null) return true;
    return input.replace(/\s/g, '').length < 1;
}

function cleanKeyword(keyword) {
    let cleanKeyword = "";
    for (var e of keyword.split(' ')) {
        if (!isNullOrWhitespace(e)) {
            cleanKeyword += e + ' ';
        }
    }
    return cleanKeyword.trim();
}

$(function () {
    $('[data-toggle="popover"]').popover({
        sanitize: false,
    })
})

$(".btn").click(function () {
    $(this).trigger("blur");
});