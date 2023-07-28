<%@ Page Language="C#" MasterPageFile="~/sitecore/admin/cms-audit/Default.Master" AutoEventWireup="true" CodeBehind="purge-logs.aspx.cs" Inherits="Basiscore.CmsAudit.sitecore.admin.cms_audit.purge_logs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Purge Logs | CMS Audit</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h5>Purge Logs</h5>
    </div>
    <form runat="server">
        <div class="row mb-3">
            <div class="col">
                <h6 style="color: chocolate">Item Audit Log Summary</h6>
                <br />
                <table class="table table-bordered table-sm tblLogSummary">
                    <thead class="thead-dark">
                        <tr>
                            <th>First Record Date</th>
                            <th>Recent Record Date</th>
                            <th>Total Records</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:Literal ID="ltFirstRecordDate_Ial" runat="server"></asp:Literal></td>
                            <td>
                                <asp:Literal ID="ltRecentRecordDate_Ial" runat="server"></asp:Literal></td>
                            <td>
                                <asp:Literal ID="ltTotalRecords_Ial" runat="server"></asp:Literal></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col">
                <label class="form-label">From Date <span class="required">*</span></label>
                <input type="date" id="txtFromDate_Pl" class="form-control form-control-sm">
            </div>
            <div class="col">
                <label class="form-label">To Date <span class="required">*</span></label>
                <input type="date" id="txtToDate_Pl" class="form-control form-control-sm">
            </div>
            <div class="col">
                <div style="line-height: 31px;">&nbsp;</div>
                <button id="btnDeleteIaLogs" type="button" class="btn btn-danger btn-sm">DELETE ITEM AUDIT LOGS</button>
            </div>
        </div>
        <div class="row">
            <div class="d-flex col">
                <span class="required" id="spValidation"></span>
            </div>
        </div>
        <div class="row mb-3 divError" id="divError">
            <label class="required">ERROR: </label>
            &nbsp;
            <span id="spError" class="required"></span>
        </div>
        <div class="mb-3 row" id="divResult" style="font-weight: bold; color: forestgreen">
        </div>
    </form>
    <div class="modal fade" id="mdlLogsDeletedStatus" tabindex="-1" aria-labelledby="mdlLogsDeletedStatus" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col" style="text-align: center;">
                                <div id="divLogsDeletedStatus" style="color: forestgreen; font-weight: bold">
                                    
                                </div><br />
                                <button id="btnOk_lds" type="button" class="btn btn-warning" data-deletedRows="">OK</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
