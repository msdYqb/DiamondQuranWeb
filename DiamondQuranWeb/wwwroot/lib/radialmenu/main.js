'use strict';
var verseId;
var menuItems = [
    {
        id: 'comment',
        title: 'تعليق',
        icon: '#comment'
    },
    {
        id: 'favorite',
        title: 'المفضلة',
        icon: "#favorite"
    },
    {
        id: 'tafsirRadialBtn',
        title: 'تفسير',
        icon: "#tafsirIcon"
    },
    {
        id: 'copy',
        title: 'نسخ',
        icon: "#copy"
    }
];

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var svgMenu;
window.onload = async function () {
    svgMenu = new RadialMenu({
        parent: document.getElementById("radialMenu"),
        size: 400,
        closeOnClick: true,
        menuItems: menuItems,
        onClick: async function (item) {
            let user = await isAuthenticated()
            switch (item.id) {
                case "comment":
                    if (!user.isAuthenticated) {
                        window.location = "/Account/Login";
                        return
                    }
                    $('#commentModal').modal('show');
                    getComment(verseId);
                    break;
                case "favorite":
                    if (!user.isAuthenticated) {
                        window.location = "/Account/Login";
                        return
                    }
                    $('#favoritesModal').modal('show');
                    getFavorites(verseId);
                    break;
                case "tafsirRadialBtn":
                    $('#tafsirModal').modal('show');
                    getTafsir(verseId);
                    break;
                case "copy":
                    copyToClipboard(verseId);
                    break;
            }
        }
    });
};