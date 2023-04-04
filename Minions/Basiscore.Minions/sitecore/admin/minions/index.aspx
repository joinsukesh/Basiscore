<%@ Page Title="Minions" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <div class="row">
        <div class="col-md-12">
            <p>
                This application is a suite of simple utilities, aimed to help Sitecore developers in a few of their everyday tasks.<br />
                Choose a tool from the menu to proceed.
            </p>
        </div>
    </div>
    <div class="row m-top-20p">
        <div class="col-md-12 text-center" style="min-height: 60px;">
            <blockquote class="blockquote minion-quote text-right animated slow">
                <div class="row">
                    <div class="col-sm-11">
                        <p class="mb-0 pQuote"></p>
                    </div>
                    <div class="col-sm-1" style="padding: 0px;">
                        <img class="pull-left m-top-m8p imgQuote" src="" style="width: 35px; height: auto;" />
                    </div>
                </div>
            </blockquote>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            ShowMinionQuote();
        });

        function ShowMinionQuote() {
            setTimeout(function () {
                var quote = GetRandomMinionQuote();
                var randomNumber = app.GetRandomNumber(1, 10);
                $(".pQuote").html(quote);
                $(".imgQuote").prop("src", "assets/img/home/" + randomNumber + ".gif");
                $(".minion-quote").addClass("blockquote-border-left fadeInDown");
            }, app.minionQuoteDelay);
        }
    </script>
</asp:Content>

<%--
INSTRUCTIONS HEADER BG COLORS USED IN ORDER
-------------------------------------------
fieldvalues : bg-cadetblue
importexceldata : bg-warning
publishitems : bg-rosybrown
generatepackage : bg-burlywood
installpackage : bg-coral
addrendering : bg-darkcyan
removerenderings : bg-darkseagreen
updatedatasource : bg-burlywood
renderingusage : bg-darksalmon
templatemapper : bg-darkkhaki

--%>

