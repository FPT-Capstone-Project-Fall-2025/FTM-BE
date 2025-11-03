namespace FTM.Domain.Constants
{
    public static partial class Constants
    {
    public static class FamilyTreeModes
    {
        public const int PRIVATE = 1;
        public const int PUBLIC = 2;
        public const int SHARED = 3;
    }

        public static class FTMemberStatus
        {
            public const int UNDEFINED = Category.FT_MEMBER_STATUS_CATEGORY * 1000 + 1;
            public const int DIVORCED = Category.FT_MEMBER_STATUS_CATEGORY * 1000 + 2;
        }

        public static class FTRelationshipCategory
        {
            public const int PARENT = Category.FT_MEMBER_RELATIONSHIP_CATEGORY * 1000 + 1;
            public const int PARTNER = Category.FT_MEMBER_RELATIONSHIP_CATEGORY * 1000 + 2;
            public const int SIBLING = Category.FT_MEMBER_RELATIONSHIP_CATEGORY * 1000 + 3;
            public const int CHILDREN = Category.FT_MEMBER_RELATIONSHIP_CATEGORY * 1000 + 4;
        }
    }
}