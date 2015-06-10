<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexManager.aspx.cs" Inherits="MongodbManagementStudio.DbAdmin.IndexManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css/base.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.4.2.min.js" type="text/javascript"></script>
    <title></title>
    <style type="text/css">
        #Select1
        {
            width: 76px;
        }
    </style>
    <script type="text/javascript">

        // add method
        $(document).ready(function () {
            $("#btAddColumn").click(function () {
                var $table = $("#tab tr");
                var title = $("#inputEnterColumn").attr("value");
                if (title != "") {
                    var item = $("#selectType option:selected").text(); //获取选择下拉框的值的名称
                    var itemValue = $("#selectType").val(); //获取选择下拉框的值
                    // $("#hdIndexInfo").appendTo("{" + table + ":" + itemValue + "}");

                    var newId = title + ":" + itemValue + "|";
                    var hfValue = $("#hdIndexInfo").val();
                    if (hfValue.indexOf(title) == -1) {
                        var newvalue = hfValue + title + ":" + itemValue + "|";
                        $("#hdIndexInfo").attr("value", newvalue);
                        $("#tab").append("<tr id=" + newId + "><td align=\'center\'>" + title + "</td><td align=\"center\">" + item + "</td><td align=\'center\'><a href=\'#\' onclick=\"deltr('" + newId + "')\">删除</a></td></tr>");
                    } else {
                        alert("Can not add duplicate column names!");
                    }
                } else {
                    alert("Please enter the index listing!");
                }
            })
        })

        // delete method
        function deltr(index) {

            var hfValue = $("#hdIndexInfo").val();
            var newValue = hfValue.replace(index, "");
            $("#hdIndexInfo").attr("value", newValue);
            //alert("tr[id=\'"+index+"\']");
            //alert(newValue);
            $("tr[id=\'" + index + "\']").remove();

        }
        function setColumnName(obj) {
            $("#inputEnterColumn").attr("value", obj);
        }

        function deleteIndex(obj) {
            if (confirm("你确定要删除吗?")) {
                var jsId = "<%=infoId %>";
                MongodbManagementStudio.DbAdmin.IndexManager.DelIndex(obj, jsId).value;
                document.URL = location.href; //刷新页面
                //alert("删除成功!");
            }

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="PageTitle">
        <img src="../images/btn_hide_sidebar.png" alt="" />&nbsp;<span><asp:Literal ID="liDbName"
            runat="server"></asp:Literal></span>
    </div>
    <div>
        <fieldset class="search">
            <legend><b>索引管理</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <div>
                            <br />
                            <b>列名:</b><br />
                            <div class="liebiao">
                                <table class="maintable">
                                    <tr>
                                        <asp:Literal ID="liColumnName" runat="server"></asp:Literal>
                                        </th>
                                    </tr>
                                </table>
                                <br />
                                创建索引:<br />
                                &nbsp;&nbsp;
                                <br />
                                索引键列:<input id="inputEnterColumn" type="text" /><select id="selectType" name="selectType">
                                    <option value="1" selected>Ascending</option>
                                    <option value="-1">Descending</option>
                                </select><input id="btAddColumn" type="button" class="button" value="Add Column" /><br />
                                列名 排序类型<br />
                                <br />
                                <table id="tab" class="maintable">
                                    <tr>
                                        <th align="center">
                                            &nbsp;列名&nbsp;
                                        </th>
                                        <th align="center">
                                            &nbsp;排序&nbsp;
                                        </th>
                                        <th align="center">
                                            &nbsp;操作&nbsp;
                                        </th>
                                    </tr>
                                </table>
                                <input id="hdIndexInfo" name="hdIndexInfo" type="hidden" />
                                <input id="cbUnique" name="cbUnique" value="true" type="checkbox" /><span>unique</span>
                                <input id="cbBackground" name="cbBackground" value="true" type="checkbox" /><span>background</span>
                                <input id="cbDropDups" name="cbDropDups" value="true" type="checkbox" /><span>dropDups</span>
                                <asp:Button ID="btCreateIndex" runat="server" Text="Create index" CssClass="button"
                                    OnClick="btCreateIndex_Click" /><asp:Literal ID="liMessage" runat="server"></asp:Literal>
                                <br />
                            </div>
    </td> </tr> </table> </fieldset> </div>
    <div>
        <fieldset class="search">
            <legend><b>查看索引</b></legend>
            <table border="0" cellpadding="3" cellspacing="0">
                <tr>
                    <td>
                        <asp:TreeView ID="dataList" runat="server">
                        </asp:TreeView>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    </form>
</body>
</html>
