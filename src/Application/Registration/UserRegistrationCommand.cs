﻿using Application.Abstraction.Messaging;
using Domain.Entities;

namespace Application.Registration
{
    public sealed record UserRegistrationCommand(
            int EmployeeId,
            string Username,
            string Password,
            int RoleId
        ) : ICommand<UserInfo>;
}
