using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MoreLinq;
using NPOI.POIFS.Properties;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class QueryModel : QueryResults
    {
        public Guid? SelectedId { get; set; }

        #region Visibility

        public bool RightPanelVisible { get; set; }
        public bool ComparePanelVisible { get; set; }
        public bool TextVisible { get; set; }
        public bool NumberVisible { get; set; }
        public bool IntegerVisible { get; set; }
        public bool CodeVisible { get; set; }
        public bool DateVisible { get; set; }
        public bool ProgramVisible { get; set; }
        public bool DivisionVisible { get; set; }
        public bool EndDateVisible { get; set; }
        public bool StartDateVisible { get; set; }
        public bool OrganizationVisible { get; set; }
        public bool ScheduleVisible { get; set; }
        public bool CampusVisible { get; set; }
        public bool OrgTypeVisible { get; set; }
        public bool DaysVisible { get; set; }
        public bool AgeVisible { get; set; }
        public bool SavedQueryVisible { get; set; }
        public bool MinistryVisible { get; set; }
        public bool QuartersVisible { get; set; }
        public bool TagsVisible { get; set; }
        public bool PmmLabelsVisible { get; set; }

        #endregion

        public int? Program { get; set; }
        public int? Division { get; set; }
        public int? Organization { get; set; }
        public int? Schedule { get; set; }
        public int? Campus { get; set; }
        public int? OrgType { get; set; }
        public int? Ministry { get; set; }
        public bool IsPublic { get; set; }
        public string Days { get; set; }
        public string Age { get; set; }
        public string Quarters { get; set; }
        public string QuartersLabel { get; set; }
        public string View { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Comparison { get; set; }
        public string[] Tags { get; set; }
        public string[] PmmLabels { get; set; }

        public string CodeValue { get; set; }
        public string[] CodeValues { get; set; }

        public string TextValue { get; set; }
        public string DateValue { get; set; }
        public string NumberValue { get; set; }
        public string IntegerValue { get; set; }

        public bool SelectMultiple { get; set; }

        public QueryModel()
        {
            Db.SetUserPreference("NewCategories", "true");
            ConditionName = "Group";
        }

        public List<SelectListItem> TagData { get; set; }
        public List<SelectListItem> PmmLabelData { get; set; }

        private static List<CodeValueItem> BitCodes =
            new List<CodeValueItem> 
            { 
                new CodeValueItem { Id = 1, Value = "True", Code = "T" }, 
                new CodeValueItem { Id = 0, Value = "False", Code = "F" }, 
            };
        public IEnumerable<SelectListItem> GetCodeData()
        {
            var cvctl = new CodeValueModel();
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    return ConvertToSelect(BitCodes, fieldMap.DataValueField);
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    if (fieldMap.DataSource == "ExtraValues")
                        return StandardExtraValues.ExtraValueCodes();
                    if (fieldMap.DataSource == "Campuses")
                        return Campuses();
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                case FieldType.DateField:
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
            }
            return null;
        }

        private FieldClass fieldMap;
        private string _ConditionName;
        public string ConditionName
        {
            get { return _ConditionName; }
            set
            {
                _ConditionName = value;
                fieldMap = FieldClass.Fields[value];
            }
        }
        public string ConditionText { get { return fieldMap.Title; } }

        public void SetVisibility()
        {
            ComparePanelVisible = fieldMap.Name != "MatchAnything";
            RightPanelVisible = ComparePanelVisible;
            ConditionName = ConditionName;
            DivisionVisible = fieldMap.HasParam("Division");
            ProgramVisible = fieldMap.HasParam("Program");
            OrganizationVisible = fieldMap.HasParam("Organization");
            ScheduleVisible = fieldMap.HasParam("Schedule");
            CampusVisible = fieldMap.HasParam("Campus");
            OrgTypeVisible = fieldMap.HasParam("OrgType");
            DaysVisible = fieldMap.HasParam("Days");
            AgeVisible = fieldMap.HasParam("Age");
            SavedQueryVisible = fieldMap.HasParam("SavedQueryIdDesc");
            MinistryVisible = fieldMap.HasParam("Ministry");
            QuartersVisible = fieldMap.HasParam("Quarters");
            if (QuartersVisible)
                QuartersLabel = fieldMap.QuartersTitle;
            PmmLabelsVisible = fieldMap.HasParam("PmmLabels");
            TagsVisible = fieldMap.HasParam("Tags");
            if (TagsVisible)
            {
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            StartDateVisible = fieldMap.HasParam("StartDate");
            EndDateVisible = fieldMap.HasParam("EndDate");

            TextVisible = NumberVisible = CodeVisible = DateVisible = false;
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                    CodeVisible = true;
                    break;
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    TextVisible = true;
                    break;
                case FieldType.NullNumber:
                case FieldType.Number:
                    NumberVisible = true;
                    break;
                case FieldType.NullInteger:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                    IntegerVisible = true;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    DateVisible = true;
                    break;
            }
        }
        public List<SelectListItem> ConvertToSelect(object items, string valuefield)
        {
            var list = items as IEnumerable<CodeValueItem>;
            List<SelectListItem> list2;
            List<string> values;
            if (CodeValues != null)
                values = CodeValues.ToList();
            else if (CodeValue != null)
                values = new List<string> { CodeValue };
            else
                values = new List<string>();
            switch (valuefield)
            {
                case "IdCode":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode, Selected = values.Contains(c.IdCode) }).ToList();
                    break;
                case "Id":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Id.ToString(), Selected = values.Contains(c.Id.ToString()) }).ToList();
                    break;
                case "Code":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Code, Selected = values.Contains(c.Code) }).ToList();
                    break;
                default:
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Value, Selected = values.Contains(c.Value) }).ToList();
                    break;
            }
            return list2;
        }
        DateTime? DateParse(string s)
        {
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return dt;
            return null;
        }
        string DateString(DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToShortDateString();
            return "";
        }
        public void UpdateCondition()
        {
            var c = Current;
            c.Field = ConditionName;
            c.Comparison = Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    c.TextValue = TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    c.TextValue = IntegerValue;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                    c.TextValue = NumberValue;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    c.DateValue = DateParse(DateValue);
                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    if (c.HasMultipleCodes && CodeValues != null)
                        c.CodeIdValue = string.Join(";", CodeValues);
                    else
                        c.CodeIdValue = CodeValue;
                    break;
            }
            c.Program = Program ?? 0;
            c.Division = Division ?? 0;
            c.Organization = Organization ?? 0;
            if (MinistryVisible)
                c.Program = Ministry ?? 0;
            c.Schedule = Schedule ?? 0;
            c.Campus = Campus ?? 0;
            c.OrgType = OrgType ?? 0;
            c.StartDate = DateParse(StartDate);
            c.EndDate = DateParse(EndDate);
            c.Days = Days.ToInt();
            c.Age = Age.ToInt();
            c.Quarters = Quarters;
            if (Tags != null)
                c.Tags = string.Join(";", Tags);
            else if (PmmLabels != null)
                c.Tags = string.Join(",", PmmLabels);
            TopClause.Save(Db, increment: true);
        }
        public void EditCondition()
        {
            var c = Current;
            SelectedId = c.Id;
            ConditionName = c.FieldInfo.Name;
            SetVisibility();
            Comparison = c.Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    TextValue = c.TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    IntegerValue = c.TextValue;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                    NumberValue = c.TextValue;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    DateValue = DateString(c.DateValue);
                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeValue = c.CodeIdValue;
                    if (c.HasMultipleCodes && CodeValue.HasValue())
                    {
                        CodeValues = c.CodeIdValue.Split(';');
                        foreach (var i in GetCodeData())
                            i.Selected = CodeValues.Contains(i.Value);
                    }
                    break;
            }
            Program = c.Program;
            Division = c.Division;
            Organization = c.Organization;
            Schedule = c.Schedule;
            Campus = c.Campus;
            OrgType = c.OrgType;
            StartDate = DateString(c.StartDate);
            EndDate = DateString(c.EndDate);
            SelectMultiple = c.HasMultipleCodes;
            Days = c.Days.ToString();
            Age = c.Age.ToString();
            Quarters = c.Quarters;
            if (TagsVisible)
            {
                if (c.Tags != null)
                    Tags = c.Tags.Split(';');
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
                foreach (var i in TagData)
                    i.Selected = Tags.Contains(i.Value);
            }
            if (PmmLabelsVisible)
            {
                if (c.Tags != null)
                    PmmLabels = c.Tags.Split(',').Select(vv => vv).ToArray();
                var cv = new CodeValueModel();
                PmmLabelData = CodeValueModel.ConvertToSelect(cv.PmmLabels(), "Id");
                if (PmmLabels != null)
                    foreach (var i in PmmLabelData)
                        i.Selected = PmmLabels.Contains(i.Value);
            }
            if (MinistryVisible)
                Ministry = c.Program;
        }
        public void SetCodes()
        {
            SetVisibility();
            SelectMultiple = Comparison.EndsWith("OneOf");
        }
        [Serializable]
        public class ClipboardItem
        {
            public string from { get; set; }
            public Guid guid { get; set; }
            public string xml { get; set; }

            public ClipboardItem(string @from, Guid guid, string xml)
            {
                this.@from = @from;
                this.guid = guid;
                this.xml = xml;
            }
        }

        public void Paste(Guid id)
        {
            var clip = HttpContext.Current.Session["QueryClipboard"] as ClipboardItem;
            if (clip == null)
                return;
            var newclause = Condition.Import(clip.xml, newGuids: clip.from == "copy");
            Condition prevParent = null;
            if (clip.from == "cut")
                if (clip.guid != TopClause.Id)
                {
                    var originalquery = Db.LoadQueryById2(clip.guid).ToClause();
                    var origclause = originalquery.AllConditions[newclause.Id];
                    originalquery.AllConditions.Remove(newclause.Id);
                    if (!origclause.Parent.Conditions.Any())
                        originalquery.AllConditions.Remove(origclause.Parent.Id);
                    originalquery.Save(Db);
                }
                else
                    prevParent = TopClause.AllConditions[newclause.Id].Parent;

            newclause.AllConditions = Current.AllConditions;
            Current.AllConditions[newclause.Id] = newclause;


            if (Current.IsGroup)
            {
                newclause.Order = Current.MaxClauseOrder() + 2;
                newclause.ParentId = Current.Id;
            }
            else
            {
                newclause.Order = Current.Order + 1;
                newclause.ParentId = Current.Parent.Id;
                Current.Parent.ReorderClauses();
            }
            if (prevParent != null && !prevParent.Conditions.Any())
                TopClause.AllConditions.Remove(prevParent.Id);
            TopClause.Save(Db, increment: true);
        }
        public Guid AddConditionToGroup()
        {
            var nc = Current.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Guid AddGroupToGroup()
        {
            var g = Current.AddNewGroupClause();
            var nc = g.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Condition Current
        {
            get
            {
                var gid = SelectedId ?? Guid.Empty;
                return TopClause.AllConditions[gid];
            }
        }
        public void DeleteCondition()
        {
            Current.DeleteClause();
            TopClause.Save(Db, increment: true);
        }
        public void InsertGroupAbove()
        {
            var g = new Condition
            {
                Id = Guid.NewGuid(),
                Field = QueryType.Group.ToString(),
                Comparison = CompareType.AnyTrue.ToString(),
                AllConditions = Current.AllConditions
            };
            if (Current.IsFirst)
            {
                Current.ParentId = g.Id;
                g.ParentId = null;
            }
            else
            {
                var list = Current.Parent.Conditions.Where(cc => cc.Order >= Current.Order).ToList();
                g.ParentId = Current.ParentId;
                foreach (var c in list)
                    c.ParentId = g.Id;
                g.Order = Current.MaxClauseOrder();
            }
            Current.AllConditions.Add(g.Id, g);
            if (g.IsFirst)
            {
                TopClause = g;
                SelectedId = g.Id;
            }
            TopClause.Save(Db, increment: true);
        }
        public IEnumerable<SelectListItem> GroupComparisons()
        {
            return from c in CompareClass2.Comparisons
                   where c.FieldType == FieldType.Group
                   select new SelectListItem
                   {
                       Text = c.CompType == CompareType.AllTrue ? "All"
                           : c.CompType == CompareType.AnyTrue ? "Any"
                               : c.CompType == CompareType.AllFalse ? "None"
                                   : "unknown",
                       Value = c.CompType.ToString()
                   };
        }
        public IEnumerable<SelectListItem> Comparisons()
        {
            return from c in CompareClass2.Comparisons
                   where c.FieldType == fieldMap.Type
                   select new SelectListItem { Text = c.CompType.ToString(), Value = c.CompType.ToString() };
        }
        public IEnumerable<SelectListItem> Schedules()
        {
            var q = from o in Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where sc != null
                    group o by new { ScheduleId = sc.ScheduleId ?? 10800, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.ToString(),
                        Text = Db.GetScheduleDesc(g.Key.MeetingTime)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Campuses()
        {
            var q = from o in Db.Organizations
                    where o.CampusId != null
                    group o by o.CampusId into g
                    orderby g.Key
                    select new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().Campu.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> OrgTypes()
        {
            var q = from t in Db.OrganizationTypes
                    orderby t.Code
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Programs()
        {
            var q = from t in Db.Programs
                    orderby t.Name
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public static IEnumerable<SelectListItem> Divisions(int? progid)
        {
            var q = from div in DbUtil.Db.Divisions
                    where div.ProgDivs.Any(d => d.ProgId == progid)
                    orderby div.Name
                    select new SelectListItem
                    {
                        Value = div.Id.ToString(),
                        Text = div.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public static IEnumerable<SelectListItem> Organizations(int? divid)
        {
            var roles = DbUtil.Db.CurrentRoles();
            var q = from ot in DbUtil.Db.DivOrgs
                    where ot.Organization.LimitToRole == null || roles.Contains(ot.Organization.LimitToRole)
                    where ot.DivId == divid
                          && (SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14
                              || ot.Organization.OrganizationStatusId == 30)
                    where (Util2.OrgMembersOnly == false && Util2.OrgLeadersOnly == false) || (ot.Organization.SecurityTypeId != 3)
                    orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                    select new SelectListItem
                    {
                        Value = ot.OrgId.ToString(),
                        Text = CmsData.Organization.FormatOrgName(ot.Organization.OrganizationName,
                            ot.Organization.LeaderName, ot.Organization.Location)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<CategoryClass2> FieldCategories()
        {
            var q = from c in CategoryClass2.Categories
                    where c.Title != "Grouping"
                    select c;
            return q;
        }
        public List<SelectListItem> SavedQueries()
        {
            var cv = new CodeValueModel();
            return ConvertToSelect(cv.UserQueries(), "Code");
        }
        public List<SelectListItem> Ministries()
        {
            var q = from t in Db.Ministries
                    orderby t.MinistryDescription
                    select new SelectListItem
                    {
                        Value = t.MinistryId.ToString(),
                        Text = t.MinistryName
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public Tag TagAllIds()
        {
            var q = DefineModelList();
            var tag = Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_Query);
            Db.TagAll(q, tag);
            return tag;
        }
        public void TagAll(Tag tag = null)
        {
            if (TopClause == null)
                LoadQuery();
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);
            if (tag != null)
                Db.TagAll(q, tag);
            else
                Db.TagAll(q);
        }
        public void UnTagAll()
        {
            if (TopClause == null)
                LoadQuery();
            Db.SetNoLock();
            var q = Db.People.Where(TopClause.Predicate(Db));
            if (TopClause.ParentsOf)
                q = Db.PersonQueryParents(q);
            Db.UnTagAll(q);
        }
        public bool Validate(ModelStateDictionary m)
        {
            SetVisibility();
            DateTime dt = DateTime.MinValue;
            if (StartDateVisible)
                if (!DateTime.TryParse(StartDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("StartDate", "invalid date");
            if (EndDateVisible && EndDate.HasValue())
                if (!DateTime.TryParse(EndDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("EndDate", "invalid date");
            int i = 0;
            if (DaysVisible && !int.TryParse(Days, out i))
                m.AddModelError("Days", "must be integer");
            if (i > 10000)
                m.AddModelError("Days", "days > 10000");
            if (AgeVisible && !int.TryParse(Age, out i))
                m.AddModelError("Age", "must be integer");
            if (IntegerVisible && !Comparison.EndsWith("Null") && !int.TryParse(IntegerValue, out i))
                m.AddModelError("IntegerValue", "need integer");
            if (TagsVisible && string.Join(",", Tags).Length > 500)
                m.AddModelError("tagvalues", "too many tags selected");
            decimal d;
            if (NumberVisible && !Comparison.EndsWith("Null") && !decimal.TryParse(NumberValue, out d))
                m.AddModelError("NumberValue", "need number");
            if (DateVisible && !Comparison.EndsWith("Null"))
                if (!DateTime.TryParse(DateValue, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.AddModelError("DateValue", "need valid date");
            if (Comparison == "Contains")
                if (!TextValue.HasValue())
                    m.AddModelError("TextValue", "cannot be empty");
            return m.IsValid;
        }
    }
}