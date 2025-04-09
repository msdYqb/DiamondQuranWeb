$(function () {
    $("#surahList").change(function () {
        let surahNumber = $(this).val();
        let data = { quran: { SurahNumber: surahNumber } };
        post(data, "ChangeSurah");
    });
    $("#ayahList").change(function () {
        let ayahNumber = $(this).val();
        let surahNumber = document.getElementById("surahList").value;
        let data = { quran: { SurahNumber: surahNumber, AyahNumber: ayahNumber } };
        post(data, "ChangeAyah");
    });
    function post(data, url) {
        $.ajax({
            url: "/Home/" + url,
            type: "POST",
            dataType: "json",
            data: data,
            success: function (data) {
                window.location.href = data.redirectToUrl;
            }
        });
    }
});