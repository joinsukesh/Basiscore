﻿<%@ Page Title="Update Fields" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="updatefields.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.updatefields" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-cadetblue bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                Use this tool for these field update related tasks, in the master database.<br />
                                <ul>
                                    <li>Replace the keyword in a field value.</li>
                                    <li>Update the field value.</li>
                                </ul>
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
                <asp:ListItem Text="Replace Keyword in Field Value" Value="2" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Replace Keyword in Field Value By Item Paths" Value="6"></asp:ListItem>
                <asp:ListItem Text="Update Field Value" Value="3"></asp:ListItem>
                <asp:ListItem Text="Update Field Value By Item Paths" Value="5"></asp:ListItem>
            </asp:DropDownList>
            <span class="task task-2 text-muted">Replace all occurrences of the keyword in the item field value, with the replacement value.</span>
            <span class="task task-6 text-muted" style="display: none;">Replace all occurrences of the keyword in the item field value, with the replacement value, for only specified items.</span>
            <span class="task task-3 text-muted" style="display: none;">Update target field values with the replacement value.</span>
            <span class="task task-5 text-muted" style="display: none;">Update target field values with the replacement value, for only specified items.</span>
        </div>
    </div>
    <div class="form-group row task task-2 task-3">
        <label class="col-sm-2 col-form-label">Parent Item <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtParentItem" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the parent item. All its descendants will be searched including this item.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>    
    <div class="form-group row task task-5 task-6" style="display: none;">
        <label class="col-sm-2 col-form-label">Target Items <span class="required">*</span></label>
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
    <div class="form-group row task task-2 task-3">
        <label class="col-sm-2 col-form-label">Target Item Template</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtTargetItemTemplate" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the template. Child items of only this template will be searched.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Field <span class="task task-3 task-5 required" style="display: none;">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtFieldId" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the field whose value should be searched/updated.</span><br />
            <span id="spFieldId" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="task task-2 task-6 form-group row">
        <label class="col-sm-2 col-form-label">Search For <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span id="spKeyword" class="validation-msg">This field is required</span>
        </div>
    </div>

    <div class="form-group row" style="display: none;">
        <label class="col-sm-2 col-form-label">Match Condition</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlMatchConditions" runat="server" CssClass="form-control" ClientIDMode="Static" Enabled="false">
                <asp:ListItem Text="Contains" Value="1"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="task task-2 task-3 task-5 task-6 form-group row">
        <label class="col-sm-2 col-form-label">New Field Value</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtReplaceWith" runat="server" CssClass="form-control" ClientIDMode="Static" TextMode="MultiLine" Rows="3" autocomplete="on"></asp:TextBox>
            <span class="task task-2 text-muted">If this field is empty, the keyword in content will be replaced with an empty string.</span>
            <span class="task task-3 task-5 task-6 text-muted" style="display: none;">If this field is empty, the field value will be replaced with an empty string.</span>
        </div>
    </div>
    <div class="task task-3 task-5 form-group row" style="display: none;">
        <label class="col-sm-2 col-form-label">New Value Update Condition</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlUpdateConditions" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Select update condition" Value="0"></asp:ListItem>
                <asp:ListItem Text="Prepend value at the start of existing value" Value="1"></asp:ListItem>
                <asp:ListItem Text="Append value at the end of existing value" Value="2"></asp:ListItem>
                <asp:ListItem Text="Replace existing field value with the new value" Value="3"></asp:ListItem>
            </asp:DropDownList>
            <span id="spUpdateCondition" class="validation-msg">Choose a condition</span>
        </div>

    </div>
    <div class="task task-2 task-3 task-5 task-6 form-group row">
        <label class="col-sm-2 col-form-label">Create New Language Version</label>
        <div class="col-sm-4">
            <asp:CheckBox ID="chkCreateVersion" runat="server" CssClass="checkbox-inline" ClientIDMode="Static" Checked="true"></asp:CheckBox>
        </div>
    </div>
    <div class="form-group row task task-2 task-3 task-5 task-6">
        <label class="col-sm-2 col-form-label">Target Languages</label>
        <div class="col-md-4">
            <div class="chkboxlist-container">
                <input type="checkbox" id="chkAllLanguages" />&nbsp;Select all<br />
                <asp:CheckBoxList ID="chkLanguages" runat="server" CssClass="chkboxlist chkLanguage"
                    RepeatLayout="Flow" ClientIDMode="Static">
                </asp:CheckBoxList>
            </div>
            <span id="spLanguageCodes" class="validation-msg">Atleast one language should be selected</span>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
            <button type="button" id="btnReplace" class="btn btn-primary pull-right task task-2 task-6">REPLACE</button>
            <button type="button" id="btnUpdate" class="btn btn-primary pull-right task task-3 task-5" style="display: none;">UPDATE</button>
        </div>
    </div>
    <br />
    <!--RESULT-->
    <div class="row">
        <div class="col-lg-12">
            <div class="divErrorLogContainer" style="display: none;">
                <p id="pError" class="error-msg"></p>
            </div>
            <br />
            <p id="pTaskStatus" class="success-msg"></p>
            <table id="tblResult" class="table table-striped table-bordered table-condensed" style="display: none;">
                <thead>
                    <tr>
                        <th class="col-sm-1 text-center">#</th>
                        <th class="col-sm-3">ITEM ID</th>
                        <th class="col-sm-5">ITEM PATH</th>
                        <th class="col-sm-3 thVariableColumn"></th>
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
                //ClearFieldValues();
                ClearResults();
                $(".task").hide();
                var selectedTaskId = $(this).val();
                $(".task-" + selectedTaskId).show();
                //$("#ddlUpdateConditions").val(0);                
            });           

            $("#btnReplace").click(function () {
                ClearResults();
                var selectedTaskId = $("#ddlTasks").val();

                if (IsValidModel(selectedTaskId)) {
                    var dataModel = GetDataModel(selectedTaskId);
                    OnSubmit(selectedTaskId, dataModel);
                }
            });

            $("#btnUpdate").click(function () {
                ClearResults();
                var selectedTaskId = $("#ddlTasks").val();

                if (IsValidModel(selectedTaskId)) {
                    var dataModel = GetDataModel(selectedTaskId);
                    OnSubmit(selectedTaskId, dataModel);
                }
            });

        });

        function GetDataModel(taskId) {
            var dataModel = {};
            dataModel.TaskId = taskId;            
            dataModel.ParentItemId = $.trim($("#txtParentItem").val());
            dataModel.TargetTemplateId = $.trim($("#txtTargetItemTemplate").val());
            dataModel.MatchCondition = $("#ddlMatchConditions").val();            
            dataModel.TargetFieldId = $.trim($("#txtFieldId").val());

            if (taskId == 2 || taskId == 6) {
                dataModel.Keyword = $("#txtKeyword").val();
            }

            if (taskId == 2 || taskId == 3 || taskId == 5 || taskId == 6) {
                dataModel.ReplaceValue = $("#txtReplaceWith").val();
                dataModel.CreateVersion = $("#chkCreateVersion").is(":checked");
            }

            if (taskId == 3 || taskId == 5) {
                dataModel.UpdateCondition = $.trim($("#ddlUpdateConditions").val());
            }

            if (taskId == 5 || taskId == 6) {
                dataModel.TargetItemPaths = $.trim($("#txtTargetItemPaths").val());
            }

            if (taskId == 2 || taskId == 3 || taskId == 5 || taskId == 6) {
                dataModel.CommaSeparatedLanguageCodes = GetSelectedLanguageCodes().join(',');
            }            

            return dataModel;
        }

        function OnSubmit(task, dataModel) {
            var postUrl = "";

            if (task == 2) {
                postUrl = "updatefields.aspx/ReplaceKeyword";
            }
            else if (task == 3) {
                postUrl = "updatefields.aspx/UpdateFieldValue";
            }
            else if (task == 5) {
                postUrl = "updatefields.aspx/UpdateFieldValueByItemPaths";
            }
            else if (task == 6) {
                postUrl = "updatefields.aspx/ReplaceKeywordForSpecifiedItems";
            }

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
                            //console.log(objData);

                            if (objData != null) {
                                if (objData.Error != null && objData.Error.length > 0) {
                                    $("#pError").html(objData.Error);
                                    $(".divErrorLogContainer").show();
                                    $("html, body").animate({
                                        scrollTop: $(".divErrorLogContainer").offset().top
                                    }, app.scrollDuration);
                                }
                                else {
                                    if (objData.TaskStatus == 1) {
                                        $("#pTaskStatus").html(objData.TaskStatusMessage);
                                        var rows = "";
                                        var index = 1;
                                        $.each(objData.LstValueMatchedItems, function () {
                                            rows += "<tr>";
                                            rows += '<td class="text-center">' + index + '</td>';
                                            rows += "<td>" + this.ItemId + "</td>";
                                            rows += "<td>" + this.ItemPath + "</td>";
                                            rows += "<td>" + this.MatchLog + "</td>";                                            
                                            rows += "</tr>";
                                            index++;
                                        });

                                        $(".thVariableColumn").html("FIELDS WITH A MATCH");
                                        $("#tbResultRows").html(rows);
                                        $("#tblResult").show();
                                        $("html, body").animate({
                                            scrollTop: $("#tblResult").offset().top
                                        }, app.scrollDuration);
                                    }
                                    else if (objData.TaskStatus == 2) {
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

            if (taskId == 2 || taskId == 3) {
                if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                    isValidModel = false;
                    $("#spParentItem").show();
                }
            }

            if (taskId == 2 || taskId == 6) {
                if (app.StringNullOrEmpty($("#txtKeyword").val())) {
                    isValidModel = false;
                    $("#spKeyword").show();
                }
            }

            if (taskId == 3 || taskId == 5) {
                if (app.StringNullOrEmpty($("#txtFieldId").val())) {
                    isValidModel = false;
                    $("#spFieldId").show();
                }
            }

            if (taskId == 5 || taskId == 6) {
                if (app.StringNullOrEmpty($("#txtTargetItemPaths").val())) {
                    isValidModel = false;
                    $("#spTargetItemPaths").show();
                }
            }

            if (taskId == 2 || taskId == 3 || taskId == 5 || taskId == 6) {
                var selectedLanguageCodes = GetSelectedLanguageCodes();

                if (selectedLanguageCodes == null || selectedLanguageCodes.length <= 0) {
                    isValidModel = false;
                    $("#spLanguageCodes").show();
                }
            }

            if (taskId == 3 || taskId == 5) {
                var updateCondition = $("#ddlUpdateConditions").val();
                if (updateCondition == 0 || updateCondition == "0") {
                    isValidModel = false;
                    $("#spUpdateCondition").show();
                }
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

        function ResetValues() {
            ClearFieldValues();
            $("#ddlTasks").val(2);
            $("#ddlUpdateConditions").val(0);
            $(".task").hide();
            $(".task-2").show();
            $("#ddlMatchConditions").val(1);
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


