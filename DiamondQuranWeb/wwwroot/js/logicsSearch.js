add("and");
add("or");
add("not");
add("andNot1");
add("andNot2");
function createGrid(selectElement, inputElement) {
    let div = document.createElement("div");

    let row = document.createElement("div");
    row.setAttribute("class", "row");

    let leftCol = document.createElement("div");
    leftCol.setAttribute("class", "col-12 col-sm-6");
    leftCol.append(selectElement);
    row.append(leftCol);

    let rightCol = document.createElement("div");
    rightCol.setAttribute("class", "col-12 col-sm-6");
    rightCol.append(inputElement);
    row.append(rightCol);

    let row2 = document.createElement("div");
    row2.setAttribute("class", "row");

    let leftColBtm = document.createElement("div");
    leftColBtm.setAttribute("class", "col-12 col-sm-6 d-none d-sm-block");
    leftColBtm.append(document.createElement("hr"));
    row2.append(leftColBtm);

    let rightColBtm = document.createElement("div");
    rightColBtm.setAttribute("class", "col-12 col-sm-6 d-none d-sm-block");
    rightColBtm.append(document.createElement("hr"));
    row2.append(rightColBtm);

    div.append(row);
    div.append(row2);
    return div;
}
function createSelect(logic) {
    let selectElement = document.createElement("select");
    selectElement.setAttribute("class", logic + "Select logicsSelect");
    let i = 0;
    for (var e of searchOptions) {
        var optionElement = document.createElement("option");
        optionElement.appendChild(document.createTextNode(e));
        optionElement.setAttribute("value", (i++).toString());
        selectElement.appendChild(optionElement);
    }
    return selectElement;
}

function changeAutoComplete(quranTextType) {
    $.ajax({
        url: "/Home/ChangeAutoComplete",
        type: "POST",
        dataType: "json",
        data: { quranTextType: quranTextType.selectedIndex },
    });
}

let widthValue;
function add(logic) {
    let div = document.getElementById(logic + "Div");
    let inputElement = document.createElement("input");
    inputElement.setAttribute("type", "text");
    inputElement.setAttribute("class", logic + "Input logicsInput form-control madb-utocomplete searchInput");
    inputElement.setAttribute("style", "display:block; float:right");
    let selectElement = createSelect(logic);
    div.append(createGrid(selectElement, inputElement));
    $("input.searchInput").autocomplete({
        minLength: 2,
        open: function (event) {
            $('.ui-autocomplete').css("width", event.target.getBoundingClientRect().width + "px");
        },
        source: function (request, response) {
            $.ajax({
                url: "/Home/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { keyword: request.term, quranTextType: document.getElementById('quranTextSearchType').selectedIndex },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item, value: item };
                    }))
                }
            })
        },
    });
}
function remove(logic) {
    let div = document.getElementById(logic + "Div");
    if (div.getElementsByClassName(logic + "Input").length > 1) {
        div.lastChild.remove();
    }
}
function search(logic) {
    let inputs2, selects2, prefix = logic;
    let andWithinSearch = document.getElementById('andWithinSearch').checked;
    let andNotWithinSearch = document.getElementById('andNotWithinSearch').checked;
    if (logic == "andNot") {
        prefix = "andNot1";
        inputs2 = document.getElementsByClassName("andNot2" + "Input");
        selects2 = document.getElementsByClassName("andNot2" + "Select");
    }
    let inputs = document.getElementsByClassName(prefix + "Input");
    let selects = document.getElementsByClassName(prefix + "Select");
    let selectedOptions1 = [];
    let selectedOptions2 = [];
    let noInput = true;
    for (let i = 0; i < inputs.length; i++) {
        if (!isNullOrWhitespace(inputs[i].value)) {
            noInput = false;
            selectedOptions1.push({ Keyword: inputs[i].value, SearchOption: selects[i].value });
        }
    }
    if (noInput) {
        return;
    }
    if (logic == "andNot") {
        noInput = true;
        for (let i = 0; i < inputs2.length; i++) {
            if (!isNullOrWhitespace(inputs2[i].value)) {
                noInput = false;
                selectedOptions2.push({ Keyword: inputs2[i].value, SearchOption: selects2[i].value });
            }
        }
    }
    if (noInput) {
        return;
    }
    let logicsSearchSetting = {
        LogicSearchChoice: logic,
        SelectedOptions1: selectedOptions1,
        SelectedOptions2: selectedOptions2,
        Within: logic == "andNot" ? andNotWithinSearch : andWithinSearch,
    };
    post(logicsSearchSetting);
}
function post(logicsSearchSetting) {
    $.ajax({
        url: "/Home/LogicsSearchAjaxPost",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: JSON.stringify({ logicsSearchSetting: logicsSearchSetting }),
        success: function (data) {
            window.location.href = data.redirectToUrl;
        }
    });
}

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

document.onload = onload();
function onload() {
    let selected = document.getElementById('quranTextSearchType');
    changeAutoComplete(selected)
}

$('#wordsSearch').on('change.bootstrapSwitch', function (e) {
    if (e.target.checked) {
        document.getElementById("quranTextResultType").disabled = true;
    }
    else {
        document.getElementById("quranTextResultType").disabled = false;
    }
});

function pagetextSearchTypeChanged() {
    $.cookie('logicsQuranTextSearchType', document.getElementById('quranTextSearchType').selectedIndex);
}

if (window.performance && window.performance.navigation.type === window.performance.navigation.TYPE_BACK_FORWARD) {
    document.getElementById("quranTextSearchType").selectedIndex = $.cookie('logicsQuranTextSearchType');
}