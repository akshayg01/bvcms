using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class AddressInfo
    {
        public int PeopleId
        {
            get { return _peopleId; }
            set
            {
                if (_peopleId != value)
                    person = DbUtil.Db.LoadPersonById(value);
                _peopleId = value;
            }
        }

        public CmsData.Person person { get; set; }

        public string Name { get; set; }
        public string OtherName {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "FamilyAddr";
                    case "FamilyAddr": return "PersonalAddr";
                }
                return "";
            }
        }
        public string OtherDisplay
        {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "Family";
                    case "FamilyAddr": return "Personal";
                }
                return "Address";
            }
        }
        private bool? _CanUserEditAddress;
        public bool CanUserEditAddress
        {
            get
            {
                if (!_CanUserEditAddress.HasValue)
                {
                    switch (Name)
                    {
                        case "PersonalAddr":
                            _CanUserEditAddress = person.CanUserEditBasic;
                            break;
                        case "FamilyAddr":
                            _CanUserEditAddress = person.CanUserEditFamilyAddress;
                            break;
                        default:
                            return true;
                    }
                }
                return _CanUserEditAddress.Value;
            }
        }

        public string Display
        {
            get
            {
                switch (Name)
                {
                    case "PersonalAddr": return "Personal";
                    case "FamilyAddr": return "Family";
                }
                return "Address";
            }
        }

        [DisplayName("Addr Line 1"), RemoveNA]
        public string AddressLineOne { get; set; }

        [DisplayName("Addr Line 2"), RemoveNA]
        public string AddressLineTwo { get; set; }

        [RemoveNA]
        public string CityName { get; set; }

        public CodeInfo StateCode { get; set; }

        [RemoveNA]
        public string ZipCode { get; set; }

        public CodeInfo Country { get; set; }

        public string AddrCityStateZip()
        {
            var sb = new StringBuilder();
            sb.AppendLine(AddressLineOne);
            if (AddressLineTwo.HasValue())
                sb.AppendLine(AddressLineTwo);
            sb.AppendLine(Util.FormatCSZ(CityName, StateCode.Value, ZipCode));
            return sb.ToString();
        }
        public string AddrCityStateZipLine()
        {
            var sb = new StringBuilder();
            sb.Append(AddressLineOne);
            if (AddressLineTwo.HasValue())
                sb.Append(", " + AddressLineTwo);
            if(sb.Length > 0)
                sb.Append(", " + Util.FormatCSZ5(CityName, StateCode.Value, ZipCode));
            return sb.ToString();
        }
        public string MapAddrCityStateZip()
        {
            return AddrCityStateZip().Replace("\n", ",");
        }
        public string CityStateZip()
        {
            return Util.FormatCSZ(CityName, StateCode.Value, ZipCode);
        }

        public AddressVerify.AddressResult Result { get; set; }

        [DisplayName("Bad Address Flag")]
        public bool? BadAddress { get; set; }

        [DisplayName("Resident Code")]
        public CodeInfo ResCode { get; set; }

        [DisplayName("Primar Address")]
        public bool Preferred { get; set; }

        [DisplayName("From Date")]
        public DateTime? FromDt { get; set; }

        [DisplayName("To Date")]
        public DateTime? ToDt { get; set; }

        public static IEnumerable<SelectListItem> ResCodes()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.ResidentCodesWithZero(), "Id");
        }
        public static IEnumerable<SelectListItem> States()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.GetStateList(), "Code");
        }
        public static IEnumerable<SelectListItem> Countries()
        {
            var list = CodeValueModel.ConvertToSelect(CodeValueModel.GetCountryList(), null);
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
            return list;
        }

        public AddressInfo()
        {
            Result = new AddressVerify.AddressResult();
            StateCode = new CodeInfo("", "State");
            Country = new CodeInfo("", "Country");
        }
        public AddressInfo(string address1, string address2, string city, string state, string zip, string country)
        {
            Result = new AddressVerify.AddressResult();
            AddressLineOne = address1;
            AddressLineTwo = address2;
            CityName = city;
            StateCode = new CodeInfo(state, "State");
            ZipCode = zip;
            Country = new CodeInfo(country, "Country");
        }

        public static AddressInfo GetAddressInfo(int id, string typeid)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, p);
            var a = new AddressInfo { PeopleId = id };
            switch (typeid)
            {
                case "PrimaryAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.PrimaryAddress;
                    a.AddressLineTwo = p.PrimaryAddress2;
                    a.BadAddress = p.PrimaryBadAddrFlag == 1;
                    a.CityName = p.PrimaryCity;
                    a.ZipCode = p.PrimaryZip;
                    a.StateCode = new CodeInfo(p.PrimaryState, "State");
                    a.Country = new CodeInfo(p.PrimaryCountry, "Country");
                    a.ResCode = new CodeInfo(p.PrimaryResCode, "ResCode");
                    break;
                case "FamilyAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.Family.AddressLineOne;
                    a.AddressLineTwo = p.Family.AddressLineTwo;
                    a.ToDt = p.Family.AddressToDate;
                    a.BadAddress = p.Family.BadAddressFlag;
                    a.CityName = p.Family.CityName;
                    a.ZipCode = p.Family.ZipCode;
                    a.StateCode = new CodeInfo(p.Family.StateCode, "State");
                    a.Country = new CodeInfo(p.Family.CountryName, "Country");
                    a.ResCode = new CodeInfo(p.Family.ResCodeId, "ResCode");
                    a.Preferred = p.AddressTypeId == 10;
                    break;
                case "PersonalAddr":
                    a.Name = typeid;
                    a.PeopleId = p.PeopleId;
                    a.AddressLineOne = p.AddressLineOne;
                    a.AddressLineTwo = p.AddressLineTwo;
                    a.ToDt = p.AddressToDate;
                    a.BadAddress = p.BadAddressFlag;
                    a.CityName = p.CityName;
                    a.ZipCode = p.ZipCode;
                    a.StateCode = new CodeInfo(p.StateCode, "State");
                    a.Country = new CodeInfo(p.CountryName, "Country");
                    a.ResCode = new CodeInfo(p.ResCodeId, "ResCode");
                    a.Preferred = p.AddressTypeId == 30;
                    break;
            }
            return a;
        }
        public void SetAddressInfo()
        {
            AddressLineOne = Result.Line1;
            AddressLineTwo = Result.Line2;
            CityName = Result.City;
            ZipCode = Result.Zip;
            StateCode = new CodeInfo(Result.State, "State");
        }

        public bool? Addrok { get; set; }
        public string Error { get; set; }
        public bool Saved { get; set; }
        public bool ResultChanged { get; set; }
        public bool ResultNotFound
        {
            get { return Result.found == false || Result.error.HasValue(); }
        }

        public bool IsValid
        {
            get
            {
                if (Result.found == null) // not checked, don't report as invalid yet
                    return Addrok != false;
                return Addrok == true && !ResultChanged && !ResultNotFound;
            }
        }

        public void UpdateAddress(ModelStateDictionary modelState, bool forceSave = false)
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            var f = p.Family;

            int? ResCodeId = ResCode.Value.ToInt();
            if (ResCodeId == 0)
                ResCodeId = null;

            switch (Name)
            {
                case "FamilyAddr":
                    UpdateValue(f, "AddressLineOne", AddressLineOne);
                    UpdateValue(f, "AddressLineTwo", AddressLineTwo);
                    UpdateValue(f, "AddressToDate", ToDt);
                    UpdateValue(f, "CityName", CityName);
                    UpdateValue(f, "StateCode", StateCode.Value);
                    UpdateValue(f, "ResCodeId", ResCodeId);
                    UpdateValue(f, "ZipCode", ZipCode ?? "");
                    UpdateValue(f, "CountryName", Country.Value);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 10);
                    if (fsb.Length > 0)
                        BadAddress = false;
                    UpdateValue(f, "BadAddressFlag", BadAddress);
                    break;
                case "PersonalAddr":
                    UpdateValue(p, "AddressLineOne", AddressLineOne);
                    UpdateValue(p, "AddressLineTwo", AddressLineTwo);
                    UpdateValue(p, "AddressToDate", ToDt);
                    UpdateValue(p, "CityName", CityName);
                    UpdateValue(p, "StateCode", StateCode.Value);
                    UpdateValue(p, "ResCodeId", ResCodeId);
                    UpdateValue(p, "ZipCode", ZipCode ?? "");
                    UpdateValue(p, "CountryName", Country.Value);
                    if (Preferred)
                        UpdateValue(p, "AddressTypeId", 30);
                    if (psb.Length > 0)
                        BadAddress = false;
                    UpdateValue(p, "BadAddressFlag", BadAddress);
                    break;
            }
            if (psb.Length > 0)
            {
                var c = new ChangeLog
                {
                    UserPeopleId = Util.UserPeopleId.Value,
                    PeopleId = PeopleId,
                    Field = Name,
                    Data = "<table>\n" + psb + "</table>",
                    Created = Util.Now
                };
                DbUtil.Db.ChangeLogs.InsertOnSubmit(c);
            }
            if (fsb.Length > 0)
            {
                var c = new ChangeLog
                {
                    FamilyId = p.FamilyId,
                    UserPeopleId = Util.UserPeopleId.Value,
                    PeopleId = PeopleId,
                    Field = Name,
                    Data = "<table>\n" + fsb + "</table>",
                    Created = Util.Now
                };
                DbUtil.Db.ChangeLogs.InsertOnSubmit(c);
            }
            try
            {
                DbUtil.Db.SubmitChanges();
                DbUtil.LogActivity("Update Address for: {0}".Fmt(person.Name));
            }
            catch (InvalidOperationException ex)
            {
                Error = ex.Message;
                return;
            }
            Saved = true;

            if (!HttpContext.Current.User.IsInRole("Access"))
                if (psb.Length > 0 || fsb.Length > 0)
                {
                    DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                        "Address Info Changed",
                        "{0} changed the following information:<br />\n<table>{1}{2}</table>"
                        .Fmt(Util.UserName, psb.ToString(), fsb.ToString()));
                }
        }

        public string GetNameFor<M, P>(M model, Expression<Func<M, P>> ex)
        {
            return ExpressionHelper.GetExpressionText(ex);
        }
        public bool ValidateAddress(ModelStateDictionary modelState)
        {
//            if (!Address1.HasValue())
//                modelState.AddModelError(this.GetNameFor(m => m.Address1), "Street Address Required");
//            if ((!City.HasValue() || !State.Value.HasValue()) && !Zip.HasValue())
//                modelState.AddModelError(this.GetNameFor(m => m.Zip), "Require either Zip Code or City/State");

            Addrok = modelState.IsValid;
            if (Addrok == false)
                return false;

            if ((Country.Value == "United States" || !Country.Value.HasValue()))
            {
                Result = AddressVerify.LookupAddress(AddressLineOne, AddressLineTwo, CityName, StateCode.Value, ZipCode);
                const string alertdiv = @" <div class=""alert"">{0}</div>";
                if (Result.Line1 == "error")
                    Error = alertdiv.Fmt("<h4>Network Error</h4>");
                else if (ResultNotFound)
                    Error = alertdiv.Fmt("<h4>Address Not Validated</h4><h6>{0}</h6>".Fmt(Result.error));
                else if (Result.Changed(AddressLineOne, AddressLineTwo, CityName, StateCode.Value, ZipCode))
                {
                    var msg = @"<h4>Address Found and Adjusted by USPS</h4><h6>What you entered</h6>"
                              + AddrCityStateZip().Replace("\n", "<br/>\n");
                    ResultChanged = true;
                    SetAddressInfo();
                    Error = alertdiv.Fmt(msg);
                }
            }
            return !Error.HasValue();
        }

        private StringBuilder fsb = new StringBuilder();
        private void UpdateValue(Family f, string field, object value)
        {
            var o = Util.GetProperty(f, field);
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            fsb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            Util.SetProperty(f, field, value);
        }
        private StringBuilder psb = new StringBuilder();
        private int _peopleId;

        private void UpdateValue(CmsData.Person p, string field, object value)
        {
            var o = Util.GetProperty(p, field);
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            psb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            Util.SetProperty(p, field, value);
        }

        public static IEnumerable<SelectListItem> StateCodes()
        {
            return CodeValueModel.ConvertToSelect(CodeValueModel.GetStateList(), "Code");
        }
    }
}