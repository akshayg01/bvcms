using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgMemberMoveModel : PagedTableModel<Organization, OrgMoveInfo>
    {
        private int? orgId;
        private int? peopleId;
        private void Populate()
        {
            var i = (from mm in DbUtil.Db.OrganizationMembers
                     where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                     select new
                         {
                             mm,
                             mm.Person.Name,
                             mm.Organization.OrganizationName,
                         }).Single();
            Name = i.Name;
            OrgName = i.OrganizationName;
        }

        public OrgMemberMoveModel() : base("", "")
        {
            Pager.pagesize = 10;
            Pager.ShowPageSize = false;
        }

        public string OrgSearch { get; set; }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                    Populate();
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                    Populate();
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }

        public override IQueryable<Organization> DefineModelList()
        {
            return from o in DbUtil.Db.Organizations
                   let org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == OrgId)
                   where o.DivOrgs.Any(dd => org.DivOrgs.Any(oo => oo.DivId == dd.DivId))
                   where o.OrganizationId != OrgId
                   where o.OrganizationStatusId == OrgStatusCode.Active
                   where !OrgSearch.HasValue() || o.OrganizationName.Contains(OrgSearch)
                   select o;
        }

        public override IQueryable<Organization> DefineModelSort(IQueryable<Organization> q)
        {
            return q.OrderBy(m => m.OrganizationName);
        }

        public override IEnumerable<OrgMoveInfo> DefineViewList(IQueryable<Organization> q)
        {
            return from o in q
                   select new OrgMoveInfo
                   {
                       OrgName = o.OrganizationName,
                       ToOrgId = o.OrganizationId,
                       PeopleId = PeopleId.Value,
                       FromOrgId = OrgId.Value,
                       Program = o.Division.Program.Name,
                       Division = o.Division.Name,
                       orgSchedule = o.OrgSchedules.First()
                   };
        }
    }
}
