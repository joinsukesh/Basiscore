<%@ Page Title="Template Mapper" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="templatemapper.aspx.cs" Inherits="SitecoreCustomTools.sitecore.admin.minions.templatemapper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <blockquote class="blockquote bg-warning">
                <p>
                    <strong>USE CASE: </strong>As a developer, whenever you create a <em>Sitecore Template</em>, you also need to have its ID, field references & a related <em>C# class</em>, to access items.
                    Use this tool to generate template references and its mapper class, that you can copy & use in your code.
                </p>
            </blockquote>
        </div>
    </div>
    <hr />
    <div class="row">
        <div class="col-md-4">
            <div class="form-group">
                <label>Template Id</label><span class="required">*</span>
                <input id="txtTemplateId" type="text" class="form-control" autocomplete="off" />
                <span id="spTemplateId" class="validation-msg">This field is required</span>
            </div>
        </div>
        <div class="col-md-4">
            <div class="form-group">
                <label>Template Struct Prefix</label>
                <input id="txtTemplateStructPrefix" type="text" class="form-control" value="References.Templates" />
            </div>
        </div>
        <div class="col-md-4">
            <div class="form-group">
                <button type="button" id="btnGenerate" class="btn btn-primary mtop21">GENERATE</button>
                &nbsp;&nbsp;&nbsp;
                <button type="button" id="btnReset" class="btn btn-default  mtop21">RESET</button>
            </div>
        </div>
    </div>    
    <br />
    <div class="row">
        <div class="col-lg-12">
            <div id="divTemplateMapperResults">
                <div class="form-group">
                    <div class="divError font-red">
                    </div>
                </div>
                <div class="form-group">
                    <label>Template References</label>
                    <textarea id="txtTemplateReferences" class="form-control selectable-readonly" rows="15" readonly>
                    </textarea>
                </div>
                <div class="form-group">
                    <label>Template Class</label>
                    <textarea id="txtTemplateClass" class="form-control selectable-readonly" rows="15" readonly>
                    </textarea>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">

        $(document).ready(function () {

            $("#txtTemplateId").focus();

            $("#txtTemplateId").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    GetTemplateMapperResults();
                    return false;
                }
            });

            //on submit click, validate all form fields
            $("#btnGenerate").click(function () {
                GetTemplateMapperResults();
            });

            $("#btnReset").click(function () {
                ClearFieldValues();
            });

        });

        //validate the form fields
        function ValidateFields() {
            var isValidData = true;
            $(".validation-msg").hide();

            if (app.StringNullOrEmpty($("#txtTemplateId").val())) {
                isValidData = false;
                $("#spTemplateId").show();
            }

            return isValidData;
        }

        function GetTemplateMapperResults() {
            ClearResult();

            if (ValidateFields()) {
                var dataModel = {};
                dataModel.TemplateId = $("#txtTemplateId").val();
                dataModel.TemplateStructPrefix = $("#txtTemplateStructPrefix").val();

                $.ajax({
                    type: "POST",
                    url: "templatemapper.aspx/GetTemplateMapperResults",
                    data: '{templateMapperDataModel: ' + JSON.stringify(dataModel) + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    beforeSend: function () {
                        app.ShowLoadingModal();
                    },
                    success: function (data) {
                        setTimeout(
                            function () {
                                //console.log(data);
                                var objData = JSON.parse(data.d);

                                if (objData != null) {
                                    if (objData.Error != null && objData.Error.length > 0) {
                                        $(".divError").html(objData.Error);
                                    }

                                    if (objData.Status == 1) {
                                        $("#txtTemplateReferences").val(objData.TemplateReferences);
                                        $("#txtTemplateClass").val(objData.TemplateClass);
                                    }
                                    else if (objData.Status == 2) {
                                        window.location.href = "/sitecore/login";
                                    }
                                }
                                app.HideLoadingModal();
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
        }

        //clear all field values
        function ClearFieldValues() {
            $("#txtTemplateId").val("")
            ClearResult();
        }

        function ClearResult() {
            $(".validation-msg").hide();
            $(".divError").html("");
            $("#txtTemplateReferences").val("");
            $("#txtTemplateClass").val("");
        }
    </script>
</asp:Content>
