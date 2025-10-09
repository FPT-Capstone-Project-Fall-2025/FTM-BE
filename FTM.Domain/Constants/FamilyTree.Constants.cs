namespace FTM.Domain.Constants
{
    public static class FamilyTreeModes
    {
        public const int PRIVATE = 1;
        public const int PUBLIC = 2;
        public const int SHARED = 3;
    }

    public static class FTMemberStatus
    {
        public const int ACTIVE = 1;
        public const int INACTIVE = 2;
        public const int PENDING = 3;
    }

    public static class FTRelationshipCategory
    {
        public const int PARENT = 1;
        public const int PARTNER = 2;
        public const int SIBLING = 3;
        public const int CHILDREN = 4;
    }
}