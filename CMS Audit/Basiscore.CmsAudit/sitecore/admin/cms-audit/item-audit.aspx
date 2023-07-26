<%@ Page Language="C#" MasterPageFile="~/sitecore/admin/cms-audit/Default.Master" EnableViewState="true" AutoEventWireup="true" CodeBehind="item-audit.aspx.cs" Inherits="Basiscore.CmsAudit.sitecore.admin.cms_audit.item_audit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Item Audit Logs | CMS Audit</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form runat="server">
        <asp:HiddenField ID="hdnIsPostBack" runat="server" ClientIDMode="Static" Value="0" />
        <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
            <h5>Item Audit Logs</h5>
        </div>

        <div class="row mb-3">
            <div class="col">
                <label class="form-label">Log Type</label>
                <asp:DropDownList ID="ddlLogTypes" runat="server" ClientIDMode="Static" CssClass="form-select form-select-sm">
                    <asp:ListItem Value="1" Text="Get all logs"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Get only Item Publish logs"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Get only Site Publish logs"></asp:ListItem>
                    <asp:ListItem Value="4" Text="Get only Publish logs (Item or Site)"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col">
                <label class="form-label">From Date <span class="required">*</span></label>
                <asp:TextBox ID="txtFromDate_Ial" runat="server" ClientIDMode="Static" type="date" CssClass="form-control form-control-sm" EnableViewState="true"></asp:TextBox>
            </div>
            <div class="col">
                <label class="form-label">To Date <span class="required">*</span></label>
                <asp:TextBox ID="txtToDate_Ial" runat="server" ClientIDMode="Static" type="date" CssClass="form-control form-control-sm" EnableViewState="true"></asp:TextBox>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-4">
                <label class="form-label">Item ID</label>
                <asp:TextBox ID="txtItemId" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm" EnableViewState="true"></asp:TextBox>
            </div>
            <div class="col-4">
                <label class="form-label">Username</label>
                <asp:TextBox ID="txtUsername" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm" EnableViewState="true"></asp:TextBox>
            </div>
            <div class="col-4">
                <label class="form-label">Language</label>
                <asp:DropDownList ClientIDMode="Static" ID="ddlLanguages" runat="server" CssClass="form-select form-select-sm" AppendDataBoundItems="true" AutoPostBack="false" EnableViewState="true">
                    <asp:ListItem Value="" Text="All"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="d-flex col">
                <span class="required" id="spValidation"></span>
            </div>
            <div class="d-flex col justify-content-end">
                <asp:Button ID="btnIalSubmit" runat="server" OnClick="btnIalSubmit_Click" ClientIDMode="Static" CssClass="btn btn-primary btn-sm" Text="SUBMIT" />
            </div>
        </div>

        <hr />
        <div class="mb-3 divError" id="divError">
            <label class="required">ERROR: </label>
            &nbsp;
        <span id="spError" class="required"></span>
        </div>
        <div class="mb-3" id="divResult_Ial" style="overflow: auto;">
            <asp:GridView ID="gvIal" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm table-striped thead-dark"
                AllowPaging="true" ClientIDMode="Static" AllowSorting="true" Width="100%"
                OnPageIndexChanging="gvIal_PageIndexChanging" PageSize="10" ShowHeaderWhenEmpty="true" EnableViewState="true">
                <Columns>
                    <asp:BoundField DataField="Row_Id" HeaderText="#" HeaderStyle-CssClass="col col-1" HtmlEncode="false" />
                    <asp:BoundField DataField="Item_Info" HeaderText="Item Info" HeaderStyle-CssClass="col col-2" HtmlEncode="false" />
                    <asp:BoundField DataField="Event" HeaderText="Event" HeaderStyle-CssClass="col col-3" HtmlEncode="false" />
                    <asp:BoundField DataField="Actioned_By" HeaderText="Actioned By" HeaderStyle-CssClass="col col-4" HtmlEncode="false" />
                    <asp:BoundField DataField="Comments" HeaderText="Comments" HeaderStyle-CssClass="col col-5" HtmlEncode="false" />
                    <asp:BoundField DataField="Logged_Time" HeaderText="Logged Time" HeaderStyle-CssClass="col col-6" HtmlEncode="false" />
                    <asp:BoundField DataField="Item_Content" HeaderText="Item Content" HeaderStyle-CssClass="col col-7" HtmlEncode="false" />
                </Columns>
                <EmptyDataTemplate>
                    <div align="center">No records found</div>
                </EmptyDataTemplate>
                <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" FirstPageText="First" LastPageText="Last" PageButtonCount="5" />
            </asp:GridView>
        </div>

        <div class="modal fade" id="mdlItemAuditLog" tabindex="-1" aria-labelledby="mdlItemAuditLog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog modal-dialog-scrollable modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col" style="border: 1px solid grey; padding-top: 5px;">
                                    <div style="border: 1px solid grey; background-color: cadetblue; text-align: center">
                                        <h6 style="margin-top: 5px; color: black;">DIFFERENCES</h6>
                                    </div>
                                    <div id="divMdlIalDiff" style="word-break: break-all">
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6" style="border: 1px solid grey; border-top: 0px; padding-top: 5px;">
                                    <div style="border: 1px solid grey; background-color: cadetblue; text-align: center">
                                        <h6 style="margin-top: 5px; color: black;">ITEM DATA BEFORE SAVE</h6>
                                    </div>
                                    <div id="divMdlIdbs" style="word-break: break-all"></div>
                                </div>
                                <div class="col-md-6" style="border: 1px solid grey; border-left: 0px; border-top: 0px; padding-top: 5px;">
                                    <div style="border: 1px solid grey; background-color: cadetblue; text-align: center">
                                        <h6 style="margin-top: 5px; color: black;">ITEM DATA AFTER SAVE</h6>
                                    </div>
                                    <div id="divMdlIdas" style="word-break: break-all"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
