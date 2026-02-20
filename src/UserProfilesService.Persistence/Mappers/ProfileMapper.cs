using Contracts;
using Documents;
using Riok.Mapperly.Abstractions;

namespace Mappers;

[Mapper]
public static partial class UserProfileMapper
{
    public static partial IQueryable<UserProfile> ProjectToDomainModel(this IQueryable<ProfileDocument> document);

    public static partial UserProfile ToDomainModel(this ProfileDocument document);

    public static partial ProfileDocument ToBson(this UserProfile profile);
}
