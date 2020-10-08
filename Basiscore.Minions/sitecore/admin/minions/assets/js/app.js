$(document).ready(function () {
    $(".moduleName").html($(".hdnModuleName").val());
    app.HighlightActiveMenu();

    /*--SECTION SUMMARY EXPAND COLLAPSE ICONS--*/
    $(".summary-panel .panel-collapse").on("show.bs.collapse", function () {        
        $(this).siblings(".anc-summary-panel-heading-section").addClass("active");
    });

    $(".summary-panel .panel-collapse").on("hide.bs.collapse", function () {
        $(this).siblings(".anc-summary-panel-heading-section").removeClass("active");        
    });
    /*--END SECTION SUMMARY EXPAND COLLAPSE ICONS--*/

    /*LANGUAGE CHECKBOX SECTION*/
    $("#chkAllLanguages").change(function () {
        if ($(this).is(":checked")) {
            $("#chkLanguages input[type='checkbox']").prop("checked", "checked");
        }
        else {
            $("#chkLanguages input[type='checkbox']").prop("checked", "");
        }
    });

    $(".chkLanguage").change(function () {
        if (!$(this).is(":checked")) {
            $("#chkAllLanguages").prop("checked", "");
        }
    });
    /*END LANGUAGE CHECKBOX SECTION*/

    
});

var app = {
    scrollDuration: 1000,
    modalShowDelay: 3000,
    minionQuoteDelay: 2000,

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
    },

    /// Function to generate random number in a range 
    GetRandomNumber(min, max) {
        min = Math.ceil(min);
        max = Math.floor(max);
        return Math.floor(Math.random() * (max - min + 1)) + min;
    },

    ///get selected language codes
    GetSelectedLanguageCodes() {
        var selectedLanguageCodes = [];
        $("#chkLanguages input:checked").each(function () {
            selectedLanguageCodes.push($(this).attr("value"));
        });
        return selectedLanguageCodes;
    },

    /*DEFAULT RESET FORM*/
    DefaultResetForm(){
        ///hide all validation msgs
        $(".validation-msg").hide();

        ///clear all textbox values
        $("input[type='text'], textarea").each(function () {
            $(this).val("");
        });

        ///uncheck all checkboxes
        $(".chkboxlist input[type='checkbox']").prop("checked", "");
        $("#chkAllLanguages").prop("checked", "");
    }
    
};

