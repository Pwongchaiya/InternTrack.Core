﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions.Equivalency.Tracing;
using InternTrack.Core.Api.Models.Interns;
using InternTrack.Core.Api.Models.Interns.Exceptions;
using Moq;
using Xunit;

namespace InternTrack.Core.Api.Tests.Unit.Services.Foundations.Interns
{
    public partial class InternServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidatonExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidInternId = Guid.Empty;
            var invalidInternException = new InvalidInternException();

            invalidInternException.AddData(
               key: nameof(Intern.Id),
               values: "Id is required");

            var expectedInternValidationException =
                new InternValidationException(invalidInternException);

            // when
            ValueTask<Intern> actualInternTask =
                this.internService.RetrieveInternByIdAsync(invalidInternId);

            // then
            await Assert.ThrowsAsync<InternValidationException>(() => actualInternTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameValidationExceptionAs(
                    expectedInternValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectInternByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidatonExceptionOnRetriveWhenStorageInternIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Intern randomIntern = CreateRandomIntern(dateTime);
            Guid inputInternId = randomIntern.Id;
            Intern inputIntern = randomIntern;
            Intern nullStorageIntern = null;

            var notFoundInternException = new NotFoundInternException(inputInternId);

            var expectedInternValidationException =
                new InternValidationException(notFoundInternException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectInternByIdAsync(inputInternId))
                    .ReturnsAsync(nullStorageIntern);

            // when
            ValueTask<Intern> actualInternTask =
                this.internService.RetrieveInternByIdAsync(inputInternId);

            // then
            await Assert.ThrowsAsync<InternValidationException>(() => actualInternTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionsAs(
                    expectedInternValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectInternByIdAsync(inputInternId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}