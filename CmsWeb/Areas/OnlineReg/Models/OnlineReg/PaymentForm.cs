using System;
using System.Linq;
using System.Text;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PaymentForm
    {
        public decimal? AmtToPay { get; set; }
        public decimal? Donate { get; set; }
        public decimal Amtdue { get; set; }
        public string Coupon { get; set; }
        public string CreditCard { get; set; }
        public string Expires { get; set; }
        public string MaskedCCV { get; set; }
        public string CCV { get; set; }
        public string Routing { get; set; }
        public string Account { get; set; }
        public string Type { get; set; }
        public bool AskDonation { get; set; }
        public bool AllowCoupon { get; set; }
        public string Url { get; set; }
        public int timeout { get; set; }
        public string Terms { get; set; }
        public int DatumId { get; set; }
        public Guid FormId { get; set; }
        //public string Name { get; set; }
        public string FullName()
        {
            string n;
            if (MiddleInitial.HasValue())
                n = "{0} {1} {2}".Fmt(First, MiddleInitial, Last);
            else
                n = "{0} {1}".Fmt(First, Last);
            if (Suffix.HasValue())
                n = n + " " + Suffix;
            return n;
        }

        public string First { get; set; }
        public string MiddleInitial { get; set; }
        public string Last { get; set; }
        public string Suffix { get; set; }
        public string Description { get; set; }
        public bool PayBalance { get; set; }
        public int? OrgId { get; set; }
        public int? OriginalId { get; set; }
        public bool testing { get; set; }
        public bool? FinanceOnly { get; set; }
        public bool? IsLoggedIn { get; set; }
        public bool? CanSave { get; set; }
        public bool? SavePayInfo { get; set; }
        public bool? IsGiving { get; set; }
        public bool NoCreditCardsAllowed { get; set; }
        private bool? _noEChecksAllowed;
        public bool NoEChecksAllowed
        {
            get
            {
                if (!_noEChecksAllowed.HasValue)
                    _noEChecksAllowed = DbUtil.Db.Setting("NoEChecksAllowed", "false") == "true"
                            || OnlineRegModel.GetTransactionGateway() != "sage";
                return _noEChecksAllowed.Value;
            }
        }


        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int? TranId { get; set; }

        public Transaction CreateTransaction(CMSDataContext Db, decimal? amount = null)
        {
            if (!amount.HasValue)
                amount = AmtToPay;
            decimal? amtdue = null;
            if (Amtdue > 0)
                amtdue = Amtdue - (amount ?? 0);
            var ti = new Transaction
                     {
                         First = First,
                         MiddleInitial = MiddleInitial.Truncate(1) ?? "",
                         Last = Last,
                         Suffix = Suffix,
                         Donate = Donate,
                         Regfees = AmtToPay,
                         Amt = amount,
                         Amtdue = amtdue,
                         Emails = Email,
                         Testing = testing,
                         Description = Description,
                         OrgId = OrgId,
                         Url = Url,
                         TransactionGateway = OnlineRegModel.GetTransactionGateway(),
                         Address = Address.Truncate(50),
                         City = City,
                         State = State,
                         Zip = Zip,
                         DatumId = DatumId,
                         Phone = Phone.Truncate(20),
                         OriginalId = OriginalId,
                         Financeonly = FinanceOnly,
                         TransactionDate = DateTime.Now
                     };
            Db.Transactions.InsertOnSubmit(ti);
            Db.SubmitChanges();
            if (OriginalId == null) // first transaction
                ti.OriginalId = ti.Id;
            return ti;
        }
        public static PaymentForm CreatePaymentFormForBalanceDue(Transaction ti)
        {
            PaymentInfo pi = null;
            if (ti.Person != null && string.Equals(OnlineRegModel.GetTransactionGateway(),
                        "Sage", StringComparison.InvariantCultureIgnoreCase))
                pi = ti.Person.PaymentInfos.FirstOrDefault();
            if (pi == null)
                pi = new PaymentInfo();
            var pf = new PaymentForm
                     {
                         PayBalance = true,
                         AmtToPay = ti.Amtdue ?? 0,
                         Amtdue = ti.Amtdue ?? 0,
                         AllowCoupon = true,
                         AskDonation = false,
                         Description = ti.Description,
                         OrgId = ti.OrgId,
                         OriginalId = ti.OriginalId,
                         Email = Util.FirstAddress(ti.Emails).Address,

                         First = ti.First,
                         MiddleInitial = ti.MiddleInitial.Truncate(1) ?? "",
                         Last = ti.Last,
                         Suffix = ti.Suffix,

                         Phone = ti.Phone,
                         Address = ti.Address,
                         City = ti.City,
                         State = ti.State,
                         Zip = ti.Zip,
                         timeout = 6000000,
                         testing = ti.Testing ?? false,
                         TranId = ti.Id,
#if DEBUG2
						 CreditCard = "4111111111111111",
						 CCV = "123",
						 Expires = "1015",
						 Routing = "056008849",
						 Account = "12345678901234"
#else
                         CreditCard = pi.MaskedCard,
                         MaskedCCV = Util.Mask(new StringBuilder(pi.Ccv), 0),
                         CCV = pi.Ccv,
                         Expires = pi.Expires,
                         Account = pi.MaskedAccount,
                         Routing = pi.Routing,
                         SavePayInfo =
                            (pi.MaskedAccount != null && pi.MaskedAccount.StartsWith("X"))
                            || (pi.MaskedCard != null && pi.MaskedCard.StartsWith("X")),
#endif
                     };
            pf.Type = pf.NoEChecksAllowed ? "C" : "";
            return pf;
        }
        public static PaymentForm CreatePaymentForm(OnlineRegModel m)
        {
            var r = m.GetTransactionInfo();
            if (r == null)
                return null;

            var pf = new PaymentForm
            {
                AmtToPay = m.PayAmount() + (m.donation ?? 0),
                AskDonation = m.AskDonation(),
                AllowCoupon = !m.OnlineGiving(),
                PayBalance = false,
                Amtdue = m.TotalAmount() + (m.donation ?? 0),
                Donate = m.donation,
                Description = m.DescriptionForPayment,
                Email = r.Email,
                First = r.First,
                MiddleInitial = r.Middle,
                Last = r.Last,
                Suffix = r.Suffix,
                IsLoggedIn = m.UserPeopleId.HasValue,
                OrgId = m.List[0].orgid,
                Url = m.URL,
                testing = m.testing ?? false,
                Terms = m.Terms,
                Address = r.Address,
                City = r.City,
                State = r.State,
                Zip = r.Zip,
                Phone = r.Phone,
#if DEBUG2
				 CreditCard = "4111111111111111",
				 CCV = "123",
				 Expires = "1015",
				 Routing = "056008849",
				 Account = "12345678901234"
#else
                CreditCard = r.payinfo.MaskedCard,
                Account = r.payinfo.MaskedAccount,
                Routing = r.payinfo.Routing,
                Expires = r.payinfo.Expires,
                MaskedCCV = Util.Mask(new StringBuilder(r.payinfo.Ccv), 0),
                CCV = r.payinfo.Ccv,
                SavePayInfo =
                   (r.payinfo.MaskedAccount != null && r.payinfo.MaskedAccount.StartsWith("X"))
                   || (r.payinfo.MaskedCard != null && r.payinfo.MaskedCard.StartsWith("X")),
                Type = r.payinfo.PreferredPaymentType,
#endif
            };
            if (m.OnlineGiving())
            {
                pf.NoCreditCardsAllowed = DbUtil.Db.Setting("NoCreditCardGiving", "false").ToBool();
                pf.IsGiving = true;
                pf.FinanceOnly = true;
                pf.Type = r.payinfo.PreferredGivingType;
                if (pf.NoCreditCardsAllowed)
                    pf.Type = "B"; // bank account only
                else if (pf.NoEChecksAllowed)
                    pf.Type = "C"; // credit card only
            }
            else if (m.ManageGiving() || m.OnlinePledge())
            {
                pf.FinanceOnly = true;
            }
            pf.Type = pf.NoEChecksAllowed ? "C" : pf.Type;
            return pf;
        }
        public static Transaction CreateTransaction(CMSDataContext Db, Transaction t, decimal? amount)
        {
            var amtdue = t.Amtdue != null ? t.Amtdue - (amount ?? 0) : null;
            var ti = new Transaction
                     {
                         Name = t.Name,
                         First = t.First,
                         MiddleInitial = t.MiddleInitial,
                         Last = t.Last,
                         Suffix = t.Suffix,
                         Donate = t.Donate,
                         Amtdue = amtdue,
                         Amt = amount,
                         Emails = Util.FirstAddress(t.Emails).Address,
                         Testing = t.Testing,
                         Description = t.Description,
                         OrgId = t.OrgId,
                         Url = t.Url,
                         Address = t.Address,
                         TransactionGateway = OnlineRegModel.GetTransactionGateway(),
                         City = t.City,
                         State = t.State,
                         Zip = t.Zip,
                         DatumId = t.DatumId,
                         Phone = t.Phone,
                         OriginalId = t.OriginalId ?? t.Id,
                         Financeonly = t.Financeonly,
                         TransactionDate = DateTime.Now,
                     };
            Db.Transactions.InsertOnSubmit(ti);
            Db.SubmitChanges();
            return ti;
        }
        public object Autocomplete(bool small = false)
        {
            string auto;
#if DEBUG
            auto = "on";
#else
			auto = "off";
#endif
            if (small)
                return new
                {
                    AUTOCOMPLETE = auto,
                    @class = "short"
                };
            return new
            {
                AUTOCOMPLETE = auto,
            };
        }
    }
}

