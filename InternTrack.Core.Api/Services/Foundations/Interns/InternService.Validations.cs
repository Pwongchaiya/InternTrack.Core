﻿using System;
using InternTrack.Core.Api.Models.Interns;
using InternTrack.Core.Api.Models.Interns.Exceptions;

namespace InternTrack.Core.Api.Services.Foundations.Interns
{
    public partial class InternService
    {
        private void ValidateInternOnAdd(Intern intern)
        {
            ValidateInternIsNotNull(intern);

            Validate(
                (Rule: IsInvalid(intern.Id), Parameter: nameof(Intern.Id)),
                (Rule: IsInvalid(intern.FirstName), Parameter: nameof(Intern.FirstName)),
                (Rule: IsInvalid(intern.MiddleName), Parameter: nameof(Intern.MiddleName)),
                (Rule: IsInvalid(intern.LastName), Parameter: nameof(Intern.LastName)),
                (Rule: IsInvalid(intern.Email), Parameter: nameof(Intern.Email)),
                (Rule: IsInvalid(intern.PhoneNumber), Parameter: nameof(Intern.PhoneNumber)),
                (Rule: IsInvalid(intern.Status), Parameter: nameof(Intern.Status)),
                (Rule: IsInvalid(intern.CreatedDate), Parameter: nameof(Intern.CreatedDate)),
                (Rule: IsInvalid(intern.CreatedBy), Parameter: nameof(Intern.CreatedBy)),
                (Rule: IsInvalid(intern.UpdatedDate), Parameter: nameof(Intern.UpdatedDate)),
                (Rule: IsInvalid(intern.UpdatedBy), Parameter: nameof(Intern.UpdatedBy)),
                (Rule: IsNotRecent(intern.CreatedDate), Parameter: nameof(Intern.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: intern.UpdatedDate,
                    secondDate: intern.CreatedDate,
                    secondDateName: nameof(Intern.CreatedDate)),
                Parameter: nameof(Intern.UpdatedDate)));
        }

        private static void ValidateInternIsNotNull(Intern intern)
        {
            if (intern is null)
            {
                throw new NullInternException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void ValidateInternId(Guid InternId) =>
            Validate((Rule: IsInvalid(InternId), Parameter: nameof(Intern.Id)));

        private static void ValidateStorageIntern(Intern intern, Guid internId)
        {
            if (intern is null)
                throw new NotFoundInternException(internId);
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidInternException =
                new InvalidInternException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidInternException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidInternException.ThrowIfContainsErrors();
        }
    }
}
