<%@ Page Title="Publish Items" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="publishitems.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.publishitems" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-rosybrown bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                When a Sitecore package is installed in an instance, generally, the items in that package may also need to be published to the <em>Web</em> databases.
                                <br />
                                This tool will help you perform a <strong><em>Smart Publish</em></strong> for bulk items, from <em>Master</em> to the target database(s), in one go.<br />
                                There are two input modes to choose from - Enter the paths of the items to be published, or pick them from an existing Sitecore package.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="row">
        <div class="col-md-12">
            <div class="form-group">
                <label>Item paths input mode</label><br />
                <label class="radio-inline">
                    <input type="radio" name="optInputType" value="1" checked>Enter item paths manually</label>
                <label class="radio-inline">
                    <input type="radio" name="optInputType" value="2">Read item paths from package</label>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <!--MANUAL INPUT-->
            <div class="divManualInput">
                <div class="form-group">
                    <label>Items to be published with subitems</label><br />
                    <span class="text-muted">Enter item paths separated by line breaks.<br />
                        e.g:&nbsp;<i>/sitecore/content/item1<br />
                            &nbsp;&nbsp;&emsp;/sitecore/content/item2</i>
                    </span>
                    <asp:TextBox ID="txtPublishWithSubitems" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ClientIDMode="Static"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Items to be published singly</label><br />
                    <span class="text-muted">Enter item paths separated by line breaks.<br />
                        e.g:&nbsp;<i>/sitecore/content/item1<br />
                            &nbsp;&nbsp;&emsp;/sitecore/content/item2</i>
                    </span>
                    <asp:TextBox ID="txtPublishItems" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ClientIDMode="Static"></asp:TextBox>
                    <br />
                    <span id="spItemPaths" class="validation-msg">Atleast one item path should be provided</span>
                </div>
            </div>
            <!--PACKAGE INPUT-->
            <div class="divPackageInput" style="display: none;">
                <p class="text-dark">
                    Enter full path of the installed Sitecore package, which has the list of items & click on <em>Get Items List</em>.<br />
                    It will pick & display, the individual item paths of <em>Master database</em>, from that package.<br />
                    Check the items that you want to publish, to the target database(s).
                </p>
                <div class="form-group">
                    <label>Sitecore package path</label><br />
                    <span class="text-muted">Enter full path of the Sitecore package, which has the items list. e.g: <em>D:\inetpub\mysite\data\packages\MyPackage.zip</em><br />
                        If the instance is on Azure, the installed packages will usually be inside <em>D:\home\site\wwwroot\App_Data\packages\</em>
                    </span>
                    <div>
                        <br />
                    </div>
                    <div class="row">
                        <div class="col-md-9">
                            <input type="text" id="txtPackagePath" class="form-control" />
                            <span id="spPackagePath" class="validation-msg">This field is required</span>
                        </div>
                        <div class="col-md-3">
                            <button type="button" id="btnGetItemPaths" class="btn btn-secondary">GET ITEMS LIST</button>
                        </div>
                    </div>
                </div>
                <br />
                <!--PACKAGE ITEM PATHS-->
                <div class="row">
                    <div class="col-lg-12">
                        <div class="divPipErrorLogContainer" style="display: none;">
                            <div class="card card-body divPipError errorLog">
                            </div>
                        </div>
                        <br />
                        <table id="tblPip" class="table table-striped table-bordered table-condensed" style="display: none;">
                            <thead>
                                <tr class="cadetblue">
                                    <th>ITEM PATH</th>
                                    <th class="center">PUBLISH&nbsp;&nbsp;<input type="checkbox" id="chkAllItemPaths" class="thead-chkbox-inline" checked /></th>
                                </tr>
                            </thead>
                            <tbody id="tbPipRows">
                            </tbody>
                        </table>
                        <br />
                        <span id="spPipRows" class="validation-msg">Select items to be published</span>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-6">
                    <div class="chkboxlist-container">
                        <label>Publishing languages</label><br />
                        <input type="checkbox" id="chkAllLanguages" />&nbsp;Select all<br />
                        <asp:CheckBoxList ID="chkLanguages" runat="server" CssClass="chkboxlist chkLanguage"
                            RepeatLayout="Flow" ClientIDMode="Static">
                        </asp:CheckBoxList>
                    </div>
                    <span id="spLanguageCodes" class="validation-msg">Atleast one language should be selected</span>
                </div>
                <div class="col-md-6">
                    <div class="chkboxlist-container">
                        <label>Publishing targets</label><br />
                        <asp:CheckBoxList ID="chkDatabases" runat="server" CssClass="chkboxlist"
                            RepeatLayout="Flow" ClientIDMode="Static">
                        </asp:CheckBoxList>
                    </div>
                    <span id="spDbNames" class="validation-msg">Atleast one target should be selected</span>
                </div>
            </div>
            <br />
            <div class="form-group chkboxlist">
                <input type="checkbox" id="chkExcludeIfWorkflow" />&nbsp;<label>Exclude items with workflow</label>
            </div>
            <br />
            <div class="row">
                <div class="col-md-12">
                    <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
                    <button type="button" id="btnPublishItems1" class="divManualInput btn btn-primary pull-right">PUBLISH</button>
                    <button type="button" id="btnPublishItems2" class="divPackageInput btn btn-primary pull-right" style="display: none;">PUBLISH</button>
                </div>
            </div>
        </div>
    </div>
    <br />
    <!--RESULT-->
    <div class="row">
        <div class="col-lg-12">
            <div class="divErrorLogContainer" style="display: none;">
                <p>
                    <button class="btn btn-danger" type="button" data-toggle="collapse" data-target="#collapseErrorLog">
                        Error Log&nbsp;<i class="fa fa-angle-down" aria-hidden="true"></i>
                    </button>
                </p>
                <div class="collapse" id="collapseErrorLog">
                    <div class="card card-body divError">
                    </div>
                </div>
            </div>
            <br />
            <div class="showForResult" style="display: none; margin-bottom:10px;">
                <strong>The <span style="color:coral">colored</span> row is to caution, if an item is neither created nor updated on this publish.</strong>
            </div>
            <table id="tblResult" class="table table-bordered table-condensed" style="display: none;">
                <thead>
                    <tr>
                        <th>ITEM PATH</th>
                        <th class="center">CREATED</th>
                        <th class="center">UPDATED</th>
                        <th class="center">SKIPPED</th>
                    </tr>
                </thead>
                <tbody id="tbResultRows">
                </tbody>
            </table>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            Init();

            $("input[name='optInputType']").on("change", function () {
                ClearFieldValues();
                var value = $(this).val();

                if (value == 1 || value == "1") {
                    $(".divPackageInput").hide();
                    $(".divManualInput").fadeIn();
                }
                else if (value == 2 || value == "2") {
                    $(".divManualInput").hide();
                    $(".divPackageInput").fadeIn();
                    $("#txtPackagePath").focus();
                }
            });

            $("#chkAllItemPaths").change(function () {
                if ($(this).is(":checked")) {
                    $("#tbPipRows input[type='checkbox']").prop("checked", "checked");
                }
                else {
                    $("#tbPipRows input[type='checkbox']").prop("checked", "");
                }
            });

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

            $("body").on("change", "#tbPipRows input[type='checkbox']", function () {
                if (!$(this).is(":checked")) {
                    $("#chkAllItemPaths").prop("checked", "");
                }
            });

            $("#btnReset").click(function () {
                ClearFieldValues();
            });

            $("#btnPublishItems1").click(function () {
                ClearResults();

                if (IsValidModel(1)) {
                    var dataModel = GetDataModel(1);
                    OnSubmit(dataModel);
                }
            });

            $("#btnPublishItems2").click(function () {
                ClearResults();

                if (IsValidModel(2)) {
                    var dataModel = GetDataModel(2);
                    OnSubmit(dataModel);
                }
            });

            $("#btnGetItemPaths").click(function () {
                ClearResults();
                ResetPipResult();
                var scPackagePath = $("#txtPackagePath").val();

                if (!app.StringNullOrEmpty(scPackagePath)) {
                    $.ajax({
                        type: "POST",
                        url: "publishitems.aspx/GetPackageItemPaths",
                        data: JSON.stringify({ packagePath: scPackagePath }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: "true",
                        cache: "false",
                        beforeSend: function () {
                            app.ShowLoadingModal();
                        },
                        success: function (data) {
                            setTimeout(
                                function () {
                                    app.HideLoadingModal();
                                    var objData = JSON.parse(data.d);
                                    //console.log(objData);

                                    if (objData != null) {
                                        if (objData.Error != null && objData.Error.length > 0) {
                                            $(".divPipError").html(objData.Error);
                                            $(".divPipErrorLogContainer").show();
                                            $("html, body").animate({
                                                scrollTop: $(".divPipErrorLogContainer").offset().top
                                            }, app.scrollDuration);
                                        }

                                        if (objData.GetItemPathsStatus == 1) {
                                            var rows = "";
                                            $.each(objData.LstPackageItemPaths, function () {
                                                rows += "<tr>";
                                                rows += "<td>" + this + "</td>";
                                                rows += "<td align='center'><input type='checkbox' name='chkItemPath' data-path='" + this + "' checked/></td>";
                                                rows += "</tr>";
                                            });
                                            $("#tbPipRows").html(rows);
                                            $("#tblPip").show();
                                            $("html, body").animate({
                                                scrollTop: $("#tblPip").offset().top
                                            }, app.scrollDuration);
                                        }
                                        else if (objData.GetItemPathsStatus == 2) {
                                            window.location.href = "/sitecore/login";
                                        }
                                    }
                                }, app.modalShowDelay);
                        },
                        failure: function (data) {
                            app.HideLoadingModal();
                        },
                        error: function (data) {
                            app.HideLoadingModal();
                            console.log(data);
                        },
                        complete: function (data) {
                        }
                    });
                }
                else {
                    $("#spPackagePath").show()
                }
            });
        });

        function GetDataModel(inputType) {
            var dataModel = {};
            if (inputType == 1) {
                dataModel.ItemPathsToPublishWithSubitems = $("#txtPublishWithSubitems").val();
                dataModel.ItemPathsToPublish = $("#txtPublishItems").val();
            }
            else if (inputType == 2) {
                dataModel.ItemPathsToPublishWithSubitems = "";
                dataModel.ItemPathsToPublish = GetCheckedItemPaths();
            }
            dataModel.CommaSeparatedLanguageCodes = GetSelectedLanguageCodes().join(',');
            dataModel.CommaSeparatedDatabaseNames = GetSelectedDbNames().join(',');;
            dataModel.ExcludeItemsWithWorkflow = $("#chkExcludeIfWorkflow").is(":checked");
            return dataModel;
        }

        function OnSubmit(dataModel) {
            $.ajax({
                type: "POST",
                url: "publishitems.aspx/PublishSitecoreItems",
                data: JSON.stringify({ customPublishDataModel: dataModel }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: "true",
                cache: "false",
                beforeSend: function () {
                    app.ShowLoadingModal();
                },
                success: function (data) {
                    setTimeout(
                        function () {
                            app.HideLoadingModal();
                            var objData = JSON.parse(data.d);
                            //console.log(objData);

                            if (objData != null) {
                                if (objData.Error != null && objData.Error.length > 0) {
                                    $(".divError").html(objData.Error);
                                    $(".divErrorLogContainer").show();
                                    $("html, body").animate({
                                        scrollTop: $(".divErrorLogContainer").offset().top
                                    }, app.scrollDuration);
                                }

                                if (objData.PublishStatus == 1) {
                                    var rows = "";
                                    $.each(objData.LstItemPublishStatus, function () {
                                        if (this.CautionUser) {
                                            rows += '<tr class="bg-coral">';
                                        }
                                        else {
                                            rows += "<tr>";
                                        }
                                        
                                        rows += "<td>" + this.ItemPath + "</td>";
                                        rows += "<td align='center'>" + this.ItemsCreatedStatus + "</td>";
                                        rows += "<td align='center'>" + this.ItemsUpdatedStatus + "</td>";
                                        rows += "<td align='center'>" + this.ItemsSkippedStatus + "</td>";
                                        rows += "</tr>";
                                    });
                                    $("#tbResultRows").html(rows);
                                    $("#tblResult").show();
                                    $(".showForResult").show();
                                    $("html, body").animate({
                                        scrollTop: $("#tblResult").offset().top
                                    }, app.scrollDuration);
                                }
                                else if (objData.PublishStatus == 2) {
                                    window.location.href = "/sitecore/login";
                                }
                            }
                        }, app.modalShowDelay);
                },
                failure: function (data) {
                    app.HideLoadingModal();
                },
                error: function (data) {
                    app.HideLoadingModal();
                    console.log(data);
                },
                complete: function (data) {
                }
            });
        }

        function IsValidModel(inputType) {
            var isValidModel = true;

            if (inputType == 1) { //manual
                var hasItemPathForFirstBox = false;
                var hasItemPathForSecondBox = false;

                if (!app.StringNullOrEmpty($("#txtPublishWithSubitems").val())) {
                    hasItemPathForFirstBox = true;
                }

                if (!app.StringNullOrEmpty($("#txtPublishItems").val())) {
                    hasItemPathForSecondBox = true;
                }

                if (!hasItemPathForFirstBox && !hasItemPathForSecondBox) {
                    isValidModel = false;
                    $("#spItemPaths").show()
                }
            }
            else if (inputType == 2) { //package item paths
                var itemPaths = GetCheckedItemPaths();
                if (app.StringNullOrEmpty(itemPaths)) {
                    isValidModel = false;
                    $("#spPipRows").show();
                }
            }

            var selectedLanguageCodes = GetSelectedLanguageCodes();

            if (selectedLanguageCodes == null || selectedLanguageCodes.length <= 0) {
                isValidModel = false;
                $("#spLanguageCodes").show();
            }

            var selectedDbNames = GetSelectedDbNames();

            if (selectedDbNames == null || selectedDbNames.length <= 0) {
                isValidModel = false;
                $("#spDbNames").show();
            }

            return isValidModel;
        }

        function GetSelectedLanguageCodes() {
            var selectedLanguageCodes = [];
            $("#chkLanguages input:checked").each(function () {
                selectedLanguageCodes.push($(this).attr("value"));
            });
            return selectedLanguageCodes;
        }

        function GetSelectedDbNames() {
            var selectedDbNames = [];
            $("#chkDatabases input:checked").each(function () {
                selectedDbNames.push($(this).attr("value"));
            });
            return selectedDbNames;
        }

        ///clear all field values
        function ClearFieldValues() {

            //hide all validation msgs
            $(".validation-msg").hide();

            //clear all textbox values
            $("input[type='text'], textarea").each(function () {
                $(this).val("");
            });

            //uncheck all checkboxes
            $(".chkboxlist input[type='checkbox']").prop("checked", "");
            $("#chkAllLanguages").prop("checked", "");

            $("#tbPipRows").html("");
            $("#tblPip").hide();

            ClearResults();
        }

        //clear result section
        function ClearResults() {
            $(".validation-msg").hide();
            $("#tbResultRows").html("");
            $("#tblResult").hide();
            $(".showForResult").hide();
            $(".divError").html("");
            $(".divErrorLogContainer").hide();
            $(".divPipError").html("");
            $(".divPipErrorLogContainer").hide();
        }

        function GetCheckedItemPaths() {
            var itemPaths = "";
            $("#tbPipRows input:checked").each(function () {
                itemPaths += $(this).attr("data-path") + "\n";
            });
            return itemPaths;
        }

        function ResetPipResult() {
            $("#spPipRows").hide();
            $(".divPipError").html("");
            $(".divPipErrorLogContainer").hide();
            $("#chkAllItemPaths").prop("checked", "checked");
            $("#tbPipRows").html("");
            $("#tblPip").hide();
        }

        function ResetForm() {
            ClearFieldValues();
        }

        function Init() {
            ResetForm();
        }
    </script>
</asp:Content>
