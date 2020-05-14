using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aquila.DataComponent
{
    /// <summary>
    /// Определяет, как компонент взаимодействует с другими компонентами в плане отношения
    /// 
    /// Пример: 
    ///     1) Справочник и документ могут иметь связть только 1 - 0..1
    ///     2) Табличная часть и справочник могут иметь связь 1 - 0..*
    ///     3) Регистр и документ имеют отношение 1 - 0..*
    /// </summary>
    public enum RelationType
    {
        OneTo,
        OneToOneOrZero,
        OneToMany
    }
}