namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers.Data;

public static class ProfileTestData
{
    public static TheoryData<string, string, string, string, bool, string?> TestCases =>
        new()
        {
            { "Test", "Tester", "test@Test.com", "83339347", true, null },
            { "", "Tester", "test@Test.com", "83339347", false, "First name is required" },
            { "Test", "", "test@Test.com", "83339347", false, "Last name is required" },
            { "Test", "Tester", "", "83339347", false, "Email is required" },
            { "Test", "Tester", "FailedEmail.com", "83339347", false, "Wrong formatted email" },
            { "Test", "Tester", "test@Test", "", false, "Wrong formatted email" },
            { "Test", "Tester", "@Test.com", "", false, "Wrong formatted email" },
            { "Test", "Tester", "Test@.com", "", false, "Wrong formatted email" },
            { "Test", "Tester", "@.com", "", false, "Wrong formatted email" },
        };
}