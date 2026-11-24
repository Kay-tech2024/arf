using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARF.Core.Managers
{
    public class UsersManager : AbstractManager
    {
        public IQueryable<User> GetUsers(string query = null, bool includeDeleted = true, Translator translator = null, bool avoidCollegeIdFilter=false)
        {
            var predicate = PredicateBuilder.BaseAnd<User>();

            if (IsCollegeUser)
            {
                if (!avoidCollegeIdFilter)
                {
                    if (IsCollegeUser)
                    {
                        if (CurrentUser.CollegeId == null) return new List<User>().AsQueryable();

                        predicate = predicate.And(p => p.CollegeId == CurrentUser.CollegeId);
                    }
                    else
                    {
                        if (CurrentUser.CollegeId == null) return new List<User>().AsQueryable();

                        predicate = predicate.And(p => p.CollegeId == CurrentUser.CollegeId);
                    }
                }
            }
            else if (!IsSystemAdmin)
            {
                predicate = predicate.And(p => p.Roles.Count(r => r.Name == ArfUserRole.SystemAdmin.ToString()) == 0);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                var predicate2 = PredicateBuilder.BaseOr<User>();

                predicate2 = predicate2.Or(a => a.FullName.Contains(query));
                predicate2 = predicate2.Or(a => a.UserName.Contains(query));
                predicate2 = predicate2.Or(a => a.Email.Contains(query));

                predicate2 = predicate2.Or(a => a.UserProfile != null && ((a.UserProfile.FirstName != null && a.UserProfile.FirstName.Contains(query)) || (a.UserProfile.LastName != null && a.UserProfile.LastName.Contains(query)) || (a.UserProfile.FirstName != null && a.UserProfile.LastName != null && (a.UserProfile.FirstName + " " + a.UserProfile.LastName).Contains(query))));
                predicate2 = predicate2.Or(a => a.Referees.Count(r => (r.FirstName != null && r.FirstName.Contains(query)) || (r.LastName != null && r.LastName.Contains(query)) || (r.FirstName != null && r.LastName != null && (r.FirstName + " " + r.LastName).Contains(query))) > 0);
                predicate2 = predicate2.Or(a => a.Researchers.Count(r => (r.FirstName != null && r.FirstName.Contains(query)) || (r.LastName != null && r.LastName.Contains(query)) || (r.FirstName != null && r.LastName != null && (r.FirstName + " " + r.LastName).Contains(query))) > 0);

                if (translator != null)
                {
                    var roles = Enum.GetValues(typeof(ArfUserRole));

                    var matchingRoles = new List<string>();

                    foreach (var r in roles)
                    {
                        var arabic = translator.GetLangValueByString(r.ToString(), true, "ar");
                        var english = translator.GetLangValueByString(r.ToString(), true, "en");

                        if (arabic != null && arabic.Contains(query))
                            matchingRoles.Add(r.ToString());
                        else if (english != null && english.Contains(query))
                            matchingRoles.Add(r.ToString());
                    }

                    if (matchingRoles.Count > 0)
                    {
                        foreach (var x in matchingRoles)
                        {
                            predicate2 = predicate2.Or(a => a.Roles.Count(r => r.Name == x) > 0);
                        }
                    }
                }

                predicate = predicate.And(predicate2);
            }

            if (!includeDeleted)
            {
                predicate = predicate.And(u => u.IsDeleted == false);
            }

            var result = DbContext.Users.Where(predicate);
            return result;
        }


        public IQueryable<Referee> GetReferees(string query = null, bool includeDeleted = true, Translator translator = null, string FilterUserId = null)
        {
            var predicate = PredicateBuilder.BaseAnd<Referee>();

            if (IsCollegeUser)
            {
                if (IsCollegeUser)
                {
                    if (CurrentUser.CollegeId == null) return new List<Referee>().AsQueryable();

                    predicate = predicate.And(p => p.CollegeId == CurrentUser.CollegeId);
                }
                else
                {
                    if (CurrentUser.CollegeId == null) return new List<Referee>().AsQueryable();

                    predicate = predicate.And(p => p.CollegeId == CurrentUser.CollegeId);
                }
            }


            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                var predicate2 = PredicateBuilder.BaseOr<Referee>();

                predicate2 = predicate2.Or(r =>
                    (r.FirstName != null && r.FirstName.ToLower().Contains(query)) ||
                    (r.SecondName != null && r.SecondName.ToLower().Contains(query)) ||
                    (r.ThirdName != null && r.ThirdName.ToLower().Contains(query)) ||
                    (r.LastName != null && r.LastName.ToLower().Contains(query)) ||
                    (r.FirstName != null && r.SecondName != null && (r.FirstName + " " + r.SecondName).ToLower().Contains(query)) ||
                    (r.FirstName != null && r.SecondName != null && r.ThirdName != null && (r.FirstName + " " + r.SecondName + " " + r.ThirdName).ToLower().Contains(query)) ||
                    (r.FirstName != null && r.SecondName != null && r.ThirdName != null && r.LastName != null && (r.FirstName + " " + r.SecondName + " " + r.ThirdName + " " + r.LastName).ToLower().Contains(query)) ||
                    (r.FirstName != null && r.ThirdName != null && r.LastName != null && (r.FirstName + " " + r.ThirdName + " " + r.LastName).ToLower().Contains(query)) ||
                    (r.FirstName != null && r.LastName != null && (r.FirstName + " " + r.LastName).ToLower().Contains(query))

                );

                predicate2 = predicate2.Or(a => a.Email != null && a.Email.Contains(query));
                predicate2 = predicate2.Or(a => a.MobileNo != null && a.MobileNo.Contains(query));
                predicate = predicate.And(predicate2);
            }

            if (!includeDeleted)
            {
                predicate = predicate.And(u => u.IsDeleted == false);
            }

            if (!String.IsNullOrEmpty(FilterUserId))
            {
                predicate = predicate.And(u => u.UserId != FilterUserId);
            }
            var result = DbContext.Referees.Where(predicate);
            return result;
        }

        public Dictionary<ArfUserRole, int?> GetDelegations(string userId = null)
        {
            var v = Utils.GetDictVal("EXTRA_ROLES");
            if (v != null) return (Dictionary<ArfUserRole, int?>)v;

            if (userId == null || !Utils.HasPrivilagesAny(Privilages.users_create_user))
            {
                userId = CurrentUser.Id;
            }

            var db = Utils.db;
            var extraRoles = db.UserExtraRoles
                    .Where(x => x.UserId == userId && x.IsActive == true && (x.EffectiveFrom == null || x.EffectiveFrom <= TM.Now) && (x.EffectiveTo == null || x.EffectiveTo >= TM.Now))
                    .Select(x => new
                    {
                        RoleName = x.Role.Name,
                        CollegeId = x.CollegeId
                    })
                    .ToList();

            var listRoles = new Dictionary<ArfUserRole, int?>();

            foreach (var d in extraRoles)
            {
                var role = (ArfUserRole)Enum.Parse(typeof(ArfUserRole), d.RoleName, true);

                listRoles.Add(role, Utils.IsCollegeRole(role) ? d.CollegeId : null);
            }

            Utils.SetDictVal("EXTRA_ROLES", listRoles);

            return listRoles;
        }

        public bool CanChangeRoleTo(ArfUserRole role, long? collegeId)
        {
            if (ActualRoles == null || ActualRoles.Length == 0 || ActualRoles[0] == ArfUserRole.None) return false;

            if (ActualRoles[0] == ArfUserRole.SystemAdmin) return true;
            if (ActualRoles[0] == role) return true;

            var delegations = GetDelegations();

            if (delegations.ContainsKey(role) && (!Utils.IsCollegeRole(role) || delegations[role] == collegeId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}