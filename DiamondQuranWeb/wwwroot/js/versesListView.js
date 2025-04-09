function getTafsir(ayahId) {
    $.ajax({
        url: "/Home/GetTafsir",
        type: "get",
        data: { ayahId: ayahId },
        success: function (data) {
            document.getElementById('tafsirPopup').innerHTML = data.tafsir;
            document.getElementById("tafsir").name = data.ayahId;
            $('.spinner-border').css('display', 'none');
        }
    });
}
function removeFavorite(favoriteId, favoriteListId) {
    $('#removeFavSpinner-' + favoriteId).css('display', 'inline-block');
    $('#removeFavBtn-' + favoriteId).addClass('btn-disable');
    $.ajax({
        url: "/Home/RemoveFavorite",
        type: "post",
        data: { favoriteId: favoriteId, favoriteListId: favoriteListId },
        success: function (data) {
            if (data.success) {
                $('#block-' + data.favoriteId).remove();
            } else {
                $('#removeFavSpinner-' + data.favoriteId).css('display', 'none');
                $('#removeFavBtn-' + data.favoriteId).removeClass('btn-disable');
            }
        }
    });
}
async function getComment(ayahId) {
    let user = await isAuthenticated()
    if (!user.isAuthenticated) {
        window.location = "/Account/Login";
    }
    else {
        $('#commentModal').modal('show');
        $.ajax({
            url: "/Home/GetComment",
            type: "get",
            data: { ayahId: ayahId },
            success: function (data) {
                document.getElementById("commentText").value = data.comment;
                $('.spinner-border').css('display', 'none');
                $('#saveCommentBtn').attr('name', data.ayahId);
                $('#deleteCommentBtn').attr('name', data.ayahId);
                $('#favTable').parents('div.dataTables_wrapper').first().show();
            }
        });
    }
}

function saveComment(ayahId) {
    let commentText = document.getElementById("commentText").value;
    $.ajax({
        url: "/Home/AddOrEditComment",
        type: "POST",
        dataType: "json",
        data: { ayahId: ayahId, commentText: commentText },
        success: function (data) {
            alert(data.msg);
            $('#comment-' + ayahId).css('visibility', 'visible');
        }
    });

}
function deleteComment(ayahId) {
    $.ajax({
        url: "/Home/DeleteComment",
        type: "POST",
        dataType: "json",
        data: { ayahId: ayahId },
        success: function (data) {
            if (data.success) {
                document.getElementById("commentText").value = "";
                $('#comment-' + ayahId).css('visibility', 'hidden');
            }
            alert(data.msg);
        }
    });
}
function favoriteButton(listName, reference, listId) {
    return `<div class="row">
                        <div class="col-6">
                            <button class="btn btn-primary" id="addFavBtn-${listId}" onclick="addFavorite(${reference})" type="button" style="display:inline-flex; align-items: center">
                                اضافة
                                <span id="addFavSpinner-${listId}" class="spinner-border spinner-border-sm" role="status" aria-hidden="true" style="display:none"></span>
                            </button>
                            <button class="btn btn-primary" id="addAllFavBtn-${listId}" onclick="addAllFavorite(${listId})" type="button" style="display:inline-flex; align-items: center">
                                اضافة الجميع
                                <span id="addAllFavSpinner-${listId}" class="spinner-border spinner-border-sm" role="status" aria-hidden="true" style="display:none"></span>
                            </button>
                                <i id="successIcon-${listId}" style="display:inline-flex; align-items: center"></i>
                        </div>
                        <div class="col-6">
                                <p>${listName}</p>
                        </div>
                        </div>`;
}
async function getFavorites(ayahId) {
    let user = await isAuthenticated()
    if (!user.isAuthenticated) {
        window.location = "/Account/Login";
    }
    else {
        $('#favoritesModal').modal('show');
        $.ajax({
            url: "/Home/GetFavorites",
            type: "get",
            data: { ayahId: ayahId },
            success: function (data) {
                getFavoritesSuccess(data);
            }
        });
    }
}

function getFavoritesSuccess(favorites) {
    let favoritesList = [];
    for (var e of favorites.favoritesString.split(',')) {
        if (!isNullOrWhitespace(e)) {
            favoritesList.push([e]);
        }
    }
    $('#favTable').DataTable({
        data: favoritesList,
        columns: [
            { title: "ListName" },
        ],
        columnDefs: [
            {
                targets: 0,
                render: function (data, type, row, meta) {
                    let listName = data.split('-')[0];
                    let listId = data.split('-')[1];
                    //console.log(listId);
                    let reference = `'${favorites.ayahId}','${listId}'`;
                    return favoriteButton(listName, reference, listId);
                }
            },
        ],
        language: {
            "info": "قائمة _TOTAL_",
            "lengthMenu": "_MENU_",
            "search": "",
            "paginate": {
                "first": "الاول",
                "last": "الاخير",
                "next": "التالي",
                "previous": "السابق"
            }
        },
        destroy: true,
    });
    $('.spinner-border').css('display', 'none');
}

$('.modal').on('show.bs.modal', function (e) {
    $('.spinnerPopup').css('display', 'inline-block');
    if (e.target.id == "tafsirModal") { document.getElementById("tafsirPopup").innerHTML = ""; }
})

function addFavorite(ayahId, listId) {
    $('#addFavSpinner-' + listId).css('display', 'inline-block');
    $('#addFavBtn-' + listId).attr('disabled', 'disabled');
    $('#addAllFavBtn-' + listId).attr('disabled', 'disabled');
    $('#successIcon-' + listId).removeClass('fas fa-check fa-times');
    $.ajax({
        url: "/Home/AddFavorite",
        type: "POST",
        dataType: "json",
        data: { ayahId: ayahId, listId: listId },
        success: function (data) {
            $('#addFavSpinner-' + listId).css('display', 'none');
            $('#addFavBtn-' + listId).removeAttr('disabled');
            $('#addAllFavBtn-' + listId).removeAttr('disabled');
            if (data.success) {
                $('#successIcon-' + listId).addClass('fas fa-check');
                $('#favorite-' + ayahId).css('visibility', 'visible');
            }
            else { $('#successIcon-' + listId).addClass('fas fa-times'); }
        }
    });
}
function addAllFavorite(listId) {
    $('#addAllFavSpinner-' + listId).css('display', 'inline-block');
    $('#addAllFavBtn-' + listId).attr('disabled', 'disabled');
    $('#addFavBtn-' + listId).attr('disabled', 'disabled');
    $('#successIcon-' + listId).removeClass('fas fa-check fa-times');
    let versesId = [];
    for (var e of document.getElementsByClassName("resultParagraph")) {
        versesId.push(e.id);
    }
    $.ajax({
        url: "/Home/AddAllFavorite",
        type: "POST",
        dataType: "json",
        data: { versesId: versesId, listId: listId },
        success: function (data) {
            $('#addAllFavSpinner-' + listId).css('display', 'none');
            $('#addAllFavBtn-' + listId).removeAttr('disabled');
            $('#addFavBtn-' + listId).removeAttr('disabled');
            if (data.success) {
                $('#successIcon-' + listId).addClass('fas fa-check');
                $('.fa-star').css('visibility', 'visible');
            }
            else { $('#successIcon-' + listId).addClass('fas fa-times'); }
        }
    });
}

function changeSearchResultTextType(quranTextType) {
    let versesIds = [];
    for (var verse of document.getElementsByClassName("resultParagraph")) {
        versesIds.push(verse.id);
    }
    $.ajax({
        url: "/Home/ChangeSearchResultTextType",
        type: "POST",
        dataType: "json",
        data: { versesIds: versesIds, quranTextType: quranTextType },
        success: function (data) {
            for (var e of data.verses) {
                $('#' + e.ID).replaceWith(`<p id="${e.ID}" class="resultParagraph">${e.AyahText}</p>`);
            }
        }
    });
}
function changeTafsir(verseId, tafsir) {
    $.ajax({
        url: "/Home/ChangeTafsir",
        type: "POST",
        dataType: "json",
        data: { verseId: verseId, tafsir: tafsir },
        success: function (data) {
            document.getElementById('tafsirPopup').innerHTML = data.tafsirText;
        }
    });
}

$('.copyBtn').tooltip({
    animated: 'fade',
    placement: 'bottom',
    trigger: 'click'
});

$('.copyBtn').mouseleave(function () {
    $(this).tooltip('hide');
});

function copyToClipboard(element) {
    element = "#" + element;
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val($(element).text().trim()).select();
    document.execCommand("copy");
    $temp.remove();
}
$("#tafsirModal").on("hidden.bs.modal", function () {
    document.getElementById("tafsir").value = 0;
});

$("#favoritesModal").on("hidden.bs.modal", function () {
    $('#favTable').parents('div.dataTables_wrapper').first().hide();
});

$("#commentModal").on("hidden.bs.modal", function () {
    document.getElementById("commentText").value = "";
});