using System;

namespace GuiTests.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PageUrlAttribute : Attribute
    {
        public string PartUrl { get; private set; }
        public string TestCase { get; set; }
        public bool Default { get; set; }

        public PageUrlAttribute(string partUrl, string testCase, bool @default)
            : this(partUrl, testCase)
        {
            Default = @default;
        }

        public PageUrlAttribute(string partUrl, string testCase)
            : this(partUrl)
        {
            TestCase = testCase;
        }

        public PageUrlAttribute(string partUrl)
        {
            PartUrl = partUrl;
            Default = false;
        }
    }
}
