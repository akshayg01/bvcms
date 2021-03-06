﻿<%@ Page Language="C#" AutoEventWireup="True" %>

<!DOCTYPE html>
<html>
<head>
    <!-- meta -->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <meta name="language" content="en-US">
    <meta name="viewport" content="maximum-scale=1.0,width=device-width,initial-scale=1.0">
    <!-- title -->
    <title>BVCMS error</title>
    <!-- css -->
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
        }

        body {
            background: #F0F0F0;
            font: 14px/18px Arial, "Helvetica Neue", sans-serif;
            text-align: center;
        }

        #surface {
            margin: 40px auto;
            position: relative;
            width: 480px;
        }

        h1 {
            color: #222;
            font-size: 22px;
            font-weight: bold;
            line-height: 26px;
            margin-bottom: 10px;
        }

        p {
            margin-bottom: 10px;
        }

        @media all and (max-width: 480px) {
            #surface {
                margin: 40px 10px;
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <div id="surface">
        <h1>Database not found: <%=Request.QueryString["dbname"] %></h1>
        <p><a href="http://www.bvcms.com">BVCMS homepage</a></p>
    </div>
</body>
</html>

