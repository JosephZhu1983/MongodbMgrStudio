<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SqlManager.aspx.cs" Inherits="MongodbManagementStudio.DbAdmin.SqlManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>sql管理</title>
    <link href="../css/base.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="PageTitle">
        <img src="../images/btn_hide_sidebar.png" alt="" />&nbsp;<span>
            <asp:Literal ID="liDbName" runat="server"></asp:Literal></span>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>查询管理</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            Please enter the sql statement:</div>
                        <asp:TextBox ID="tbSql" runat="server" Height="51px" TextMode="MultiLine" Width="584px"></asp:TextBox>
                        <asp:Button ID="tbRunSql" runat="server" Text=" run " CssClass="button" OnClick="tbRunSql_Click" />
                        &nbsp;
                        <asp:Button ID="Button1" runat="server" CssClass="button" OnClick="Button1_Click"
                            Text="Show implementation plan" />
                        <br />
                        <asp:Literal ID="liMessage" runat="server"></asp:Literal>
                        <br />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div>
            <fieldset class="search">
                <legend><b>查询结果</b></legend>
                <table border="0" cellpadding="3" cellspacing="0">
                    <tr>
                        <td>
                            <div>
                                <asp:TreeView ID="dataList" runat="server">
                                </asp:TreeView>
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </div>
    </form>
</body>
</html>
