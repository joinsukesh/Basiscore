<%@ Page Title="Remove Renderings" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="removerenderings.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.removerenderings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-darkseagreen bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                Remove a rendering from the Shared Layout, for multiple pages, in the master database.
                            </p>
                            <p>
                                <strong>NOTE: </strong>This tool is best suited for a monolingual website or if all the language versions use the same renderings.
                                <br />
                                <br />
                                Though the 'Target Language' field's purpose is to update an item of the selected language, it could be possible that the changes may apply to other languages also, depending on how the Presentation details' Standard values are set for the item.<br />
                                If you are working on a multi-lingual site, it is recommended that you first check with only a few items before updating multiple pages.<br />
                                <br />
                                Reference: <a href="https://doc.sitecore.com/developers/90/sitecore-experience-manager/en/versioned-layouts.html" target="_blank">https://doc.sitecore.com/developers/90/sitecore-experience-manager/en/versioned-layouts.html</a>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Tasks</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlTasks" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Remove first instance of rendering" Value="1"></asp:ListItem>
                <asp:ListItem Text="Remove last instance of rendering" Value="2"></asp:ListItem>
                <asp:ListItem Text="Remove rendering if it is at specified index" Value="3"></asp:ListItem>
                <asp:ListItem Text="Remove all instances of rendering" Value="4"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Items</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlTargetItems" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Child items under the parent" Value="1"></asp:ListItem>
                <asp:ListItem Text="Specified item paths" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-group row dtl dtl-1">
        <label class="col-sm-2 col-form-label">Parent Item <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtParentItem" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the parent item. This item will be excluded.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row dtl dtl-2" style="display: none;">
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
    <div class="form-group row dtl dtl-1">
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
    <div class="form-group row task task-3">
        <label class="col-sm-2 col-form-label">Rendering Index <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtRenderingIndex" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">If the rendering is available at this index, it will be removed. The first position will have the index as 0.</span><br />
            <span id="spRenderingIndex" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Layout</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlTargetLayouts" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Shared Layout" Value="1"></asp:ListItem>
                <asp:ListItem Text="Final Layout" Value="2"></asp:ListItem>
            </asp:DropDownList>
            <span class="text-muted tl tl-2" style="display: none;">Only renderings that are exclusively added in the final layout will be removed.</span><br />
        </div>
    </div>
    <div class="form-group row tl tl-2" style="display: none;">
        <label class="col-sm-2 col-form-label">Copy Final Layout's Renderings to Shared Layout</label>
        <div class="col-sm-4">
            <asp:CheckBox ID="chkCopyFinalToShared" runat="server" CssClass="checkbox-inline" ClientIDMode="Static"></asp:CheckBox>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Create New Language Version</label>
        <div class="col-sm-4">
            <asp:CheckBox ID="chkCreateVersion" runat="server" CssClass="checkbox-inline" ClientIDMode="Static" Checked="true"></asp:CheckBox>
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
            <table id="tblResult" class="table table-striped table-bordered table-condensed" style="display: none;">
                <thead>
                    <tr>
                        <th class="col-sm-1 text-center">#</th>
                        <th class="col-sm-6">ITEM PATH</th>
                        <th class="col-sm-5">STATUS</th>
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
                $(".task").hide();
                var selectedTaskId = $(this).val();
                $(".task-" + selectedTaskId).show();
                ClearResults();
            });

            $("#ddlTargetItems").change(function () {
                $(".dtl").hide();
                var selectedId = $(this).val();
                $(".dtl-" + selectedId).show();
                ClearResults();
            });

            $("#ddlTargetLayouts").change(function () {
                $(".tl").hide();
                var selectedId = $(this).val();
                $(".tl-" + selectedId).show();

                if (selectedId == 1 || selectedId == "1") {
                    $("#chkCopyFinalToShared").prop("checked", "");
                }

                ClearResults();
            });

            $("#btnSubmit").click(function () {
                ClearResults();
                var selectedTaskId = $("#ddlTasks").val();
                var targetItemsTypeId = $("#ddlTargetItems").val();

                if (IsValidModel(selectedTaskId, targetItemsTypeId)) {
                    var dataModel = GetDataModel(selectedTaskId, targetItemsTypeId);
                    OnSubmit(dataModel);
                }
            });

        });

        function GetDataModel(taskId, targetItemsTypeId) {
            var dataModel = {};
            dataModel.TaskId = taskId;
            dataModel.TargetItemsTypeId = targetItemsTypeId;

            if (targetItemsTypeId == 1 || targetItemsTypeId == "1") {
                dataModel.ParentItemId = $.trim($("#txtParentItem").val());
                dataModel.TargetTemplateId = $.trim($("#txtTargetItemTemplate").val());
            }

            if (targetItemsTypeId == 2 || targetItemsTypeId == "2") {
                dataModel.TargetItemPaths = $.trim($("#txtTargetItemPaths").val());
            }

            if (taskId == 3 || taskId == "3") {
                dataModel.InputRenderingIndex = $.trim($("#txtRenderingIndex").val());
            }
            else {
                dataModel.InputRenderingIndex = -1;
            }

            dataModel.RenderingId = $.trim($("#txtRenderingId").val());
            dataModel.TargetLayoutId = $("#ddlTargetLayouts").val();
            dataModel.CreateVersion = $("#chkCreateVersion").is(":checked");
            dataModel.CopyFinalRenderingsToShared = $("#chkCopyFinalToShared").is(":checked");
            dataModel.TargetLanguageCode = $("#ddlLanguages").val();
            return dataModel;
        }

        function OnSubmit(dataModel) {
            var postUrl = "removerenderings.aspx/RemoveRenderings";

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
                            console.log(objData.StatusCode);
                            console.log(objData.LstRenderingStatus);

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

        function IsValidModel(taskId, targetItemsTypeId) {
            var isValidModel = true;

            if (taskId == 3 || taskId == "3") {
                if (app.StringNullOrEmpty($("#txtRenderingIndex").val())) {
                    isValidModel = false;
                    $("#spRenderingIndex").show();
                }
            }

            if (targetItemsTypeId == 1 || targetItemsTypeId == "1") {
                if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                    isValidModel = false;
                    $("#spParentItem").show();
                }
            }

            if (targetItemsTypeId == 2 || targetItemsTypeId == "2") {
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
            $("#ddlTasks").val(1);
            $("#ddlTargetItems").val(1);
            $("#ddlTargetLayouts").val(1);
            $("#ddlLanguages").val(0);
            $(".task, .dtl, .tl").hide();
            $(".task-1, .dtl-1, .tl-1").show();
            $("#chkCopyFinalToShared").prop("checked", "");
            $("#chkCreateVersion").prop("checked", "checked");
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
        }

        function Init() {
            ResetValues();
        }
    </script>
</asp:Content>

