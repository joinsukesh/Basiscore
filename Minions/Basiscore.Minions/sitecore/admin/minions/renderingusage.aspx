<%@ Page Title="Rendering Usage" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="renderingusage.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.renderingusage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <asp:HiddenField ClientIDMode="Static" ID="hdnSessionId" runat="server" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-darksalmon bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                This tool will help you find all the pages under a parent node, which have used a rendering.
                            <br />
                                It will search the presentation details of an item's latest version, in the target database.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Database <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtDatabase" runat="server" CssClass="form-control" ClientIDMode="Static" Text="master"></asp:TextBox>
            <span class="text-muted">Enter name of the target database.</span><br />
            <span id="spDatabase" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Tasks</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlTasks" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Find rendering in child pages" Value="1"></asp:ListItem>
                <asp:ListItem Text="Find rendering in specified page items" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-group row task task-1">
        <label class="col-sm-2 col-form-label">Parent Item <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtParentItem" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the parent item. This item will be excluded from the search.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row task task-2" style="display: none;">
        <label class="col-sm-2 col-form-label">Target Page Items <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtTargetItemPaths" runat="server" CssClass="form-control" ClientIDMode="Static" TextMode="MultiLine" Rows="5"></asp:TextBox>
            <span class="text-muted">Enter item paths separated by line breaks.<br />
                e.g:&nbsp;<i>/sitecore/content/item1<br />
                    &nbsp;&nbsp;&emsp;/sitecore/content/item2</i>
            </span>
            <br />
            <span id="spTargetItemPaths" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row task task-1">
        <label class="col-sm-2 col-form-label">Target Page Item Template</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtTargetItemTemplate" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the template. Child items of only this template will be considered.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Rendering Id <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtRenderingId" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the rendering.</span><br />
            <span id="spRenderingId" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Language</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlLanguages" runat="server" CssClass="form-control" ClientIDMode="Static" AppendDataBoundItems="true">
                <asp:ListItem Text="Select language" Value="0"></asp:ListItem>
            </asp:DropDownList>
            <span id="spLanguageCodes" class="validation-msg">Select target language</span>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
            <button type="button" id="btnSubmit" class="btn btn-primary pull-right">SUBMIT</button>
        </div>
    </div>
    <br />
    <!--RESULT-->
    <div class="row">
        <div class="col-lg-12">
            <div class="divErrorLogContainer" style="display: none;">
                <p id="pError" class="error-msg"></p>
            </div>
            <p id="pTaskStatus" class="success-msg"></p>
            <p>
                <button type="button" id="btnDownload" class="btn btn-warning pull-right" style="display: none;">DOWNLOAD</button>
                <br />
                <br />
            </p>
            <table id="tblResult" class="table table-striped table-bordered table-condensed" style="display: none;">
                <thead>
                    <tr>
                        <th class="col-sm-1 text-center">#</th>
                        <th class="col-sm-6">ITEM PATH</th>
                        <th class="col-sm-5">TARGET LAYOUTS</th>
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

            $("#btnReset").click(function () {
                ResetValues();
            });

            $("#ddlTasks").change(function () {
                $("#txtParentItem").val("");
                $("#txtTargetItemTemplate").val("");
                $("#txtTargetItemPaths").val("");
                $(".task").hide();
                var selectedTaskId = $(this).val();
                $(".task-" + selectedTaskId).show();
            });

            $("#btnSubmit").click(function () {
                ClearResults();
                var selectedTaskId = $("#ddlTasks").val();

                if (IsValidModel(selectedTaskId)) {
                    var dataModel = GetDataModel(selectedTaskId);
                    OnSubmit(dataModel);
                }
            });

            $("#btnDownload").click(function () {
                app.DownloadData('renderingusage', $("#hdnSessionId").val());
            });

        });

        function GetDataModel(taskId) {
            var dataModel = {};
            dataModel.SessionId = $("#hdnSessionId").val();
            dataModel.TaskId = taskId;
            dataModel.DatabaseName = $.trim($("#txtDatabase").val());
            dataModel.ParentItemId = $.trim($("#txtParentItem").val());
            dataModel.TargetTemplateId = $.trim($("#txtTargetItemTemplate").val());
            dataModel.RenderingId = $.trim($("#txtRenderingId").val());
            dataModel.TargetLanguageCode = $("#ddlLanguages").val();

            if (taskId == 2 || taskId == "2") {
                dataModel.TargetItemPaths = $.trim($("#txtTargetItemPaths").val());
            }

            return dataModel;
        }

        function OnSubmit(dataModel) {
            var postUrl = "renderingusage.aspx/GetRenderingUsageResult";

            $.ajax({
                type: "POST",
                url: postUrl,
                data: JSON.stringify({ dataModel: dataModel }),
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
                            //console.log(objData.StatusCode);
                            //console.log(objData.LstRenderingStatus);

                            if (objData != null) {
                                if (objData.Error != null && objData.Error.length > 0) {
                                    $("#pError").html(objData.Error);
                                    $(".divErrorLogContainer").show();
                                    $("html, body").animate({
                                        scrollTop: $(".divErrorLogContainer").offset().top
                                    }, app.scrollDuration);
                                }
                                else {
                                    if (objData.StatusCode == 0 || objData.StatusCode == 1) {
                                        if (objData.StatusCode == 0) {
                                            $("#pError").html(objData.StatusMessage);
                                            $(".divErrorLogContainer").show();
                                        }
                                        else {
                                            $("#pTaskStatus").html(objData.StatusMessage);
                                            $("#btnDownload").show();
                                        }
                                        var rows = "";
                                        var index = 1;
                                        $.each(objData.LstRenderingStatus, function () {
                                            if (this.StatusCode != 1 || this.StatusCode != "1") {
                                                rows += '<tr class="errorLog">';
                                            }
                                            else {
                                                rows += "<tr>";
                                            }
                                            rows += '<td class="text-center">' + index + '</td>';
                                            rows += "<td>" + this.PageItemPath + "</td>";
                                            rows += "<td>" + this.StatusMessage + "</td>";
                                            rows += "</tr>";
                                            index++;
                                        });

                                        $("#tbResultRows").html(rows);
                                        $("#tblResult").show();

                                        if (objData.StatusCode == 0) {
                                            $("html, body").animate({
                                                scrollTop: $(".divErrorLogContainer").offset().top
                                            }, app.scrollDuration);
                                        }
                                        else {
                                            $("html, body").animate({
                                                scrollTop: $("#tblResult").offset().top
                                            }, app.scrollDuration);
                                        }
                                    }
                                    else if (objData.StatusCode == 2) {
                                        window.location.href = "/sitecore/login";
                                    }
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

        function IsValidModel(taskId) {
            var isValidModel = true;

            if (app.StringNullOrEmpty($("#txtDatabase").val())) {
                isValidModel = false;
                $("#spDatabase").show();
            }

            if (taskId == 1 || taskId == "1") {
                if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                    isValidModel = false;
                    $("#spParentItem").show();
                }
            }

            if (taskId == 2 || taskId == "2") {
                if (app.StringNullOrEmpty($("#txtTargetItemPaths").val())) {
                    isValidModel = false;
                    $("#spTargetItemPaths").show();
                }
            }

            ///language
            if ($("#ddlLanguages").val() == "0" || $("#ddlLanguages").val() == 0) {
                isValidModel = false;
                $("#spLanguageCodes").show();
            }

            ///rendering
            if (app.StringNullOrEmpty($("#txtRenderingId").val())) {
                isValidModel = false;
                $("#spRenderingId").show();
            }

            return isValidModel;
        }

        function ResetValues() {
            ClearFieldValues();
            $("#txtDatabase").val("master");
            $("#ddlTasks").val(1);
            $("#ddlLanguages").val(0);
            $(".task, .tl").hide();
            $(".task-1, .tl-1").show();
            $("#btnDownload").hide();
            $("#d-iframe").remove();
        }

        ///clear all field values
        function ClearFieldValues() {
            app.DefaultResetForm();
            ClearResults();
        }

        ///clear result section
        function ClearResults() {
            $(".validation-msg").hide();
            $("#pTaskStatus").html("");
            $("#tbResultRows").html("");
            $("#tblResult").hide();
            $(".divErrorLogContainer").hide();
            $("#btnDownload").hide();
            $("#d-iframe").remove();
        }

        function Init() {
            ResetValues();
        }
    </script>
</asp:Content>
