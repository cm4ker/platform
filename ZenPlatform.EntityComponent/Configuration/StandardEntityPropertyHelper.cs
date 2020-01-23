using System;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    // internal static class StandardEntityPropertyHelper
    // {
    //     public static XCSingleEntityProperty CreateUniqueProperty()
    //     {
    //         return new XCSingleEntityProperty()
    //         {
    //             Name = "Id",
    //             Guid = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
    //             DatabaseColumnName = "Id",
    //             Types = {PlatformTypesFactory.Guid},
    //             IsSystemProperty = true,
    //             Unique = true,
    //             IsReadOnly = true
    //         };
    //     }
    //
    //     public static IXCProperty CreateLinkProperty(XCSingleEntity type)
    //     {
    //         var linkType = type.Parent.Types.First(x => x is XCLinkTypeBase a && a.ParentType == type) as IXCLinkType;
    //
    //         return CreateLinkProperty(linkType);
    //     }
    //     
    //     public static IXCProperty CreateLinkProperty(IXCLinkType linkType)
    //     {
    //         return new XCSingleEntityLinkProperty
    //         {
    //             Types = {linkType},
    //             Name = "Link",
    //             DatabaseColumnName = "<U N K N O W N>",
    //             Guid = Guid.Parse("7976d8c6-ce1a-4ec4-b965-be394e215670"),
    //             IsSystemProperty = false,
    //             IsReadOnly = true,
    //         };
    //     }
    //
    //     public static XCSingleEntityProperty CreateNameProperty()
    //     {
    //         return new XCSingleEntityProperty()
    //         {
    //             Name = "Name",
    //             Guid = Guid.Parse("7976d8c6-ce1a-4ec4-b965-be394e215689"),
    //             DatabaseColumnName = "Name",
    //             Types = {PlatformTypesFactory.GetString(150)},
    //             IsSystemProperty = false,
    //             Unique = false,
    //             IsReadOnly = false
    //         };
    //     }
    // }
}