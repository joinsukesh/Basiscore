<%@ Page Title="Generate Package" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="generatepackage.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.generatepackage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <asp:HiddenField ID="hdnPostbackComplete" runat="server" ClientIDMode="Static" Value="0" />
    <asp:HiddenField ID="hdnFileName" runat="server" ClientIDMode="Static" Value="" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-burlywood bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                Use this utility to quickly generate a Sitecore package by specifying the item paths.<br />
                                You can download the package just after it is created. It will also be created in the instance's packages folder.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Source Database<span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtDatabase" runat="server" CssClass="form-control" ClientIDMode="Static" Text="master"></asp:TextBox>
            <span id="spDatabase" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Package Name<span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtPackageName" runat="server" CssClass="form-control" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
            <span class="text-muted">Use only alphanumerics, dot, hyphen, underscore</span><br />
            <span id="spPackageName" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Author</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtAuthor" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Version</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtVersion" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">Use only alphanumerics, dot, hyphen, underscore</span><br />
            <span id="spVersion" class="validation-msg">This field is required</span>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="">
                <div class="form-group">
                    <label>Items to be included with subitems</label><br />
                    <span class="text-muted">Enter item paths separated by line breaks.<br />
                        e.g:&nbsp;<i>/sitecore/content/item1<br />
                            &nbsp;&nbsp;&emsp;/sitecore/content/item2</i>
                    </span>
                    <asp:TextBox ID="txtIncludeWithSubitems" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ClientIDMode="Static"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Items to be included singly</label><br />
                    <span class="text-muted">Enter item paths separated by line breaks.<br />
                        e.g:&nbsp;<i>/sitecore/content/item1<br />
                            &nbsp;&nbsp;&emsp;/sitecore/content/item2</i>
                    </span>
                    <asp:TextBox ID="txtIncludeItems" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ClientIDMode="Static"></asp:TextBox>
                    <br />
                    <span id="spItemPaths" class="validation-msg">Atleast one item path should be provided</span>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-12">
                    <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
                    <asp:Button ID="btnGeneratePackage" runat="server" CssClass="btn btn-primary pull-right" ClientIDMode="Static" Text="GENERATE PACKAGE" OnClick="btnGeneratePackage_Click" />
                </div>
            </div>
        </div>
    </div>
    <br />
    <div class="modal modal-bg" id="mdlStatus" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog modal-sm-modal-dialog-centered top20p">
            <div class="modal-content">
                <div class="modal-header">
                    Status
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblError" runat="server" ClientIDMode="Static" CssClass="error-msg" Visible="false"></asp:Label>
                    <asp:Label ID="lblSuccess" runat="server" ClientIDMode="Static" CssClass="success-msg" Visible="false"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnCloseStatusModal" class="btn btn-default pull-left">Close</button>
                    <asp:Button ID="btnDownload" runat="server" CssClass="btn btn-warning pull-right" ClientIDMode="Static" Text="Download" OnClick="btnDownload_Click" />
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            app.HideLoadingModal();

            ///if it pageload after postback, then show the status modal.
            var isPostbackComplete = $("#hdnPostbackComplete").val();

            if (isPostbackComplete == 1 || isPostbackComplete == "1") {
                $("#mdlStatus").modal("show");
            }

            $("#btnReset").click(function () {
                ClearFieldValues();
            });

            $("#btnGeneratePackage").click(function () {
                ClearResults();

                if (IsValidModel()) {
                    app.ShowLoadingModal();
                    return true;
                }
                else {
                    return false;
                }
            });

            $("#btnCloseStatusModal").click(function () {
                $("#mdlStatus").modal("hide");
            });

        });

        function IsValidModel() {
            var isValidModel = true;

            if (app.StringNullOrEmpty($("#txtDatabase").val())) {
                isValidModel = false;
                $("#txtDatabase").focus();
                $("#spDatabase").show();
            }

            if (app.StringNullOrEmpty($("#txtPackageName").val())) {
                isValidModel = false;
                $("#txtPackageName").focus();
                $("#spPackageName").show();
            }

            var hasItemPathForFirstBox = false;
            var hasItemPathForSecondBox = false;

            if (!app.StringNullOrEmpty($("#txtIncludeWithSubitems").val())) {
                hasItemPathForFirstBox = true;
            }

            if (!app.StringNullOrEmpty($("#txtIncludeItems").val())) {
                hasItemPathForSecondBox = true;
            }

            if (!hasItemPathForFirstBox && !hasItemPathForSecondBox) {
                isValidModel = false;
                $("#spItemPaths").show();
                $("#spItemPaths").focus();
            }

            return isValidModel;
        }

        ///clear all field values
        function ClearFieldValues() {

            ///hide all validation msgs
            $(".validation-msg").hide();

            ///clear all textbox values
            $("input[type='text'], textarea").each(function () {
                $(this).val("");
            });

            $("#txtDatabase").val("master");
            ClearResults();
        }

        ///clear result section
        function ClearResults() {
            $(".validation-msg").hide();
            $(".divStatusContainer").html("");
            $(".divStatusContainer").hide();
            $("#hdnPostbackComplete").val("");
            $("#hdnFileName").val("");
            $("#lblError").html("");
            $("#lblSuccess").html("");
        }
    </script>
</asp:Content>

