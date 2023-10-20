using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums;

public enum OrganizationInvitationState
{
    Pending,
    Accepted,
    Declined,
    // TODO: Expired, Canceled?
}
