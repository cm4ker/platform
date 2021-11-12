// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Xml;
using System.Xml.Serialization;

namespace Aquila.SyntaxGenerator2
{
    public class Comment
    {
        [XmlAnyElement]
        public XmlElement[] Body;
    }
}
