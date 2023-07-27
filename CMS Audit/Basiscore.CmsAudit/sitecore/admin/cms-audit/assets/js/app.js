$(document).ready(function () {

    AppInit();

    /// Reposition when the modal is shown
    $('.modal').on('show.bs.modal', RepositionModal);

    /// Reposition when the window is resized
    $(window).on('resize', function () {
        $('.modal:visible').each(RepositionModal);
    });

    $("#btnSidebarOpen").click(function () {
        var isSidebarOpen = $(this).attr("data-isSidebarOpen");

        if (isSidebarOpen == 1) {
            document.getElementById("sidebar").style.marginLeft = "-17%";
            document.getElementById("main").style.width = "100%";
            $(this).attr("data-isSidebarOpen", 0);
        }
        else {
            document.getElementById("sidebar").style.marginLeft = "0%";
            document.getElementById("main").style.width = "83.3%";
            $(this).attr("data-isSidebarOpen", 1);
        }
    });

    $("#ddlLogTypes").change(function () {

        var logType = $("#ddlLogTypes").val();

        if (logType == 1 || logType == 2) {
            $("#txtItemId").prop("disabled", false);
        }
        else {
            $("#txtItemId").val("");
            $("#txtItemId").prop("disabled", true);
        }
    });

    $("#btnIalSubmit").click(function () {
        ClearResult();        
    });

    $('body').on('click', 'button.btnViewItemAuditLog', function () {
        var rowId = $(this).attr("data-rowid");

        var itemDataBeforeSave = $(".divIdbs-" + rowId).html();
        itemDataBeforeSave = JSON.stringify(itemDataBeforeSave, null, 4);
        itemDataBeforeSave = itemDataBeforeSave.replace(/\\n/g, "<br>").replace(/\\"/g, '"').replace(/ /g, '&nbsp;');
        $("#divMdlIdbs").html(itemDataBeforeSave);

        var itemDataAfterSave = $(".divIdas-" + rowId).html();
        itemDataAfterSave = JSON.stringify(itemDataAfterSave, null, 4);
        itemDataAfterSave = itemDataAfterSave.replace(/\\n/g, "<br>").replace(/\\"/g, '"').replace(/ /g, '&nbsp;');
        $("#divMdlIdas").html(itemDataAfterSave);

        var itemDataDiff = $(".divIdDiff-" + rowId).html();
        itemDataDiff = JSON.stringify(itemDataDiff, null, 4);
        itemDataDiff = itemDataDiff.replace(/\\n /g, "<br>&nbsp;").replace(/\\n/g, "<br>").replace(/\\"/g, '"');
        $("#divMdlIalDiff").html(itemDataDiff);

        itemAuditLogModal.show();
    });

    $('#mdlItemAuditLog').on('hidden.bs.modal', function (e) {
        $("#divMdlIdbs").html("");
        $("#divMdlIdas").html("");
        $("#divMdlIalDiff").html("");
    });

    $("#btnDeleteIaLogs").click(function () {
        $("#divResult").html("");
        $("#spValidation").html("");
        $("#divLogsDeletedStatus").html("");

        if (confirm("Are you sure you want to delete all the audit logs in this date range?")) {
            if (AreInputDatesValid($("#txtFromDate_Pl").val(), $("#txtToDate_Pl").val())) {
                DeleteIaLogs();
            }
            else {
                $("#spValidation").html("Enter valid From & To dates");
            }
        }
        else {
            return false;
        }
    });

    $("#btnOk_lds").click(function () {
        var deletedRows = $(this).attr("data-deletedRows");
        logDeleteStatusModal.hide();

        if (deletedRows > 0) {
            location.reload(true);
        }
    });
});

let waitModal;
let itemAuditLogModal;
let logDeleteStatusModal;

Date.prototype.toDateInputValue = (function () {
    var local = new Date(this);
    local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
    return local.toJSON().slice(0, 10);
});

function AppInit() {
    SetActiveMenu();
    WaitModalInit();
    ItemAuditLogModalInit();
    AuditLogDeleteStatusModalInit();

    var isPostBack = $("#hdnIsPostBack").val();

    /// set current date only on initial page load.
    if (isPostBack == 0) {
        $('#txtFromDate_Ial').val(new Date().toDateInputValue());
        $('#txtToDate_Ial').val(new Date().toDateInputValue());
    }
}

function SetActiveMenu() {
    $(".nav-item a").each(function () {
        var href = $(this).attr("href");

        if (href == location.pathname) {
            $(this).parent().addClass("active");
            return;
        }
    });
}

function WaitModalInit() {
    waitModal = new bootstrap.Modal(document.getElementById('mdlWait'), {
        backdrop: 'static',
        keyboard: false
    });
}

function ItemAuditLogModalInit() {
    var mdlItemAuditLog = document.getElementById('mdlItemAuditLog');
    if (mdlItemAuditLog != null && mdlItemAuditLog != undefined) {
        itemAuditLogModal = new bootstrap.Modal(mdlItemAuditLog, {
            backdrop: 'static',
            keyboard: false
        });
    }
}

function AuditLogDeleteStatusModalInit() {
    var mdlLogsDeletedStatus = document.getElementById('mdlLogsDeletedStatus');
    if (mdlLogsDeletedStatus != null && mdlLogsDeletedStatus != undefined) {
        logDeleteStatusModal = new bootstrap.Modal(mdlLogsDeletedStatus, {
            backdrop: 'static',
            keyboard: false
        });
    }
}

function RepositionModal() {
    var modal = $(this), dialog = modal.find('.modal-dialog');
    modal.css('display', 'block');

    /// Dividing by two centers the modal exactly, but dividing by three
    /// or four works better for larger screens.
    dialog.css("margin-top", Math.max(0, ($(window).height() - dialog.height()) / 2));
}

function ShowWaitModal() {
    waitModal.show();
}

function HideWaitModal() {
    waitModal.hide();
}

function IsNullOrWhiteSpace(value) {
    return value == null || value == undefined || value.length <= 0 || value == "" || $.trim(value) == "";
}

function IsValidData() {
    var isValid = true;

    if (AreInputDatesValid($("#txtFromDate_Ial").val(), $("#txtToDate_Ial").val())) {
        ///do nothing
    }
    else {
        isValid = false;
        $("#spValidation").html("Enter valid From & To dates");
    }

    return isValid;
}

function AreInputDatesValid(fromDate, toDate) {
    var areDatesValid = false;

    if ((!IsNullOrWhiteSpace(fromDate)) && (!IsNullOrWhiteSpace(toDate))) {
        var today = new Date().setHours(0, 0, 0, 0);
        var fromDate = new Date(fromDate).setHours(0, 0, 0, 0);
        var toDate = new Date(toDate).setHours(0, 0, 0, 0);
        areDatesValid = fromDate <= today && fromDate <= toDate && toDate <= today;
    }

    return areDatesValid;
}

function ClearResult() {
    $("#spValidation").html("");
    $("#divError").hide();
    $("#spError").html("");
    $("#divResult").html("");
}

function GetData_Ial() {
    var request = {};
    request.LogType = $("#ddlLogTypes").val();
    request.FromDateString = $("#txtFromDate_Ial").val();
    request.ToDateString = $("#txtToDate_Ial").val();
    request.ItemId = $.trim($("#txtItemId").val());
    request.Username = $.trim($("#txtUsername").val());
    request.ItemLanguage = $("#ddlLanguages").val();

    $.ajax({
        type: "POST",
        url: "item-audit.aspx/GetData",
        data: JSON.stringify({ itemAuditLogRequest: request }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: "false",
        cache: "false",
        beforeSend: function () {
            ShowWaitModal();
        },
        success: function (data) {
            if (data != null && data != undefined) {
                BindIalDataToGrid(data);
                HideWaitModal();                
            }
            else {
                HideWaitModal();
                $("#spError").html("A problem occurred while procesing your request.");
                $("#divError").show();
                console.log(data);
            }
        },
        error: function (data) {
            HideWaitModal();
            $("#spError").html("A problem occurred while procesing your request. Please check the console log for more information.");
            $("#divError").show();
            console.log(data);
        }
    });
}

function BindIalDataToGrid(data) {

    try {
        /// Parse the XML and extract the records.
        var logs = $($.parseXML(data.d)).find("dtIal");

        /// Reference GridView Table.
        var table = $("[id*=gvIal]");

        /// Reference the Dummy Row.
        var row = table.find("tr:last-child").clone(true);

        /// Remove the Dummy Row.
        $("tr", table).not($("tr:first-child", table)).remove();

        /// Loop through the XML and add Rows to the Table.
        $.each(logs, function () {
            var log = $(this);
            $("td", row).eq(0).html($(log).find("Row_Id").text());
            $("td", row).eq(1).html($(log).find("Item_Info").text());
            $("td", row).eq(2).html($(log).find("Event").text());
            $("td", row).eq(3).html($(log).find("Actioned_By").text());
            $("td", row).eq(4).html($(log).find("Comments").text());
            $("td", row).eq(5).html($(log).find("Logged_Time").text());
            $("td", row).eq(6).html($(log).find("Item_Content").text());
            table.append(row);
        });
    } catch (e) {
        $("#spError").html("A problem occurred while procesing your request.");
        $("#divError").show();
        console.log(e);
        console.log(data);
    }
}

function DeleteIaLogs() {

    $.ajax({
        type: "POST",
        url: "purge-logs.aspx/PurgeItemAuditLogs",
        data: JSON.stringify({ fromDate: $("#txtFromDate_Pl").val(), toDate: $("#txtToDate_Pl").val() }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: "false",
        cache: "false",
        beforeSend: function () {
            ShowWaitModal();
        },
        success: function (data) {
            if (data != null && data != undefined) {
                data = JSON.parse(data.d);

                if (data != null && data != undefined) {
                    if (data.StatusCode == 0) {
                        HideWaitModal();
                        $("#spError").html(data.StatusMessage);
                        $("#divError").show();
                        console.log(data);
                    }
                    else if (data.StatusCode == 1) {
                        HideWaitModal();
                        $("#divLogsDeletedStatus").html(data.StatusMessage + " rows deleted");
                        $("#btnOk_lds").attr("data-deletedRows", data.StatusMessage);
                        logDeleteStatusModal.show();
                    }
                    else if (data.StatusCode == 2) {
                        window.location = "/sitecore/login";
                    }
                }
                else {
                    HideWaitModal();
                    $("#spError").html("A problem occurred while procesing your request.");
                    $("#divError").show();
                    console.log(data);
                }
            }
            else {
                HideWaitModal();
                $("#spError").html("A problem occurred while procesing your request.");
                $("#divError").show();
                console.log(data);
            }
        },
        error: function (data) {
            HideWaitModal();
            $("#spError").html("A problem occurred while procesing your request. Please check the console log for more information.");
            $("#divError").show();
            console.log(data);
        }
    });
}