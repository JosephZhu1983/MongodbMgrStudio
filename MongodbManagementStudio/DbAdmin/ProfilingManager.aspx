<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="ProfilingManager.aspx.cs"
    Inherits="MongodbManagementStudio.DbAdmin.ProfilingManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/base.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.4.2.min.js" type="text/javascript"></script>
    <script src="../js/jsClass.js" type="text/javascript"></script>
    <script type="text/javascript">
        // add method
        $(document).ready(function () {
            $("#tbRunTime").attr("disabled", true);
            $("#rblType").click(function () {
                var itemValue = js.radio("rblType");
                //alert(itemValue == "1");
                if (itemValue == "1") {
                    $("#tbRunTime").attr("disabled", false);
                } else {
                    $("#tbRunTime").attr("disabled", true);
                }
            })
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="PageTitle">
        <img src="../images/btn_hide_sidebar.png" alt="" />&nbsp;<span>
            <asp:Literal ID="liDbName" runat="server"></asp:Literal></span>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>Profile管理</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            <div style="color: Red;">
                                <asp:Literal ID="LiProfileStats" runat="server"></asp:Literal></div>
                            <br />
                        </div>
                        <div>
                            设置profile:
                            <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True" Value="0">关闭Profile</asp:ListItem>
                                <asp:ListItem Value="2">打开Profile</asp:ListItem>
                                <asp:ListItem Value="1">打开Profile,并添加过滤条件</asp:ListItem>
                            </asp:RadioButtonList>
                            操作时间:<asp:TextBox ID="tbRunTime" runat="server" Width="54px">10</asp:TextBox>ms<br />
                            <br />
                            <asp:Button ID="btSetProfile"  CssClass="button" runat="server" Text="Set Profile" OnClick="btSetProfile_Click" />
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <br />
    <div>
        <fieldset class="search">
            <legend><b>列表</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            分页大小:<asp:TextBox ID="tbPageSize" runat="server" Width="51px">10</asp:TextBox>&nbsp;<asp:Button
                                ID="btRefresh" runat="server" Text="Refresh" CssClass="button" OnClick="btRefresh_Click" />
                            <asp:TreeView ID="DataTreeList" runat="server">
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
