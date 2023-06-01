﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace InternTrack.Core.Api.Models.Interns.Exceptions
{
    public class FailedInternServiceException : Xeption
    {
        public FailedInternServiceException(Exception innerException)
            : base(message: "Failed intern service occurred, please contact support.", innerException)
        { }
    }
}
