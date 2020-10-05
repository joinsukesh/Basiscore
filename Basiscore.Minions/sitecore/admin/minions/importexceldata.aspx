<%@ Page Title="Import Excel Data" Language="C#" MasterPageFile="~/sitecore/admin/minions/Default.Master" AutoEventWireup="true" CodeBehind="importexceldata.aspx.cs" Inherits="Basiscore.Minions.sitecore.admin.minions.importexceldata" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" class="hdnModuleName" value="<%=Page.Title%>" />
    <asp:HiddenField ID="hdnPostbackComplete" runat="server" ClientIDMode="Static" />
    <div class="row">
        <div class="col-md-12">
            <div class="panel-group">
                <div class="panel summary-panel">
                    <a class="anc-summary-panel-heading-section" data-toggle="collapse" href="#collapse1">
                        <div class="bg-warning bg-noise summary-panel-heading-section">
                            <h4 class="panel-title">Instructions
                            </h4>
                            <span class="expand-collapse-icon fa fa-chevron-down"></span>
                        </div>
                    </a>
                    <div id="collapse1" class="panel-collapse collapse">
                        <div class="panel-body summary-panel-body-section">
                            <p>Import data from an excel sheet to create or update Sitecore items.</p>
                            <ul>
                                <li>The Sitecore template(s) with which the items should be created, should already exist in your CMS.</li>
                                <li>Use only an excel file with <strong>.xlsx</strong> extension.</li>
                                <li>The data should be in the first worksheet of the excel file.</li>
                                <li>The first row should have column headers with these names and exactly in this order - <strong>ITEM_NAME</strong> | <strong>ITEM_TEMPLATE</strong> | <strong>PARENT_ITEM</strong></li>
                                <li>Each row after that, should have information about one item.
                            <ul>
                                <li><em>ITEM_NAME</em> - Enter a valid Sitecore item name</li>
                                <li><em>ITEM_TEMPLATE</em> - Enter path or ID of the template with which the item should be created.</li>
                                <li><em>PARENT_ITEM</em> - Enter path or ID of the parent item under which this item should be created.</li>
                            </ul>
                                </li>
                                <li>You can also specify field names and their values, when creating or updating the items. To do this, add the field name(s) after the third column.<br />
                                    For e.g., <em>ITEM_NAME</em> | <em>ITEM_TEMPLATE</em> | <em>PARENT_ITEM</em> | <em>MyField1</em> | <em>MyField2</em> | ...<br />
                                    Ensure that the column name matches exactly with the item's field name.<br />
                                    It is not necessary that all rows should have items info of only one template. You can have the data for items of different templates in the same sheet. If an item does not have that field, leave the respective cell empty. Check the sample sheet for reference.
                                </li>
                                <li>If you have marked '<i>Update Field Values if Item Exists</i>', and the related cell value is empty, the respective field in CMS will be replaced with an empty string.
                                </li>
                                <li>Click <a href="/sitecore/admin/minions/assets/files/ImportExcelDataSample.xlsx"><strong>here</strong></a> to download the template excel file.<br />
                                    You can see that, the <em>Country</em> & <em>State</em> are different templates, with these fields.<br />
                                    Country - Title, Code, Currency<br />
                                    State - Title, Code, Minimum Wage<br />
                                    Refer the sheet to understand how to create/update items with field values.
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>            
        </div>
    </div>
    <hr />
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Choose File <span class="required">*</span></label>
        <div class="col-sm-4">
            <asp:FileUpload ID="fupExcel" runat="server" ClientIDMode="Static" CssClass="form-control" />
            <span class="text-muted">Select the Excel file</span><br />
            <span id="spFupExcel" class="validation-msg"></span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Start from Row</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtStartFromRow" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">The app will read data from this row of Excel sheet.</span><br />
            <span id="spStartFromRow" class="validation-msg"></span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">End at Row</label>
        <div class="col-sm-4">
            <asp:TextBox ID="txtEndAtRow" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            <span class="text-muted">The app will read data upto this row of Excel sheet</span><br />
            <span id="spEndAtRow" class="validation-msg"></span>
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label m-top-m5">Update Field Values if Item Exists</label>
        <div class="col-sm-4">
            <asp:CheckBox ID="chkUpdateFieldValues" runat="server" CssClass="chkbox" ClientIDMode="Static" />
        </div>
    </div>
    <div class="form-group row">
        <label class="col-sm-2 col-form-label">Target Language</label>
        <div class="col-sm-4">
            <asp:DropDownList ID="ddlLanguages" runat="server" CssClass="form-control" ClientIDMode="Static">
            </asp:DropDownList>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <button type="button" id="btnReset" class="btn btn-default pull-left">RESET</button>
            <asp:Button ID="btnImportData" runat="server" CssClass="btn btn-primary pull-right" ClientIDMode="Static" Text="IMPORT DATA" OnClick="btnImportData_Click" />
        </div>
    </div>
    <br />
    <!--RESULT-->
    <div class="row divResult">
        <div class="col-lg-12">
            <div class="divStatus">
                <asp:Literal ID="ltStatus" runat="server" ClientIDMode="Static">
                </asp:Literal>
            </div>           
            <br />
            <div id="tblResult" runat="server" class="tblResult">
            </div>
        </div>
    </div>

    <script>
        var selectedFile;

        $(document).ready(function () {
            Init();            

            $("#fupExcel").change(function (e) {
                selectedFile = null;

                if (!app.StringNullOrEmpty($("#fupExcel").val())) {
                    selectedFile = e.target.files[0];
                }
            });

            //on submit click, validate all form fields
            $("#btnImportData").click(function () {
                ClearResults();

                if (IsValidModel()) {
                    app.ShowLoadingModal();
                    return true;
                }
                else {
                    return false;
                }
            });

            $("#btnReset").click(function () {
                ClearFieldValues();
                $("#hdnPostbackComplete").val("");
                $("#ltStatus").html("");
                $(".tblResult").html("");    
            });

        });

        //validate the form fields
        function IsValidModel() {
            $(".validation-msg").html('');
            var invalidValueCount = 0;

            //validation messages
            var msgRequired = "This field value is required";
            var invalidFileExtension = "Select a valid Excel file with .xlsx extension";
            var invalidNumber = "Invalid number";

            ///excel file path is required
            if (app.StringNullOrEmpty($("#fupExcel").val())) {
                invalidValueCount++;
                $("#spFupExcel").html(msgRequired);
            }

            if (selectedFile != null && selectedFile != undefined) {
                var fileExtension = selectedFile.name.substr((selectedFile.name.lastIndexOf('.') + 1));
                //console.log(fileExtension);
                if (fileExtension != "xlsx") {
                    invalidValueCount++;
                    $("#spFupExcel").html(invalidFileExtension);
                }
            }

            var startRow = $.trim($("#txtStartFromRow").val());

            if (!app.StringNullOrEmpty(startRow) && isNaN(startRow)) {
                invalidValueCount++;
                $("#spStartFromRow").html(invalidNumber);
            }

            var endRow = $.trim($("#txtEndAtRow").val());

            if (!app.StringNullOrEmpty(endRow) && isNaN(endRow)) {
                invalidValueCount++;
                $("#spEndAtRow").html(invalidNumber);
            }

            $(".validation-msg").show();
            return invalidValueCount == 0;
        }

        //clear all field values
        function ClearFieldValues() {

            //clear all textbox values
            $("input[type='text'], input[type='file']").each(function () {
                $(this).val('');
            });

            $("#ddlLanguages").prop('selectedIndex', 0);
            $("#chkUpdateFieldValues").prop("checked", "");            
        }

        ///clear result section
        function ClearResults() {
            $(".validation-msg").html("");
            $(".validation-msg").hide();                    
        }

        function ScrollToResults() {
            ///if it pageload after postback, scroll down to result section.
            var isPostbackComplete = $("#hdnPostbackComplete").val();

            if (isPostbackComplete == 1 || isPostbackComplete == "1") {
                var divResult = $(".divResult");

                if (divResult.length) {
                    $("html, body").animate({
                        scrollTop: $(divResult).offset().top
                    }, app.scrollDuration);
                }
            }
        }
         
        function Init() {
            app.HideLoadingModal();
            ClearFieldValues();
            ClearResults();
            ScrollToResults();
        }

    </script>
</asp:Content>

