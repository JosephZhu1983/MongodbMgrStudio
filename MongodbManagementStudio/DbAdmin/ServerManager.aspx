<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerManager.aspx.cs"
    Inherits="MongodbManagementStudio.DBAdmin.ServerManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>服务器管理</title>
    <link href="../css/base.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function delServer(ip, port) {
            if (confirm("你确定要删除吗?")) {
                MongodbManagementStudio.DBAdmin.ServerManager.DeleteServer(ip, port).value;
                //window.location.reload(); //刷新页面
                document.URL = location.href;
                //alert("删除成功!");
            }

        }
        function jsVilidate() {
            var ip = document.getElementById("tbIp").value;
            if (isIPa(ip)) {
                return true;
            } else {
                alert("IP格式不正确!");
                return false;
            }
        }
        function isIPa(strIP) {
            var re = /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/g;
            if (re.test(strIP)) {
                if (RegExp.$1 < 256 && RegExp.$2 < 256 && RegExp.$3 < 256 && RegExp.$4 < 256) return true;
            }
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="PageTitle">
        <img src="../images/btn_hide_sidebar.png" alt="" />&nbsp;<span>Servers</span>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>列表</b></legend>
            <div class="liebiao">
                <table width="100%" class="maintable">
                    <tr>
                        <th>
                            ip
                        </th>
                        <th>
                            port
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# Eval("IP")%>
                                </td>
                                <td>
                                    <%# Eval("Port")%>
                                </td>
                                <td>
                                    <span style="cursor: pointer;" onclick="delServer('<%# Eval("IP")%>','<%# Eval("Port")%>')"><img src="../images/action_delete.gif" /></span>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <asp:Literal ID="liServerInfo" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>List</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        ip:<asp:TextBox ID="tbIp" runat="server"></asp:TextBox>&nbsp;prot:<asp:TextBox ID="tbPort"
                            runat="server"></asp:TextBox>&nbsp;<asp:Button ID="Button1" CssClass="button" runat="server"
                                Text="Add Server" OnClientClick="return jsVilidate();" OnClick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    </form>
</body>
</html>
