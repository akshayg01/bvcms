<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/onlinereg.Master" Inherits="System.Web.Mvc.ViewPage<CmsWeb.Models.PaymentModel>" %>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
div.terms {
   width:600px;
   height:200px;
   border:1px solid #ccc;
   background:#f2f2f2;
   padding:6px;
   overflow:auto;
}
div.terms p,
div.terms li {font:normal 11px/15px arial;color:#333;}
div.terms h3 {font:bold 14px/19px arial;color:#000;}
div.terms h4 {font:bold 12px/17px arial;color:#000;}
div.terms strong {color:#000;}	
a.submitbutton,a.button {
  padding:5px;
    border-color:#D9DFEA #0E1F5B #0E1F5B #D9DFEA;
    background-color:#3B5998;
  border: 1px solid;
    color:#FFFFFF;
  text-decoration:none;
}
</style>
    <script src="/Content/js/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="/Content/js/jquery.idle-timer.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function() {
            $(document).bind("idle.idleTimer", function() {
                window.location.href = '<%=Model._URL %>';
            });
            var tmout = parseInt('<%=Model._timeout %>');

            $("a.submitbutton").click(function(ev) {
                ev.preventDefault();
                var f = $(this).closest('form');
                var q = f.serialize();
                $.post(this.href, q, function(ret) {
                    if (ret.error) {
                        $('#validatecoupon').text(ret.error);
                    }
                    else {
                        window.location = ret.confirm;
                    }
                }, "json");
                return false;
            });
            $.idleTimer(tmout);
        });
    </script>
    <script type="text/javascript">
        $(function() {
            if ($('#IAgree').attr("id")) {
                $("#Submit").attr("disabled", "disabled");
                $("a.submitbutton").attr("disabled", "disabled");
            }
            $("#IAgree").click(function() {
                var checked_status = this.checked;
                if (checked_status == true) {
                    $("#Submit").removeAttr("disabled");
                    $("a.submitbutton").removeAttr("disabled");
                }
                else {
                    $("#Submit").attr("disabled", "disabled");
                    $("a.submitbutton").attr("disabled", "disabled");
                }
            });
        });
    </script>

    <h2>Terms of Agreement</h2>
<%=Model.Terms %>
    <form action="<%=Model.PostbackURL %>" method="post">
    <%=Html.Hidden("TransactionID", "zero due") %>
<p><%=Html.CheckBox("IAgree") %> I agree to the above terms and conditions.</p>
    <p>
        You must agree to the terms above for you or your minor child before you can continue with confirmation.</p>
    <p>
    <p><input type="submit" name="Submit" value="Complete Registration" /></p>
    </form>
</asp:Content>