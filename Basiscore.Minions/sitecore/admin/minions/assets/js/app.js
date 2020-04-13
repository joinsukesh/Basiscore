$(document).ready(function () {
    $(".moduleName").html($(".hdnModuleName").val());
    app.HighlightActiveMenu();


});

var app = {
    scrollDuration: 1000,
    modalShowDelay: 3000,

    StringNullOrEmpty(value) {
        return value == null || value == undefined || (value.trim()).length <= 0;
    },

    ShowLoadingModal() {
        ///get & fill the random wait message
        var randomWaitMessage = GetRandomLoadingMessage();
        $("#pWaitMessage").html(randomWaitMessage);
        $("#mdlLoad").modal("show");
    },

    HideLoadingModal() {
        $("#pWaitMessage").html("");
        $("#mdlLoad").modal("hide");  
    },

    HighlightActiveMenu() {
        var currentRelativeUrl = window.location.pathname;
        var relatedAnchorTag = $('.toplevelcontainer a[href="' + currentRelativeUrl + '"]')[0];

        if (relatedAnchorTag != null && relatedAnchorTag != undefined) {
            var toplevelcontainer = $(relatedAnchorTag).closest(".toplevelcontainer");
            var menuItem = $(relatedAnchorTag).closest(".menuItem");
            if (toplevelcontainer != null && toplevelcontainer != undefined) {
                $(toplevelcontainer).addClass("show");
            }
            if (menuItem != null && menuItem != undefined) {
                $(menuItem).addClass("active");
            }
        }
    }
};

