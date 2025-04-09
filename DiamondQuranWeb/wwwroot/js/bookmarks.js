
function deleteComment(data) {
    $('#deleteCommentSpinner-' + data.commentId).css('visibility', 'hidden');
    if (data.success) {
        $('#block-' + data.commentId).remove()
    }
    else {
        $('#successIcon-' + data.commentId).addClass('fas fa-times');
    }
}

let comment;
function editComment(id) {
    $("#editCommentBtn-" + id).attr('disabled', 'disabled');
    comment = document.getElementById('comment-' + id).innerText.trim();
    var textareaAttrs = { "id": `commentTextarea-${id}`, "class": "commentTextarea", "value": comment, "style": "float:right" };
    $("#comment-" + id).replaceWith(function () {
        return $(`<textarea>`, textareaAttrs).append($(this).contents());
    });
    //setTimeout(function () {
    //    $("#commentTextarea-" + id).focus();
    //}, 0);
    $("#commentTextarea-" + id).focus();
}

$(document).on('focusout', '.commentTextarea', function (e) {
    let id = e.target.id.split('-')[1];
    let textarea = "#" + e.target.id;
    if ($(this).val() != comment && !isNullOrWhitespace($(this).val())) {
        let newComment = $(this).val();
        $(textarea).attr('disabled', 'disabled');
        $('#editCommentSpinner-' + id).css('visibility', 'visible');
        $.ajax({
            url: "/Account/EditComment",
            type: "POST",
            dataType: "json",
            data: { commentId: id, comment: newComment },
            success: function (data) {
                $("#editCommentBtn-" + id).removeAttr('disabled');
                $('#editCommentSpinner-' + id).css('visibility', 'hidden');
                if (data.success) {
                    $('#commentEditSuccessIcon-' + id).addClass('fas fa-check');
                    $(textarea).removeAttr('disabled');
                    $(textarea).replaceWith(`<p id="comment-${id}">${newComment}</p>`);
                }
                else {
                    $('#commentEditSuccessIcon-' + id).addClass('fas fa-times');
                    $(textarea).removeAttr('disabled');
                    $(textarea).replaceWith(`<p id="comment-${id}">${comment}</p>`);
                }
            }
        });
    }
    else {
        $("#editCommentBtn-" + id).removeAttr('disabled');
        $(this).replaceWith(`<p id="comment-${id}">${comment}</p>`);
    }
});

function deleteCommentClicked(commentId) {
    $('#deleteCommentSpinner-' + commentId).css('visibility', 'visible');
    $.ajax({
        url: "/Account/DeleteComment",
        type: "post",
        data: { commentId: commentId },
        success: function (data) {
            deleteComment(data);
        }
    });
}
function deleteFavoriteListClicked(favoriteId) {
    $('#deleteSpinner-' + favoriteId).css('display', 'inline-block');
    $.ajax({
        url: "/Account/DeleteFavoriteList",
        type: "post",
        data: { favoriteId: favoriteId },
        success: function (data) {
            deleteFavoriteList(data);
        }
    });
}
function deleteFavoriteList(data) {
    $('#deleteSpinner-' + data.favoriteId).css('display', 'none');
    if (data.success) {
        $('#favRow-' + data.favoriteId).remove()
    }
    else {
        $('#successIcon-' + data.favoriteId).addClass('fas fa-times');
    }
}

let listName;
function editFavoriteList(id) {
    $("#editFavoriteBtn-" + id).attr('disabled', 'disabled');
    listName = document.getElementById('listNameAnchor-' + id).innerText;
    var inputAttrs = { "id": `listNameInput-${id}`, "class": "favoriteInput", "type": "text", "value": listName, "style": "float:right" };
    $("#listNameAnchor-" + id).replaceWith(function () {
        return $(`<input />`, inputAttrs).append($(this).contents());
    });
    $("#listNameInput-" + id).focus();
}
$(document).on('focusout', '.favoriteInput', function (e) {
    let id = e.target.id.split('-')[1];
    let inputId = "#" + e.target.id;
    let anchor = `<a id="listNameAnchor-${id}" href="/Account/Favorites?favoriteListId=${id}" style="float:right">${listName}</a>`
    if ($(this).val() != listName) {
        let newListName = $(this).val();
        $(inputId).attr('disabled', 'disabled');
        $('#editSpinner-' + id).css('visibility', 'visible');
        $.ajax({
            url: "/Account/EditFavoriteList",
            type: "POST",
            dataType: "json",//TODO add error response .
            data: { favoriteId: id, listName: newListName },
            success: function (data) {
                $("#editFavoriteBtn-" + id).removeAttr('disabled');
                $('#editSpinner-' + id).css('visibility', 'hidden');
                if (data.success) {
                    $('#successIcon-' + id).addClass('fas fa-check');
                    $(inputId).removeAttr('disabled');
                    $(inputId).replaceWith(`<a id="listNameAnchor-${id}" href="/Account/Favorites?favoriteListId=${id}" style="float:right">${newListName}</a>`);
                }
                else {
                    $('#successIcon-' + data.favoriteId).addClass('fas fa-times');
                    $(inputId).replaceWith(anchor);
                }
            }
        });
    }
    else {
        //let anchorAttribute = { "id": `listNameAnchor-${id}`, "href": `/Account/Favorites?favoriteListId=${id}`, "style": "float:right", "value": `${listName}` };
        $("#editFavoriteBtn-" + id).removeAttr('disabled');
        $(this).replaceWith(anchor);
    }
});