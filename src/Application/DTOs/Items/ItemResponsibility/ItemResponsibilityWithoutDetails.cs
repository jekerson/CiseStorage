using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Items.ItemResponsibility
{
    public record ItemResponsibilityWithoutDetails(
        string ItemName,
        string ItemNumber,
        string AssignedAt
    );
}
