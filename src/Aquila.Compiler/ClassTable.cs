using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Aquila.Compiler.Contracts;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Compiler
{
    public class ClassTable
    {
        private class TableClassRow
        {
            public string FullName { get; set; }

            public string Name { get; set; }

            public IType Type { get; set; }

            public TypeEntity Class { get; set; }
        }

        private List<TableClassRow> _rows;

        public ClassTable()
        {
            _rows = new List<TableClassRow>();
        }

        /// <summary>
        /// Добавляет связку
        /// </summary>
        /// <param name="fullName">Платформенное пространство имён</param>
        /// <param name="type"></param>
        /// <param name="class"></param>
        public void AddClass(TypeEntity @class, IType type)
        {
            AddClass("", @class, type);
        }


        public void AddClass(string ns, TypeEntity @class, IType type)
        {
            _rows.Add(new TableClassRow
            {
                FullName = $"{ns}.{@class.Name}", Type = type,
                Class = @class, Name = @class.Name
            });
        }

        private TableClassRow FindRow(string ns, string typeName)
        {
            if (!string.IsNullOrEmpty(ns))
                ns = ns + ".";

            var results = _rows.Where(x => x.FullName == $"{ns}{typeName}").ToList();

            if (results.Count == 1)
                return results.First();

            if (results.Count > 1)
                throw new Exception("Multiply types");

            throw new Exception("Type not found");
        }

        private TableClassRow FindRow(string typeName)
        {
            var parts = typeName.Split(".");

            if (parts.Length == 1)
                return FindRow("", typeName);
            else
                return FindRow(string.Join('.', parts[..^1]), parts.Last());
        }

        public IType FindType(string ns, string typeName)
        {
            return FindRow(ns, typeName).Type;
        }

        public IType FindType(string typeName)
        {
            return FindRow(typeName).Type;
        }
    }

    public static class BindingClassTalbeExt
    {
        public static void FillStandard(this ClassTable bc, SystemTypeBindings sb)
        {
            // var @int = new BindingClass("int", sb.Int);
            // var @string = new BindingClass("string", sb.String);
            // var @guid = new BindingClass("guid", sb.Guid);
            // var @int64 = new BindingClass("i64", sb.Int64);
            // var @int642 = new BindingClass("Platform.SomeNamespace.Int64", sb.Int64);
            //
            // bc.AddClass(@int, @int.BindingType);
            // bc.AddClass(@string, @string.BindingType);
            // bc.AddClass(guid, @guid.BindingType);
            // bc.AddClass(int64, @int64.BindingType);
            // bc.AddClass(int642, @int642.BindingType);
        }
    }
}