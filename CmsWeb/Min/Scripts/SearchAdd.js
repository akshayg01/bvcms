$(function(){$("a.formlink").live("click",function(n){n.preventDefault();var t=$(this).closest("form"),i=t.serialize();return $.post($(this).attr("href"),i,function(n){if(n.close){n.message&&alert(n.message);switch(n.how){case"rebindgrids":self.parent.RebindMemberGrids&&self.parent.RebindMemberGrids($("#from").val());break;case"addselected":self.parent.AddSelected&&self.parent.AddSelected(n);break;case"addselected2":self.parent.AddSelected2&&self.parent.AddSelected2(n);break;case"CloseAddDialog":self.parent.CloseAddDialog&&self.parent.CloseAddDialog()}}else $(t).html(n).ready(function(){$("a.bt").button(),$(".addrcol").tooltip({showURL:!1,showBody:"|"}),$("#people > tbody > tr:even").addClass("alt")})}),!1}),$("a.bt").button(),$("a.clear").live("click",function(n){return n.preventDefault(),$("#name").val(""),$("#phone").val(""),$("#address").val(""),$("#dob").val(""),!1}),$("#verifyaddress").live("click",function(){var n=$(this).closest("form"),t=n.serialize();return $.post($(this).attr("href"),t,function(t){confirm(t.address+"\nUse this Address?")&&($("#address",n).val(t.Line1),$("#address2",n).val(t.Line2),$("#city",n).val(t.City),$("#state",n).val(t.State),$("#zip",n).val(t.Zip))}),!1}),$("form input").live("keypress",function(n){return n.which&&n.which==13||n.keyCode&&n.keyCode==13?($("a.default").click(),!1):!0})})