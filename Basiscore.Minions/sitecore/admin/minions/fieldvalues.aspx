<%@ Page Title="Field Values" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="fieldvalues.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.fieldvalues" %>

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
                                Use this tool for these item field related tasks, in the master database.<br />
                                <ul>
                                    <li>Find all items that have the keyword as a raw value, in any of their fields.</li>
                                    <li>Get items and their field values for a field.</li>
                                    <li>Replace the keyword in a field value.</li>
                                    <li>Update the field value.</li>
                                </ul>
                                <br />
                                <strong>NOTE: </strong>For a faster search, specify the target template, field IDs & select only the required language.
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
            <asp:dropdownlist id="ddlTasks" runat="server" cssclass="form-control" clientidmode="Static">
                <asp:ListItem Text="Find Items by Field Value" Value="1"></asp:ListItem>
                <asp:ListItem Text="Get Items and Field Values by Field" Value="4"></asp:ListItem>
                <asp:ListItem Text="Replace Keyword in Field Value" Value="2"></asp:ListItem>
                <asp:ListItem Text="Update Field Value" Value="3"></asp:ListItem>
            </asp:dropdownlist>
            <span class="task task-1 text-muted">Find all items under a parent node whose field value <strong>Contains</strong> the keyword.</span>
            <span class="task task-4 text-muted">Get all items under a parent node with field values, having this field.</span>
            <span class="task task-2 text-muted" style="display: none;">Replace all occurrences of the keyword in the item field value, with the replacement value.</span>
            <span class="task task-3 text-muted" style="display: none;">Update target field values with the replacement value.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Parent Item <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:textbox id="txtParentItem" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter ID of the parent item. All its descendants will be searched including this item.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Item Template</label>
        <div class="col-sm-4">
            <asp:textbox id="txtTargetItemTemplate" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter ID of the template. Child items of only this template will be searched.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Field <span class="task task-3 task-4 required" style="display: none;">*</span></label>
        <div class="col-sm-4">
            <asp:textbox id="txtFieldId" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter ID of the field whose value should be searched/updated.</span><br />
            <span id="spFieldId" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="task task-1 task-2 form-group row">
        <label class="col-sm-2 col-form-label">Search For <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:textbox id="txtKeyword" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span id="spKeyword" class="validation-msg">This field is required</span>
        </div>
    </div>

    <div class="form-group row" style="display: none;">
        <label class="col-sm-2 col-form-label">Match Condition</label>
        <div class="col-sm-4">
            <asp:dropdownlist id="ddlMatchConditions" runat="server" cssclass="form-control" clientidmode="Static" enabled="false">
                <asp:ListItem Text="Contains" Value="1"></asp:ListItem>
            </asp:dropdownlist>
        </div>
    </div>
    <div class="task task-2 task-3 form-group row" style="display: none;">
        <label class="col-sm-2 col-form-label">New Field Value</label>
        <div class="col-sm-4">
            <asp:textbox id="txtReplaceWith" runat="server" cssclass="form-control" clientidmode="Static" textmode="MultiLine" rows="3" autocomplete="on"></asp:textbox>
            <span class="task task-2 text-muted" style="display: none;">If this field is empty, the keyword in content will be replaced with an empty string.</span>
            <span class="task task-3 text-muted" style="display: none;">If this field is empty, the field value will be replaced with an empty string.</span>
        </div>
    </div>
    <div class="task task-2 task-3 form-group row" style="display: none;">
        <label class="col-sm-2 col-form-label">Create New Language Version</label>
        <div class="col-sm-4">
            <asp:checkbox id="chkCreateVersion" runat="server" cssclass="checkbox-inline" clientidmode="Static" checked="true"></asp:checkbox>
        </div>
    </div>
    <div class="form-group row task task-1 task-2 task-3">
        <label class="col-sm-2 col-form-label">Target Languages</label>
        <div class="col-md-4">
            <div class="chkboxlist-container">
                <input type="checkbox" id="chkAllLanguages" />&nbsp;Select all<br />
                <asp:checkboxlist id="chkLanguages" runat="server" cssclass="chkboxlist chkLanguage"
                    repeatlayout="Flow" clientidmode="Static">
                </asp:checkboxlist>
            </div>
            <span id="spLanguageCodes" class="validation-msg">Atleast one language should be selected</span>
        </div>
    </div>
    <div class="form-group row task task-4" style="display: none;">
        <label class="col-sm-2 col-form-label">Target Language</label>
        <div class="col-sm-4">            
            <asp:dropdownlist id="ddlLanguages" runat="server" cssclass="form-control" clientidmode="Static">
            </asp:dropdownlist>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
            <button type="button" id="btnFind" class="btn btn-primary pull-right task task-1 task-4">FIND ITEMS</button>
            <button type="button" id="btnReplace" class="btn btn-primary pull-right task task-2" style="display: none;">REPLACE</button>
            <button type="button" id="btnUpdate" class="btn btn-primary pull-right task task-3" style="display: none;">UPDATE</button>
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

            $("#btnReset").click(function () {
                ClearFieldValues();
            });

            $("#ddlTasks").change(function () {
                $(".validation-msg").hide();
                $(".task").hide();
                var selectedTaskId = $(this).val();
                $(".task-" + selectedTaskId).show();                
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

            $("#btnFind").click(function () {
                ClearResults();
                var selectedTaskId = $("#ddlTasks").val();

                if (IsValidModel(selectedTaskId)) {
                    var dataModel = GetDataModel(selectedTaskId);
                    OnSubmit(selectedTaskId, dataModel);
                }
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

            if (taskId == 1 || taskId == 2) {
                dataModel.Keyword = $("#txtKeyword").val();
            }

            if (taskId == 2 || taskId == 3) {
                dataModel.ReplaceValue = $("#txtReplaceWith").val();
                dataModel.CreateVersion = $("#chkCreateVersion").is(":checked");
            }

            if (taskId == 1 || taskId == 2 || taskId == 3) {
                dataModel.CommaSeparatedLanguageCodes = GetSelectedLanguageCodes().join(',');
            }
            else if (taskId == 4){
                dataModel.TargetLanguageCode = $("#ddlLanguages").val();
            }

            return dataModel;
        }

        function OnSubmit(task, dataModel) {
            var postUrl = "";

            if (task == 1) {
                postUrl = "fieldvalues.aspx/FindItems";
            }
            else if (task == 2) {
                postUrl = "fieldvalues.aspx/ReplaceKeyword";
            }
            else if (task == 3) {
                postUrl = "fieldvalues.aspx/UpdateFieldValue";
            }
            else if (task == 4) {
                postUrl = "fieldvalues.aspx/GetItemsAndFieldValues";
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

                                        if (task == 4) {
                                            $(".thVariableColumn").html(objData.ColumnName);
                                        }
                                        else {
                                            $(".thVariableColumn").html("FIELDS WITH A MATCH");
                                        }
                                        
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

            if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                isValidModel = false;
                $("#spParentItem").show();
            }

            if (taskId == 1 || taskId == 2) {
                if (app.StringNullOrEmpty($("#txtKeyword").val())) {
                    isValidModel = false;
                    $("#spKeyword").show();
                }
            }

            if (taskId == 3 || taskId == 4) {
                if (app.StringNullOrEmpty($("#txtFieldId").val())) {
                    isValidModel = false;
                    $("#spFieldId").show();
                }
            }

            if (taskId == 1 || taskId == 2 || taskId == 3) {
                var selectedLanguageCodes = GetSelectedLanguageCodes();

                if (selectedLanguageCodes == null || selectedLanguageCodes.length <= 0) {
                    isValidModel = false;
                    $("#spLanguageCodes").show();
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

        ///clear all field values
        function ClearFieldValues() {

            ///hide all validation msgs
            $(".validation-msg").hide();

            ///clear all textbox values
            $("input[type='text'], textarea").each(function () {
                $(this).val("");
            });

            $("#ddlTasks").val(1);
            $(".task").hide();
            $(".task-1").show();
            $("#ddlMatchConditions").val(1);
            $("#chkCreateVersion").prop("checked", "checked");

            ///uncheck all checkboxes
            $(".chkboxlist input[type='checkbox']").prop("checked", "");
            $("#chkAllLanguages").prop("checked", "");

            $("#tbPipRows").html("");
            $("#tblPip").hide();

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

    </script>
</asp:Content>

