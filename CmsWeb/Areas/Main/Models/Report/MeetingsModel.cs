using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Report
{
    public class MeetingsModel : OrgSearchModel
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }

        public bool NoZero { get; set; }

        public MeetingsModel()
        {
            Direction = "asc";
            Sort = "--Attendance--";
            NoZero = true;
        }

        public int MeetingsCount { get; set; }
        public int TotalHeadCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalAttends { get; set; }
        public int TotalGuests { get; set; }
        public int OtherAttends { get; set; }
        public int TotalPeople { get; set; }

        public IEnumerable<MeetingInfo> MeetingsForDate()
        {
            if (FromWeekAtAGlance)
                StatusId = null;
            var q = FetchOrgs();
            var list = (from o in q
                        join m in DbUtil.Db.Meetings on o.OrganizationId equals m.OrganizationId into mr
                        from m in mr.Where(m => m.MeetingDate >= Dt1 && m.MeetingDate <= Dt2).DefaultIfEmpty()
                        where (m != null && m.MaxCount > 0) || NoZero == false
                        let div = o.Division
                        select new MeetingInfo
                        {
                            Program = div.Program.Name,
                            Division = div.Name,
                            OrganizationId = o.OrganizationId,
                            Organization = o.OrganizationName,
                            MeetingId = m.MeetingId,
                            Tracking = m.GroupMeetingFlag ? "Group" : "Individual",
                            Time = m.MeetingDate,
                            HeadCount = m.HeadCount,
                            MaxCount = m.MaxCount,
                            Attended = m.NumPresent,
                            Guests = m.NumNewVisit + m.NumRepeatVst,
                            Leader = o.LeaderName,
                            Description = m.Description,
                            OtherAttends = m.NumOtherAttends,
                            Inactive = o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive
                        }).ToList();

            MeetingsCount = list.Count(a => a.Attended > 0);
            TotalAttends = list.Sum(m => m.Attended ?? 0);
            TotalHeadCount = list.Sum(m => m.HeadCount ?? 0);
            TotalCount = list.Sum(m => m.MaxCount ?? 0);
            OtherAttends = list.Sum(m => m.OtherAttends ?? 0);

            var attends = from o in q
                          join m in DbUtil.Db.Meetings on o.OrganizationId equals m.OrganizationId into mr
                          from m in mr.Where(m => m.MeetingDate >= Dt1 && m.MeetingDate <= Dt2)
                          let div = o.Division
                          from a in m.Attends
                          where a.AttendanceFlag == true
                          select a;

            TotalPeople = attends.Select(pp => pp.PeopleId).Distinct().Count();

            attends = from a in attends
                      where a.AttendanceTypeId == 50 || a.AttendanceTypeId == 60
                      select a;

            TotalGuests = attends.Select(pp => pp.PeopleId).Distinct().Count();

            switch (Sort)
            {
                case "Recorded":
                    return from m in list
                           orderby m.Attended descending
                           select m;
                case "Count":
                    return from m in list
                           orderby m.HeadCount descending, m.Attended descending
                           select m;
                case "Guests":
                    return from m in list
                           orderby m.Guests descending, m.MaxCount descending
                           select m;
                case "Leader":
                    return from m in list
                           orderby m.Leader, m.Time, m.Division, m.Organization
                           select m;
                case "Time":
                    return from m in list
                           orderby m.Time, m.Division, m.Organization
                           select m;
                case "Division":
                    return from m in list
                           orderby m.Division, m.Organization, m.Time
                           select m;
                case "Organization":
                    return from m in list
                           orderby m.Organization, m.Time
                           select m;
                default:
                    return from m in list
                           orderby m.MaxCount descending
                           select m;
            }
        }

        public string ConvertToSearch(string type)
        {
            if (Fingerprint.UseNewLook())
                return ConvertToQuery(type);
            return ConvertToSearchBuilder(type);
        }

        public string ConvertToSearchBuilder(string type)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);

            QueryBuilderClause nc = null;
            if (type == "Guests")
                nc = qb.AddNewClause(QueryType.GuestAsOf, CompareType.Equal, "1,T");
            else
                nc = qb.AddNewClause(QueryType.AttendedAsOf, CompareType.Equal, "1,T");
            if (ProgramId.HasValue && ProgramId > 0)
                nc.Program = ProgramId.Value;
            if (DivisionId.HasValue && DivisionId > 0)
                nc.Division = DivisionId.Value;
            nc.StartDate = Dt1;
            nc.EndDate = Dt2;
            DbUtil.Db.SubmitChanges();
            return "/QueryBuilder/Main/" + qb.QueryId;
        }
        public string ConvertToQuery(string type)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset(DbUtil.Db);
            var c = cc.AddNewClause(type == "Guests" ? QueryType.GuestAsOf : QueryType.AttendedAsOf, CompareType.Equal, "1,T");
            if (ProgramId.HasValue && ProgramId > 0)
                c.Program = ProgramId.Value;
            if (DivisionId.HasValue && DivisionId > 0)
                c.Division = DivisionId.Value;
            c.StartDate = Dt1;
            c.EndDate = Dt2;
            cc.Save(DbUtil.Db);
            return "/Query/" + cc.Id;
        }
    }
    public class MeetingInfo
    {
        public string Program { get; set; }
        public string Division { get; set; }
        public int OrganizationId { get; set; }
        public int? MeetingId { get; set; }
        public string Organization { get; set; }
        public string Tracking { get; set; }
        public DateTime? Time { get; set; }
        public int? HeadCount { get; set; }
        public int? MaxCount { get; set; }
        public int? Attended { get; set; }
        public int? Guests { get; set; }
        public int? OtherAttends { get; set; }
        public string Leader { get; set; }
        public string Description { get; set; }
        public bool Inactive { get; set; }
    }
}