using Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction.Messaging
{
    public interface IJwtProvider
    {
        string GenerateToken(UserInfo user);
        string GenerateRefreshToken();

    }
}
