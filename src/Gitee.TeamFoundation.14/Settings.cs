using Microsoft.TeamFoundation.Controls;

namespace Gitee.TeamFoundation
{
    internal static class Settings
    {
        public const string InvitationSectionId = "C2443FCC-6D62-4D31-B08A-C4DE70109C7F";
        public const int InvitationSectionPriority = 100;

        public const string ConnectSectionId = "80F53301-4049-42D1-BC9C-A3121A3F73E9";
        public const int ConnectSectionPriority = 10;

        public const string HomeSectionId = "7DDFCD61-C0CD-48A3-BB7A-2A6603DFDBDC";
        public const int HomeSectionPriority = 10;

        public const string PublishSectionId = "426F751E-120E-4078-9A13-03374E181034";
        public const int PublishSectionPriority = 10;

        public const string IssuesNavigationItemId = "3B107B33-2BDF-4D31-9564-DE4A06DAA4F9";
        public const int Issues = TeamExplorerNavigationItemPriority.GitCommits - 1;

        public const string PullRequestsNavigationItemId = "AC35B0CA-D52D-4019-8C10-6EB238B18E97";
        public const int PullRequests = TeamExplorerNavigationItemPriority.GitCommits - 2;

        public const string AttachmentsNavigationItemId = "CC837A29-C9DE-41E9-9D0E-7402CA92F0C5";
        public const int Attachments = TeamExplorerNavigationItemPriority.GitCommits - 3;

        public const string WikiNavigationItemId = "5C08763C-3713-4180-976A-D1D0342A489C";
        public const int Wiki = TeamExplorerNavigationItemPriority.Settings - 4;

        public const string StatisticsNavigationItemId = "BAD40359-E95C-4CD2-96F9-4C4016114A1B";
        public const int Statistics = TeamExplorerNavigationItemPriority.Settings - 5;
    }
}