<%@ Page Title="Install Package" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="installpackage.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.installpackage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-coral bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>
                                Use this utility to install a Sitecore package that is uploaded in the instance.<br />
                                The items will be installed using the MERGE > APPEND option.
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
            <div class="">
                <div class="form-group">
                    <label>Choose the package to be installed<span class="required">*</span></label>
                    <br />
                    <span id="spChoosePackage" class="validation-msg" style="display: block;">Choose a package</span><br />
                    <div class="panel panel-default" style="max-height: 300px; overflow: scroll;">
                        <div class="panel-body">
                            <asp:ListView ID="lvPackageNames" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                                ItemPlaceholderID="itemPlaceHolder1">
                                <LayoutTemplate>
                                    <table id="tblPackageNames" class="table table-bordered table-condensed">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th style="text-align: center;">#
                                                </th>
                                                <th>Package Name
                                                </th>
                                                <th>Uploaded Date
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td style="text-align: center; vertical-align: middle">
                                        <input type="radio" class="rbPackageName" name="rblPackageNames" data-pname="<%# Eval("Name") %>" />
                                    </td>
                                    <td>
                                        <%# Eval("Name") %>
                                    </td>
                                    <td>
                                        <%# Eval("CreationTime") %>
                                    </td>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-12">
                    <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
                    <button id="btnInstallPackage" type="button" class="btn btn-primary pull-right">INSTALL PACKAGE</button>
                </div>
            </div>
        </div>
    </div>
    <br />
    <!--RESULT-->
    <div class="row">
        <div class="col-lg-12">
            <div id="divStatus" style="font-size: 14px; font-weight: bold">
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            Init();

            $("#btnReset").click(function () {
                ClearFieldValues();
            });

            $("#btnInstallPackage").click(function () {
                ClearResults();

                if (IsValidModel()) {
                    OnSubmit();
                }
            });

        });

        function OnSubmit() {
            var selectedPackageName = GetSelectedPackageName();

            $.ajax({
                type: "POST",
                url: "installpackage.aspx/InstallSitecorePackage",
                data: JSON.stringify({ packageName: selectedPackageName }),
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
                                if (objData.StatusCode == 0 || objData.StatusCode == 1) {
                                    var status = "";
                                    status += '<span style="font-weight:bold;">STATUS: </span>';

                                    if (objData.StatusCode == 0) {
                                        status += '<span style="color:red;"> ERROR- ';
                                        status += objData.StatusMessage;
                                        status += '</span>';
                                    }
                                    else if (objData.StatusCode == 1) {
                                        status += '<span style="color:forestgreen;">';
                                        status += objData.StatusMessage;
                                        status += '</span>';
                                    }

                                    $("#divStatus").html(status);
                                    $("html, body").animate({
                                        scrollTop: $("#divStatus").offset().top
                                    }, app.scrollDuration);
                                }
                                else if (objData.StatusCode == 2) {
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

        function IsValidModel() {
            var isValidModel = true;
            var packageName = GetSelectedPackageName();

            if (app.StringNullOrEmpty(packageName)) {
                isValidModel = false;
                $("#spChoosePackage").show();
            }

            return isValidModel;
        }

        ///clear all field values
        function ClearFieldValues() {

            //hide all validation msgs
            $(".validation-msg").hide();

            //uncheck all radios
            $(".rbPackageName").prop("checked", "");

            ClearResults();
        }

        //clear result section
        function ClearResults() {
            $(".validation-msg").hide();
            $("#divStatus").html("");
        }

        function GetSelectedPackageName() {
            var packageName = $('input[name="rblPackageNames"]:checked').attr("data-pname");
            packageName = $.trim(packageName);
            return packageName;
        }

        function ResetForm() {
            ClearFieldValues();
        }

        function Init() {
            ResetForm();
        }
    </script>
</asp:Content>
