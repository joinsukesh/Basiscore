<%@ Page Title="Find by Value" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="findbyvalue.aspx.cs" Inherits="SitecoreCustomTools.sitecore.admin.minions.findbyvalue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <blockquote class="blockquote bg-warning">
                <p>
                    Find all items in the master database, that have the keyword(s) as a raw value, in any of their fields.<br />
                    <em>Parent Item</em> - Enter ID of the root item. The tool will search this item its descendants.<br />
                    <em>Target Item Template</em> - If you want to search items of a specific template, enter the template ID.
                    <br /><br />
                    <strong>NOTE: </strong>For a faster search, specify the target template ID & select only the reqd. language.
                </p>
            </blockquote>
        </div>
    </div>
    <hr />
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Tasks</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlTasks" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Find Items by Value" Value="1"></asp:ListItem>
                <asp:ListItem Text="Replace Keyword" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Parent Item<span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtParentItem" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the parent item.</span><br />
            <span id="spParentItem" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Item Template</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtTargetItemTemplate" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Enter ID of the template.</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Search For<span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span id="spKeyword" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Match Condition</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlMatchConditions" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Text="Contains" Value="1"></asp:ListItem>
                <asp:ListItem Text="Starts With" Value="2"></asp:ListItem>
                <asp:ListItem Text="Ends With" Value="3"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Look in</label>
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
            <button type="button" id="btnFind" class="btn btn-primary pull-right">FIND ITEMS</button>
            <button type="button" id="btnReplace" class="btn btn-primary pull-right" style="display: none;">FIND & REPLACE</button>
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
            <table id="tblResult" class="table table-striped table-bordered table-condensed" style="display: none;">
                <thead>
                    <tr>
                        <th class="col-sm-7">ITEM PATH</th>
                        <th class="col-sm-5">FIELDS WITH A MATCH</th>
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

                if (IsValidModel(1)) {
                    var dataModel = GetDataModel(1);
                    OnSubmit(1, dataModel);
                }
            });

            $("#btnReplace").click(function () {
                ClearResults();

                if (IsValidModel(2)) {
                    var dataModel = GetDataModel(2);
                    OnSubmit(2, dataModel);
                }
            });

        });

        function GetDataModel(inputType) {
            var dataModel = {};
            dataModel.TaskId = $("#ddlTasks").val();
            dataModel.ParentItemId = $("#txtParentItem").val();
            dataModel.TargetTemplateId = $("#txtTargetItemTemplate").val();
            dataModel.Keyword = $("#txtKeyword").val();
            dataModel.MatchCondition = $("#ddlMatchConditions").val();

            if (inputType == 2) {

            }

            dataModel.CommaSeparatedLanguageCodes = GetSelectedLanguageCodes().join(',');
            return dataModel;
        }

        function OnSubmit(task, dataModel) {
            var postUrl = task == 1 ? "findbyvalue.aspx/FindItems" : "findbyvalue.aspx/ReplaceKeyword"
            $.ajax({
                type: "POST",
                url: postUrl,
                data: JSON.stringify({ findByValueDataModel: dataModel }),
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
                            console.log(objData);

                            if (objData != null) {
                                if (objData.Error != null && objData.Error.length > 0) {
                                    $("#pError").html(objData.Error);
                                    $(".divErrorLogContainer").show();
                                }
                                else {
                                    if (objData.TaskStatus == 1) {
                                        var rows = "";
                                        $.each(objData.LstValueMatchedItems, function () {
                                            rows += "<tr>";
                                            rows += "<td>" + this.ItemPath + "</td>";
                                            rows += "<td>" + this.MatchLog + "</td>";
                                            rows += "</tr>";
                                        });
                                        $("#tbResultRows").html(rows);
                                        $("#tblResult").show();
                                        $("html, body").animate({
                                            scrollTop: $("#tblResult").offset().top
                                        }, 1000);
                                    }
                                    else if (objData.TaskStatus == 2) {
                                        window.location.href = "/sitecore/login";
                                    }
                                }
                            }
                        }, 1000);
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

        function IsValidModel(task) {
            var isValidModel = true;

            if (app.StringNullOrEmpty($("#txtParentItem").val())) {
                isValidModel = false;
                $("#spParentItem").show();
            }

            if (app.StringNullOrEmpty($("#txtKeyword").val())) {
                isValidModel = false;
                $("#spKeyword").show();
            }

            if (task == 2) {
                if (app.StringNullOrEmpty($("#txtReplaceValue").val())) {
                    isValidModel = false;
                    $("#spReplaceValue").show();
                }
            }

            var selectedLanguageCodes = GetSelectedLanguageCodes();

            if (selectedLanguageCodes == null || selectedLanguageCodes.length <= 0) {
                isValidModel = false;
                $("#spLanguageCodes").show();
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

            //hide all validation msgs
            $(".validation-msg").hide();

            //clear all textbox values
            $("input[type='text'], textarea").each(function () {
                $(this).val("");
            });

            $("#ddlTasks").val(1);
            $("#ddlMatchConditions").val(1);

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
            $(".divErrorLogContainer").hide();
        }

    </script>
</asp:Content>

