/// <summary>
/// dropped the idea of account related audit logging.
/// The initial plan was to log all account related actions like - account created, deleted, role added, removed etc
/// When we make a user as admin, the user:updated event is triggered and not the roles:userAdded
/// But then, the user:Updated event is being triggered for every user action like even a mouse click &
/// this will flood the db table.
/// If I exclude the user:Updated event and just proceed with user:created, user:deleted, roles:useradded & roles:userremoved, 
/// then I don't think this audit logging will be of much help
/// Hence excluding this feature from this module for now.
/// </summary>
namespace Basiscore.CmsAudit.Handlers
{
    using Basiscore.CmsAudit.Models;
    using Basiscore.CmsAudit.Services;
    using Basiscore.CmsAudit.Utilities;
    using Sitecore.Diagnostics;
    using Sitecore.Events;
    using Sitecore.Security.Accounts;
    using System;
    using System.Linq;
    using System.Web.Security;
    using static Basiscore.CmsAudit.Constants;

    public class AccountEventHandler
    {
        private CmsAuditService caService = new CmsAuditService();

        protected void OnUserCreated(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, UserEventType.USER_CREATED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        protected void OnUserRuntimeRolesUpdated(object sender, EventArgs args)
        {
            try
            {
                //CommonEventHandler(sender, args, ItemEventType.ITEM_CREATED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        /// <summary>
        /// Commenting this event and also removed the related user:updatd event in the config
        /// Because, this event gets triggered for every user action, even on a mouse click, which will flood the db table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //protected void OnUserUpdated(object sender, EventArgs args)
        //{
        //    try
        //    {
        //        CommonEventHandler(sender, args, UserEventType.USER_UPDATED, DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(Constants.ModuleName, ex, this);
        //    }
        //}

        protected void OnUserDeleted(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, UserEventType.USER_DELETED, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        protected void OnRoleAddedForUser(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, UserEventType.ROLES_ADDED_FOR_USER, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        protected void OnRoleRemovedForUser(object sender, EventArgs args)
        {
            try
            {
                CommonEventHandler(sender, args, UserEventType.ROLES_REMOVED_FOR_USER, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, this);
            }
        }

        private void CommonEventHandler(object sender, EventArgs args, UserEventType userEvent, DateTime logTime)
        {
            if (caService.IsAuditLoggingEnabled)
            {
                string username = string.Empty;
                string eventAuditLabel = string.Empty;
                string comments = string.Empty;

                switch (userEvent)
                {
                    case UserEventType.USER_CREATED:
                        username = ((MembershipUser)((SitecoreEventArgs)args)?.Parameters[0])?.UserName;
                        eventAuditLabel = Constants.EventAuditLabels.User.USER_CREATED;
                        break;
                    case UserEventType.USER_UPDATED:
                        username = ((MembershipUser)((SitecoreEventArgs)args)?.Parameters[0])?.UserName;
                        eventAuditLabel = Constants.EventAuditLabels.User.USER_UPDATED;
                        break;
                    case UserEventType.USER_DELETED:
                        username = Convert.ToString(((SitecoreEventArgs)args).Parameters[0]);
                        eventAuditLabel = Constants.EventAuditLabels.User.USER_DELETED;
                        break;
                    case UserEventType.ROLES_ADDED_FOR_USER:
                        username = ((string[])((SitecoreEventArgs)args).Parameters[0])[0];
                        comments = "Role(s) Added: " + string.Join(Constants.Comma.ToString(), (string[])((SitecoreEventArgs)args).Parameters[1]);
                        eventAuditLabel = Constants.EventAuditLabels.User.ROLES_ADDED_FOR_USER;
                        break;
                    case UserEventType.ROLES_REMOVED_FOR_USER:
                        username = ((string[])((SitecoreEventArgs)args).Parameters[0])[0];
                        comments = "Role(s) Removed: " + string.Join(Constants.Comma.ToString(), (string[])((SitecoreEventArgs)args).Parameters[1]);
                        eventAuditLabel = Constants.EventAuditLabels.User.ROLES_REMOVED_FOR_USER;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    string roles = string.Empty;
                    string fullName = string.Empty;
                    string email = string.Empty;
                    string domain = string.Empty;
                    bool isAdmin = false;
                    Guid userId = Guid.Empty;

                    User user = SitecoreUtility.GetUserByUsername(username, false);

                    if (user != null)
                    {
                        fullName = user.LocalName;
                        email = user.Profile.Email;
                        domain = user.GetDomainName();
                        isAdmin = user.IsAdministrator;
                        MembershipUser membershipUser = Membership.GetUser(username);

                        if (membershipUser != null)
                        {
                            userId = (Guid)membershipUser.ProviderUserKey;
                        }

                        if (user.Roles != null && user.Roles.Count > 0)
                        {
                            roles = string.Join(Constants.Comma.ToString(), user.Roles.OrderBy(x => x.Name).Select(x => x.Name));
                        }
                    }

                    AuditLog_Account ala = new AuditLog_Account
                    {
                        UserId = userId,
                        Username = username,
                        FullName = fullName,
                        Email = email,
                        Domain = domain,
                        IsAdministrator = isAdmin,
                        Roles = roles,
                        Event = eventAuditLabel,
                        ActionedBy = Sitecore.Context.User.Name,
                        Comments = comments,
                        LoggedTime = logTime
                    };

                    new DbService().InsertAccountAuditLog(ala);
                }
            }
        }
    }
}
