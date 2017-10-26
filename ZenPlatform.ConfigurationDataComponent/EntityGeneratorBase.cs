using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration.Data;
using ZenPlatform.CSharpCodeBuilder.Syntax;

namespace ZenPlatform.DataComponent
{
    public abstract class EntityGeneratorBase
    {
        protected EntityGeneratorBase()
        {

        }

        public virtual void GenerateEntityClass(DocumentSyntax document, PObjectType conf)
        {
            var workspace = new AdhocWorkspace();

            // Получаем SyntaxGenerator для указанного языка
            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var usingDirectives =
                generator.NamespaceImportDeclaration("System");

            if (conf.IsAbstractType) return;

            var members = new List<SyntaxNode>();

            foreach (var prop in conf.Propertyes)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is PObjectType)
                        members.Add(generator.PropertyDeclaration(prop.Name, generator.IdentifierName(propType.Name))); // cs.AddProperty(new PropertySyntax(prop.Name, new TypeSyntax(propType.Name)));

                    if (propType is PPrimetiveType primitiveType)
                        //members.Add(generator);
                        members.Add(generator.PropertyDeclaration(prop.Name, generator.IdentifierName(primitiveType.CLRType.CSharpName()), Accessibility.NotApplicable, );//cs.AddProperty(new PropertySyntax(prop.Name, new TypeSyntax(primitiveType.CLRType)));
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is PObjectType && !alreadyHaveObjectTypeField)
                        {
                            members.Add(generator.PropertyDeclaration($"{prop.Name}_Ref", generator.IdentifierName("Guid")));//cs.AddProperty(new PropertySyntax($"{prop.Name}_Ref", new TypeSyntax(typeof(Guid))));
                            members.Add(generator.PropertyDeclaration($"{prop.Name}_Type", generator.IdentifierName("int")));// cs.AddProperty(new PropertySyntax($"{prop.Name}_Type", new TypeSyntax(typeof(int))));
                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is PPrimetiveType primitiveType)
                            members.Add(generator.PropertyDeclaration($"{prop.Name}_{primitiveType.Name}", generator.IdentifierName(primitiveType.CLRType.CSharpName())));// cs.AddProperty(new PropertySyntax($"{prop.Name}_{primitiveType.Name}", new TypeSyntax(primitiveType.CLRType)));
                    }
                }
            }


            //// Массив объектов SyntaxNode как членов класса
            //var members = new SyntaxNode[] { lastNameField,
            //    firstNameField, lastNameProperty, firstNameProperty,
            //    cloneMethodWithInterfaceType, constructor };

            // Генерируем класс
            var classDefinition = generator.ClassDeclaration(
                "Person", typeParameters: null,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Abstract,
                baseType: null,
                interfaceTypes: null,
                members: members);

            // Объявляем пространство имен
            var namespaceDeclaration =
                generator.NamespaceDeclaration("MyTypes", classDefinition);

            // Получаем CompilationUnit (файл кода)
            // для сгенерированного кода
            var newNode = generator.CompilationUnit(usingDirectives,
                namespaceDeclaration).NormalizeWhitespace();

            Console.WriteLine(newNode);
        }
    }
}