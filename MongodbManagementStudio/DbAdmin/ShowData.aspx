<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowData.aspx.cs" Inherits="MongodbManagementStudio.DbAdmin.ShowData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/base.css" rel="stylesheet" type="text/css" />
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
            <legend><b>信息</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            <asp:TreeView ID="DataTreeList" runat="server">
                            </asp:TreeView>
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>其它操作</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div id="divProfile" runat="server" visible="false">
                            &nbsp;
                            <a href="ProfilingManager.aspx?id=<%=infoId %>&type=<%=infoType %>">查看profile信息</a></div>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    </form>
</body>
</html>
