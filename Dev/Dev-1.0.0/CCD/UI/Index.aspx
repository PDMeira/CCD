<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Index.aspx.cs" Inherits="UI.Index" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="ccd.css" rel="stylesheet" type="text/css" />
    <script src="jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="jquery.blockUI.js" type="text/javascript"></script>
    <script src="ccd.js" type="text/javascript"></script>
    <style>
        div.blockMsg
        {
            width: 40%;
            top: 30%;
            left: 30%;
            text-align: center;
            background-color: #f00;
            border: 1px solid #ddd;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=50)";
            filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=50);
            -moz-opacity: .70;
            opacity: .70;
            padding: 15px;
            color: #fff;</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form2" runat="server">
    <div>
        <div class="header">
            <div>
                <div class="pane">
                    <div class="top">
                        Pre-format Rules
                    </div>
                    <div class="bottom">
                        <div class="ruleRow">
                            <asp:CheckBox ID="StripText" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="preformat">
                                RULE: Strip Text</label>
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="pane">
                    <div class="top">
                        Primary Rule
                    </div>
                    <div class="bottom">
                        <div class="ruleRow">
                            <asp:CheckBox ID="PrimaryMergeRuleWithValidation" class="inp" runat="server" Checked="True"
                                Enabled="False" />
                            <label class="lbl" for="preformat">
                                RULE: Latest Valid CCD</label>
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="pane">
                    <div class="top">
                        Merge and De-Dup Rules
                    </div>
                    <div class="bottom">
                        <div class="ruleRow">
                            <asp:CheckBox ID="AlertsSection" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="alertsSection">
                                RULE: Alerts Section</label>
                        </div>
                        <div class="ruleRow">
                            <asp:CheckBox ID="MedicationRule1" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="medicationsSection">
                                RULE: Medications Sections</label>
                        </div>
                        <div class="ruleRow">
                            <asp:CheckBox ID="TestProblemsDedup" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="problemsSection">
                                RULE: Problems Section</label>
                        </div>
                        <div class="ruleRow">
                            <asp:CheckBox ID="SocialHistory_EtohUse" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="alcoholUse">
                                RULE: Social History - Alcohol Use</label>
                        </div>
                        <div class="ruleRow">
                            <asp:CheckBox ID="SocialHistory_SmokingConsolidation" class="inp" runat="server"
                                Checked="True" />
                            <label class="lbl" for="smoking">
                                RULE: Social History - Smoking</label>
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="pane">
                    <div class="top">
                        Post-Format Rules
                    </div>
                    <div class="bottom">
                        <div class="ruleRow">
                            <asp:CheckBox ID="AddFormatedText" class="inp" runat="server" Checked="True" />
                            <label class="lbl" for="insertFormattedText">
                                RULE: Insert Formatted Text</label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:Button ID="MergeCCD" runat="server" Text="Process Rules" CssClass="mergeccd" OnClick="MergeCCD_Click" />
        <div class="contentBody">
            <div class="bodyLeft content">
                <div class="txtArea">
                    <div class="txtAreaHead">
                        CCD 1</div>
                    <div class="txtAreaBody">
                        <textarea class="actualTxtArea" id="TxtCcd1" runat="server"></textarea></div>
                </div>
                <div class="txtArea">
                    <div class="txtAreaHead">
                        CCD 2</div>
                    <div class="txtAreaBody">
                        <textarea class="actualTxtArea" id="TxtCcd2" runat="server"></textarea></div>
                </div>
                <div class="txtArea">
                    <div class="txtAreaHead">
                        CCD 3</div>
                    <div class="txtAreaBody">
                        <textarea class="actualTxtArea" id="TxtCcd3" runat="server"></textarea></div>
                </div>
            </div>
            <div class="bodyRight content">
                <div class="txtArea">
                    <div class="txtAreaHead">
                        De-Duped Final CCD</div>
                    <div class="txtAreaBody">
                        <textarea class="actualTxtArea finalTxtArea" id="TxtMasterCcd" runat="server"></textarea></div>
                </div>
            </div>
        </div>
    </div>
    </form>
</asp:Content>
