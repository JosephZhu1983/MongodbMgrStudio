<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReplactionInfo.aspx.cs"
    Inherits="MongodbManagementStudio.DbAdmin.ReplactionInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>同步信息查看</title>
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
            <legend><b>服务器类型</b></legend>
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
            <legend><b>同步信息</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            <asp:TreeView ID="DataTreeList2" runat="server">
                            </asp:TreeView>
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    </form>
</body>
</html>
