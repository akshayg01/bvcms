$(function(){$(".datepicker").datepicker({dateFormat:"m/d/yy",changeMonth:!0,changeYear:!0,onSelect:$.reload}),$("#Dt1").change(function(){$("#Dt2").val(""),$.reload()}),$.reload=function(){var n=$("form").serialize();window.location="/Reports/Meetings?"+n},$("#Inactive").change($.reload),$("#NoZero").change($.reload),$("table.grid tbody tr:even").addClass("alt"),$("a.sortable").click(function(n){var t,r,i;n.preventDefault(),t=$(this).text(),r=$("#Sort").val(),$("#Sort").val(t),i=$("#Dir").val(),r==t&&i=="asc"?$("#Dir").val("desc"):$("#Dir").val("asc"),$.reload()})})