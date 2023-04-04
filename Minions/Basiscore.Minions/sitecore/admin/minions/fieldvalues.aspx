<%@ Page Title="Search Fields" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="fieldvalues.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.fieldvalues" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <asp:hiddenfield clientidmode="Static" id="hdnSessionId" runat="server" />
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
                                Use this tool for these item field search related tasks.<br />
                                <ul>
                                    <li>Find all items that have the keyword as a raw value, in any of their fields.</li>
                                    <li>Get items and their field values for a field.</li>
                                </ul>
                                <br />
                                <strong>NOTE: </strong>For a faster search, specify the target template and field IDs.
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
            </asp:dropdownlist>
            <span class="task task-1 text-muted">Find all items under a parent node whose field value <strong>Contains</strong> the keyword.</span>
            <span class="task task-4 text-muted" style="display: none;">Get all items under a parent node with field values, having this field.</span>
        </div>
    </div>
    <div class="form-group row task task-1 task-4">
        <label class="col-sm-2 col-form-label">Parent Item <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:textbox id="txtParentItem" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter ID of the parent item. All its descendants will be searched including this item.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Source Database</label>
        <div class="col-sm-4">
            <asp:textbox id="txtDatabase" runat="server" cssclass="form-control" clientidmode="Static" text="master"></asp:textbox>
        </div>
    </div>
    <div class="form-group row task task-1 task-4">
        <label class="col-sm-2 col-form-label">Target Item Template</label>
        <div class="col-sm-4">
            <asp:textbox id="txtTargetItemTemplate" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter ID of the template. Child items of only this template will be searched.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Field Input Type</label>
        <div class="col-sm-4">
            <asp:dropdownlist id="ddlTargetFieldInputTypes" runat="server" cssclass="form-control" clientidmode="Static">
                <asp:ListItem Text="Field ID" Value="1"></asp:ListItem>
                <asp:ListItem Text="Field Name" Value="2"></asp:ListItem>
            </asp:dropdownlist>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Field <span class="task task-4 required" style="display: none;">*</span></label>
        <div class="col-sm-4">
            <asp:textbox id="txtFieldId" runat="server" cssclass="form-control" clientidmode="Static"></asp:textbox>
            <span class="text-muted">Enter <span id="spTfit">ID</span> of the field whose value should be searched/updated.</span><br />
            <span id="spFieldId" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="task task-1 form-group row">
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
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Language</label>
        <div class="col-sm-4">
            <asp:dropdownlist id="ddlLanguages" runat="server" cssclass="form-control" clientidmode="Static">
            </asp:dropdownlist>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
            <button type="button" id="btnFind" class="btn btn-primary pull-right">FIND ITEMS</button>
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
             <p>
                <button type="button" id="btnDownload" class="btn btn-warning pull-right" style="display: none;">DOWNLOAD</button>
                <br />
                <br />
            </p>
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
                //ResetTargetFieldInputType();
            });

            $("#ddlTargetFieldInputTypes").change(function () {
                var type = $(this).val();
                if (type == 2 || type == "2") {
                    $("#spTfit").html("exact name (case sensitive)");
                }
                else {
                    $("#spTfit").html("ID");
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

            $("#btnDownload").click(function () {
                var taskId = $("#ddlTasks").val();
                var sessionId = $("#hdnSessionId").val();

                if (taskId == 1) {
                    app.DownloadData('finditems', sessionId);
                }
                else if (taskId == 4) {
                    app.DownloadData('fieldvalues', sessionId);
                }                
            });

        });

        function GetDataModel(taskId) {
            var dataModel = {};
            dataModel.SessionId = $("#hdnSessionId").val();
            dataModel.TaskId = taskId;
            dataModel.InputSourceDatabase = $.trim($("#txtDatabase").val());
            dataModel.ParentItemId = $.trim($("#txtParentItem").val());
            dataModel.TargetTemplateId = $.trim($("#txtTargetItemTemplate").val());
            dataModel.MatchCondition = $("#ddlMatchConditions").val();
            dataModel.TargetFieldInputType = $("#ddlTargetFieldInputTypes").val();
            dataModel.TargetFieldId = $.trim($("#txtFieldId").val());
            dataModel.TargetLanguageCode = $("#ddlLanguages").val();

            if (taskId == 1) {
                dataModel.Keyword = $("#txtKeyword").val();
            }

            return dataModel;
        }

        function OnSubmit(task, dataModel) {
            var postUrl = "";

            if (task == 1) {
                postUrl = "fieldvalues.aspx/FindItems";
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
                                        $("#btnDownload").show();
                                        var rows = "";
                                        var index = 1;
                                        $.each(objData.LstValueMatchedItems, function () {
                                            rows += "<tr>";
                                            rows += '<td class="text-center">' + index + '</td>';
                                            rows += "<td>" + this.ItemId + "</td>";
                                            rows += "<td>" + this.ItemPath + "</td>";

                                            if (task == 4) {
                                                rows += "<td><xmp>" + this.MatchLog + "</xmp></td>";
                                            }
                                            else {
                                                rows += "<td>" + this.MatchLog + "</td>";
                                            }

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

            if (taskId == 1 || taskId == 2 || taskId == 3 || taskId == 4) {
                if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                    isValidModel = false;
                    $("#spParentItem").show();
                }
            }

            if (taskId == 1 || taskId == 2 || taskId == 6) {
                if (app.StringNullOrEmpty($("#txtKeyword").val())) {
                    isValidModel = false;
                    $("#spKeyword").show();
                }
            }

            if (taskId == 3 || taskId == 4 || taskId == 5) {
                if (app.StringNullOrEmpty($("#txtFieldId").val())) {
                    isValidModel = false;
                    $("#spFieldId").show();
                }
            }

            return isValidModel;
        }

        ///clear all field values
        function ResetTargetFieldInputType() {
            $("#ddlTargetFieldInputTypes").val(1);
            $("#spTfit").html("ID");
        }

        function ResetValues() {
            ClearFieldValues();
            $("#ddlTasks").val(1);
            $(".task").hide();
            $(".task-1").show();
            $("#ddlMatchConditions").val(1);
            $("#btnDownload").hide();
            $("#d-iframe").remove();
            $("#ddlLanguages").prop('selectedIndex', 0);
        }

        ///clear all field values
        function ClearFieldValues() {
            app.DefaultResetForm();
            $("#txtDatabase").val("master");
            ResetTargetFieldInputType();
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

