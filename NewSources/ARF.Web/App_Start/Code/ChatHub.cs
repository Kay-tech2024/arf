using ARF.Core;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;

namespace ARF.Web.App_Start.Code
{
    public class ChatHub : Hub
    {
        public void SendMesage(string subject, string message, string toUsers, string toRoles, string toColleges, bool isAnnoucement, string inReplyTo)
        {
            if (Context.User != null && Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                var username = Context.User.Identity.Name;
                var db = Utils.db;

                var session = db.UserConnections
                    .Where(u => u.User.UserName == username || u.User.Email == username)
                    .Where(u => u.IsConnected == true)
                    .OrderByDescending(u => u.Created)
                    .FirstOrDefault();

                if (session != null)
                {

                    var user = session.User;

                    var predicate = PredicateBuilder.BaseAnd<UserConnection>();

                    // must have a current connection
                    predicate = predicate.And(s => s.IsConnected == true);

                    // must have a user
                    predicate = predicate.And(s => s.UserId != null);


                    var toValidUserIds = (string)null;
                    var toValidRoleIds = (string)null;
                    var toValidCollegeIds = (string)null;

                    // if sending to specific users, must be one of them
                    if (!string.IsNullOrWhiteSpace(toUsers))
                    {
                        var subpredicate = PredicateBuilder.BaseOr<UserConnection>();

                        var userIds = toUsers.Split('|');
                        var foundValid = false;

                        foreach (var u in userIds)
                        {
                            if (string.IsNullOrWhiteSpace(u)) continue;

                            foundValid = true;
                            subpredicate = subpredicate.Or(s => s.UserId == u);


                            if (toValidUserIds == null)
                            {
                                toValidUserIds = u;
                            }
                            else
                            {
                                toValidUserIds += "|" + u;
                            }
                        }


                        if (!foundValid)
                        {
                            subpredicate = subpredicate.Or(s => false);
                        }

                        predicate = predicate.And(subpredicate);
                    }

                    // if sending to specific roles, must be one of them
                    if (!string.IsNullOrWhiteSpace(toRoles))
                    {
                        var subpredicate = PredicateBuilder.BaseOr<UserConnection>();

                        var roles = toRoles.Split('|');
                        var foundValid = false;

                        foreach (var u in roles)
                        {
                            if (string.IsNullOrWhiteSpace(u)) continue;

                            foundValid = true;
                            subpredicate = subpredicate.Or(s => s.User.Roles.Count(r => r.Id == u || r.Name.ToLower() == u.ToLower()) > 0);


                            if (toValidRoleIds == null)
                            {
                                toValidRoleIds = u;
                            }
                            else
                            {
                                toValidRoleIds += "|" + u;
                            }
                        }


                        if (!foundValid)
                        {
                            subpredicate = subpredicate.Or(s => false);
                        }

                        predicate = predicate.And(subpredicate);
                    }

                    // if sending to specific colleges, must be one of them
                    if (!string.IsNullOrWhiteSpace(toColleges))
                    {
                        var subpredicate = PredicateBuilder.BaseOr<UserConnection>();

                        var roles = toRoles.Split('|');
                        var foundValid = false;

                        foreach (var u in roles)
                        {
                            if (string.IsNullOrWhiteSpace(u)) continue;
                            var collegeId = 0;
                            if (!int.TryParse(u, out collegeId) || collegeId <= 0) continue;


                            foundValid = true;
                            subpredicate = subpredicate.Or(s => s.User.CollegeId == collegeId);


                            if (toValidCollegeIds == null)
                            {
                                toValidCollegeIds = u;
                            }
                            else
                            {
                                toValidCollegeIds += "|" + u;
                            }

                        }


                        if (!foundValid)
                        {
                            subpredicate = subpredicate.Or(s => false);
                        }

                        predicate = predicate.And(subpredicate);
                    }

                    var inReplyToMessageId = (long)0;

                    if (!string.IsNullOrEmpty(inReplyTo))
                    {
                        if (!long.TryParse(inReplyTo, out inReplyToMessageId) || inReplyToMessageId <= 0)
                        {
                            inReplyToMessageId = 0;
                        }
                    }

                    var msg = new Message
                    {
                        IsAnnouncement = isAnnoucement,
                        IsDeleted = false,
                        ToUsers = toValidUserIds,
                        ToColleges = toValidCollegeIds,
                        ToRoles = toValidRoleIds,

                        Subject = subject,
                        Body = message,

                        InReplyTo = inReplyToMessageId == 0 ? (long?)null : inReplyToMessageId,
                        Sent = TM.Now,
                        SentBy = user.Id
                    };


                    db.Messages.Add(msg);
                    db.SaveChanges();


                    var eligibleUsers = db.UserConnections
                        .Where(predicate)
                        .ToList();

                    foreach (var eu in eligibleUsers)
                    {
                        Clients.Client(eu.ConnectionId).renderMessage(msg.Id, user.Id, user.UserProfile.GetFullName(), "", subject, message, isAnnoucement, inReplyTo, msg.Sent);
                    }

                }
            }


        }


        public void GetMessageHistory(string userId, long? fromMessageId, int? take)
        {

            if (Context.User != null && Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                var username = Context.User.Identity.Name;
                var db = Utils.db;

                var session = db.UserConnections
                    .Where(u => u.User.UserName == username || u.User.Email == username)
                    .Where(u => u.IsConnected == true)
                    .OrderByDescending(u => u.Created)
                    .FirstOrDefault();

                if (session != null)
                {

                    var predicate = PredicateBuilder.BaseOr<Message>();
                    var isAnnouncement = string.IsNullOrWhiteSpace(userId);
                    predicate = predicate.Or(m => (isAnnouncement && m.IsAnnouncement == true) || (!isAnnouncement && m.SentBy == userId));


                    var user = session.User;




                    var subpredicate = PredicateBuilder.BaseAnd<Message>();


                    // user IDs
                    {
                        subpredicate = subpredicate.And(m => m.ToUsers == null || (m.ToUsers.Contains(user.Id) && m.SentBy == userId) || (m.ToUsers.Contains(userId) && m.SentBy == user.Id));
                    }

                    // role IDs
                    {
                        var role = user.Roles.FirstOrDefault();

                        if (role != null)
                        {
                            subpredicate = subpredicate.And(m => m.ToRoles == null || m.ToRoles.Contains(role.Id) || m.ToRoles.Contains(role.Name));
                        }
                        else
                        {
                            subpredicate = subpredicate.And(m => m.ToRoles == null);
                        }
                    }

                    // college IDs
                    {

                        if (user.CollegeId > 0)
                        {
                            subpredicate = subpredicate.And(m =>
                                m.ToColleges == null ||
                                m.ToColleges == "" + user.CollegeId ||
                                m.ToColleges.Contains("|" + user.CollegeId + "|") ||
                                m.ToColleges.EndsWith("|" + user.CollegeId) ||
                                m.ToColleges.StartsWith(user.CollegeId + "|")
                            );
                        }
                        else
                        {
                            subpredicate = subpredicate.And(m => m.ToRoles == null);
                        }
                    }



                    predicate = predicate.Or(subpredicate);

                    var messages = db.Messages.Where(predicate)
                        .OrderBy(m => m.Sent)

                        .ToList().Select(m =>
                            new
                            {
                                MessageId = m.Id,
                                Userid = m.SentBy,
                                Name = m.User.UserProfile.GetFullName(),
                                PhotoUrl = "",
                                Subject = m.Subject,
                                Body = m.Body,
                                IsAnnouncement = m.IsAnnouncement,
                                InreplyTo = m.InReplyTo,
                                SentOn = m.Sent
                            }
                        )
                        .ToList();


                    Clients.Client(session.ConnectionId).renderMessageHistory(messages);
                }
            }

        }





        public void UpdateUnseenMessageCount()
        {
            var rand = new Random();
            var connId = Context.ConnectionId;
            Clients.All.renderUnseenMessageCount(rand.Next(0, 20));


        }


        public void GetChatBuddies(string lastUserID)
        {

            if (Context.User != null && Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                var username = Context.User.Identity.Name;
                var db = Utils.db;

                var session = db.UserConnections
                    .Where(u => u.User.UserName == username || u.User.Email == username)
                    .Where(u => u.IsConnected == true)
                    .OrderByDescending(u => u.Created)
                    .FirstOrDefault();

                if (session != null)
                {

                    session.ConnectionId = Context.ConnectionId;
                    db.SaveChanges();



                    var onlineUsers = db.UserConnections
                        .Where(u => u.UserId != null)
                        .Where(u => u.UserId != session.UserId)
                        .Where(u => u.IsConnected == true)
                        .Where(u => u.User.Roles.Count() > 0)
                        .ToList()
                        .Select(u => new
                        {
                            ConnId = u.ConnectionId,
                            UserId = u.UserId,
                            Role = LangHelpers.Instance[u.User.Roles.First().Name],
                            Name = u.User.UserProfile.GetFullName(),
                            PhotoUrl = "",
                            LastMessageTime = (DateTime?)null
                        })
                        .ToList();


                    onlineUsers.Insert(0, new
                    {
                        ConnId = "",
                        UserId = (string)null,
                        Role = LangHelpers.Instance["announcements"],
                        Name = LangHelpers.Instance["announcements"],
                        PhotoUrl = "",
                        LastMessageTime = (DateTime?)null
                    });

                    Clients.Client(session.ConnectionId).renderChatBuddies(onlineUsers, string.IsNullOrWhiteSpace(lastUserID));
                }
            }



        }



        public override System.Threading.Tasks.Task OnConnected()
        {
            if (Context.User != null && Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                var username = Context.User.Identity.Name;
                var db = Utils.db;



                var session = db.UserConnections
                    .Where(u => u.User.UserName == username || u.User.Email == username)
                    .Where(u => u.IsConnected == true)
                    .OrderByDescending(u => u.Created)
                    .FirstOrDefault();

                var user = db.Users.FirstOrDefault(u => u.UserName == username || u.Email == username);

                if (session == null && user != null)
                {

                    session = new UserConnection
                    {
                        UserId = user.Id,
                        ConnectionId = Context.ConnectionId,
                        IsConnected = true,
                        Updated = TM.Now,
                        Created = TM.Now
                    };

                    db.UserConnections.Add(session);
                    db.SaveChanges();


                    Clients.All.renderRegister(session.ConnectionId);
                }
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            if (Context.User != null && Context.User != null && Context.User.Identity != null && !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                var username = Context.User.Identity.Name;
                var db = Utils.db;

                var session = db.UserConnections
                    .Where(u => u.User.UserName == username || u.User.Email == username)
                    .Where(u => u.IsConnected == true)
                    .OrderByDescending(u => u.Created)
                    .FirstOrDefault();

                if (session != null)
                {


                    session.IsConnected = false;
                    db.SaveChanges();
                    Clients.All.renderUnregister(session.ConnectionId);
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {
            OnConnected();

            return base.OnReconnected();
        }

    }
}