﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CmsWeb {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CmsWeb.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Someone recently requested a new password for this email address {email}.  
        ///However, we could not find an account associated with this email address.
        ///You may try a different email address, or contact the church.&lt;/p&gt;
        ///&lt;p&gt;If this is a mistake, please disregard this message, your password will not be changed.&lt;/p&gt;
        ///&lt;p&gt;Thanks,&lt;br /&gt;
        ///The BVCMS Team&lt;/p&gt;
        ///.
        /// </summary>
        internal static string AccountModel_ForgotPasswordBadEmail {
            get {
                return ResourceManager.GetString("AccountModel_ForgotPasswordBadEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Someone recently requested a new password for this email address {email}.
        ///To set your password, click the link below:&lt;/p&gt;
        ///&lt;blockquote&gt;&lt;a href=&quot;{resetlink}&quot;&gt;New Password&lt;/a&gt;&lt;/blockquote&gt;
        ///&lt;p&gt;If this is a mistake, please disregard this message, your password will not be changed.&lt;/p&gt;
        ///&lt;p&gt;Thanks,&lt;br /&gt;
        ///The BVCMS Team&lt;/p&gt;
        ///.
        /// </summary>
        internal static string AccountModel_ForgotPasswordReset {
            get {
                return ResourceManager.GetString("AccountModel_ForgotPasswordReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Someone recently requested a new password for {email}.
        ///To set your password, click your username below:&lt;/p&gt;
        ///&lt;blockquote&gt;{resetlink}&lt;/blockquote&gt;
        ///&lt;p&gt;If this is a mistake, please disregard this message, your password will not be changed.&lt;/p&gt;
        ///&lt;p&gt;Thanks,&lt;br /&gt;
        ///The BVCMS Team&lt;/p&gt;
        ///.
        /// </summary>
        internal static string AccountModel_ForgotPasswordReset2 {
            get {
                return ResourceManager.GetString("AccountModel_ForgotPasswordReset2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi {name},
        ///&lt;p&gt;You have a new account on our Church Management System. 
        ///Click on your username below to set your password and login to the system.&lt;/p&gt;
        ///&lt;blockquote&gt;
        ///&lt;h3&gt;Your username is: &lt;b&gt;&lt;a href=&quot;{link}&quot;&gt;{username}&lt;/a&gt;&lt;/h3&gt;
        ///&lt;/blockquote&gt;.
        /// </summary>
        internal static string AccountModel_NewUserWelcome {
            get {
                return ResourceManager.GetString("AccountModel_NewUserWelcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;
        ///&lt;TestPlan&gt;
        ///  &lt;Sections&gt;
        ///    &lt;Section name=&quot;Meta&quot;&gt;
        ///      &lt;Tests&gt;
        ///        &lt;test name=&quot;Lookup&quot;&gt;
        ///          &lt;args&gt;
        ///            &lt;name&gt;table&lt;/name&gt;
        ///          &lt;/args&gt;
        ///          &lt;script&gt;
        ///            &lt;![CDATA[
        ///xml = webclient.DownloadString(&apos;APIMeta/lookups/&apos; + table)
        ///return xml
        ///]]&gt;
        ///          &lt;/script&gt;
        ///          &lt;description&gt;
        ///            &lt;![CDATA[
        ///&lt;ul&gt;
        ///&lt;li&gt;These are tables of id / value pairs for look up tables&lt;/li&gt;
        ///&lt;li&gt;Try MemberStatus as a name of a table&lt;/ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string APITestPlan {
            get {
                return ResourceManager.GetString("APITestPlan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h1&gt;Sample Church&lt;/h1&gt;
        ///&lt;h2&gt;2000 Appling Rd. | Cordova | TN 38088-1210 | (901) 347-2000&lt;/h2&gt;.
        /// </summary>
        internal static string ContributionStatementHeader {
            get {
                return ResourceManager.GetString("ContributionStatementHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;&lt;i&gt;
        ///NOTE: No goods or services were provided to you by the church in connection with any contibution;
        ///any value received consisted entirely of intangible religious benefits.
        ///Sample Church, FEIN # 1234, is a 501(c)(3) organization and
        ///qualifies as a part of the Southern Baptist Convention&apos;s group tax exemption ruling number GEN #1674.
        ///&lt;/i&gt;&lt;/p&gt;
        ///&lt;p&gt;&lt;i&gt;
        ///Thank you for your faithfulness in the giving of your time, talents, and resources. Together we can share the love of Jesus with our city.
        ///&lt;/i&gt;&lt;/p&gt;.
        /// </summary>
        internal static string ContributionStatementNotice {
            get {
                return ResourceManager.GetString("ContributionStatementNotice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Hi {first},&lt;/p&gt;
        ///
        ///&lt;p&gt;I need a substitute for {org}&lt;br&gt;
        ///
        ///on {meetingdate} at {meetingtime}&lt;/p&gt;
        ///
        ///&lt;p&gt;DO NOT REPLY. Instead, click one of the links below.&lt;/p&gt;
        ///&lt;blockquote&gt;
        ///
        ///&lt;p&gt;{yeslink}&lt;/p&gt;
        ///
        ///&lt;p&gt;{nolink}&lt;/p&gt;
        ///
        ///&lt;/blockquote&gt;
        ///
        ///&lt;p&gt;
        ///Thank you for your consideration,&lt;br /&gt;
        ///
        ///{sendername}
        ///&lt;/p&gt;.
        /// </summary>
        internal static string VolSubModel_ComposeMessage_Body {
            get {
                return ResourceManager.GetString("VolSubModel_ComposeMessage_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///&lt;p&gt;Hi {first},&lt;/p&gt;
        ///&lt;p&gt;We need additional volunteers for {org}&lt;br&gt;
        ///on {meetingdate} at {meetingtime}&lt;/p&gt;
        ///
        ///&lt;p&gt;DO NOT REPLY. Instead, click one of the links below.&lt;/p&gt;
        ///&lt;blockquote&gt;
        ///&lt;p&gt;{yeslink}&lt;/p&gt;
        ///&lt;p&gt;{nolink}&lt;/p&gt;
        ///&lt;/blockquote&gt;
        ///&lt;p&gt;
        ///Thank you for your consideration,&lt;br /&gt;
        ///{sendername}
        ///&lt;/p&gt;.
        /// </summary>
        internal static string VolunteerRequestModel_ComposeMessage_Body {
            get {
                return ResourceManager.GetString("VolunteerRequestModel_ComposeMessage_Body", resourceCulture);
            }
        }
    }
}