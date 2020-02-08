namespace Common.Core
{
    public static class NonEmpty
    {
        public static NonEmptyGuid Guid() => System.Guid.NewGuid().NonEmpty();
    }
}
