using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration.Extensions;

public static class AssertExtensions
{
    /// <summary>
    /// Validates a single validation error inside the ValidationProblemDetails object.
    /// </summary>
    /// <param name="assert"></param>
    /// <param name="validationProblemDetails">The validation problem details.</param>
    /// <param name="expectedFieldName">The expected field that is invalid.</param>
    /// <param name="expectedErrorMessage">The expected error message</param>
    /// <returns></returns>
    public static void SingleValidationProblemDetailsErrorIsValid(this Assert assert, ValidationProblemDetails validationProblemDetails, string expectedFieldName, string expectedErrorMessage)
    {
        Assert.AreEqual(1, validationProblemDetails.Errors.Count);
        var error = validationProblemDetails.Errors.Single();
        Assert.AreEqual(expectedFieldName, error.Key);
        Assert.AreEqual(1, error.Value.Length);
        Assert.AreEqual(expectedErrorMessage, error.Value.Single());
    }
}