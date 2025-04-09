function textSearchTypeChanged() {
    pagetextSearchTypeChanged();
    let selected = document.getElementById('quranTextSearchType');
    changeAutoComplete(selected)
    if (selected.value.includes("Normal")) {
        $(".Uthmani").attr("disabled", "disabled");
        $(".Normal").removeAttr("disabled");
        if (!document.getElementById("quranTextResultType").value.includes("Normal")) {
            document.getElementById("quranTextResultType").selectedIndex = 0;
        }
    }
    else {
        $(".Normal").attr("disabled", "disabled");
        $(".Uthmani").removeAttr("disabled");
        if (!document.getElementById("quranTextResultType").value.includes("Uthmani")) {
            document.getElementById("quranTextResultType").selectedIndex = 3;
        }
    }
}

function changeAutoComplete(quranTextType) {
    $.ajax({
        url: "/Home/ChangeAutoComplete",
        type: "POST",
        dataType: "json",
        data: { quranTextType: quranTextType.selectedIndex },
    });
}
function searchClicked() {
    let wordsSearchChecked = document.getElementById('wordsSearch').checked;
    let keyword = cleanKeyword(keywordInput.value);
    let quranTextSearchType = document.getElementById('quranTextSearchType').selectedIndex
    window.location.href = "/" + (wordsSearchChecked ? "WordsSearchResult" : "Search")
        + "?Keyword=" + keyword
        + "&SearchOption=" + document.getElementById('searchOption').value
        + "&QuranTextSearchType=" + quranTextSearchType
        + (wordsSearchChecked ? "" : "&QuranTextResultType=" + document.getElementById('quranTextResultType').selectedIndex);
}

document.onload = onload();
function onload() {
    //textSearchTypeChanged();
}

$('#wordsSearch').on('change.bootstrapSwitch', function (e) {
    if (e.target.checked) {
        document.getElementById("quranTextResultType").disabled = true;
    }
    else {
        document.getElementById("quranTextResultType").disabled = false;
    }
});

let keywordInput = document.getElementById("form-autocomplete");

keywordInput.addEventListener("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        searchClicked()
    }
});

$(document).ready(function () {
    $("#form-autocomplete").autocomplete({
        minLength: 2,
        open: function () {
            var width = document.getElementsByClassName("searchInput")[0].getBoundingClientRect().width;
            $('.ui-autocomplete').css("width", width + "px");
        },
        source: function (request, response) {
            let keyword = cleanKeyword(request.term);
            $.ajax({
                url: "/Home/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { keyword: keyword, quranTextType: document.getElementById('quranTextSearchType').selectedIndex },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item, value: item };
                    }))
                }
            })
        },
    });
});

function displayKeyboard() {
    const display = document.querySelector('.simple-keyboard').style.display;
    if (display == "none" || !display) {
        $('.simple-keyboard').css("display", "block");
    }
    else {
        $('.simple-keyboard').css("display", "none");
    }
}

//ِ ْ ٱ ّ َ ٰ ُ ٓ ً ۟ ٌ ۢ ۥ ٍ ۦ ـ ٔ ۭ ۜ ۠ ۪ ۫ ۨ ۬ ۣ
//ِ ْ ّ َ ُ ً ٌ ٍ
const arabic = {
    default: [
        "ذ ِ ْ ّ َ ُ ً ٌ ٍ - = {bksp}",
        "{tab} ض ص ث ق ف غ ع ه خ ح ج د \\",
        "{lock} ش س ي ب ل ا ت ن م ك ط {enter}",
        "{shift} ئ ء ؤ ر لا ى ة و ز ظ {shift}",
        ".com @ {space}"],
    shift: ["ّ ! @ # $ % ^ & * ) ( _ + {bksp}",
        "{tab} َ ً ُ ٌ لإ إ ‘ ÷ × ؛ < > |",
        '{lock} ِ ٍ ] [ لأ أ ـ ، / : " {enter}',
        "{shift} ~ ْ } { لآ آ ’ , . ؟ {shift}",
        ".com @ {space}"]
};

let Keyboard = window.SimpleKeyboard.default;

let keyboard = new Keyboard({
    onChange: input => onChange(input),
    onKeyPress: button => onKeyPress(button),
    layout: arabic
});

document.querySelector(".input").addEventListener("input", event => {
    keyboard.setInput(event.target.value);
});

function onChange(input) {
    document.querySelector(".input").value = input;
}

function onKeyPress(button) {
    if (button === "{shift}" || button === "{lock}") handleShift();
}

function handleShift() {
    let currentLayout = keyboard.options.layoutName;
    let shiftToggle = currentLayout === "default" ? "shift" : "default";

    keyboard.setOptions({
        layoutName: shiftToggle
    });
}

//keyboard first button
//$("[data-skbtnuid='default-r0b1']").css("font-size", "7vh").css("font-weight", "bold").css("padding-bottom", "25px").css("padding-right", "25px");

