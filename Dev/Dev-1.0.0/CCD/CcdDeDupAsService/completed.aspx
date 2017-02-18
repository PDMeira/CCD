<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="completed.aspx.cs" Inherits="CcdDeDupAsService.completed" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="refresh" content="15" />
    <title></title>
    <script type="text/javascript">

        function loadXml(strGuid) {
            window.location.replace("ccd.aspx?mergeId=" + strGuid);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <section id="demo">
                <header>
                    <h3>CCD DeDuplication</h3>
                </header>
                <div id="divContent" runat="server"></div>
            </section>
        </div>
    </form>
</body>
</html>
