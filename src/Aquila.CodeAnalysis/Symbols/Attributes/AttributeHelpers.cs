using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.Symbols.PE;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Attributes
{
    static class AttributeHelpers
    {
        private static readonly byte[] s_signature_HasThis_Void = new byte[]
            { (byte)SignatureAttributes.Instance, 0, (byte)SignatureTypeCode.Void };


        public static readonly AttributeDescription NotNullAttribute =
            new AttributeDescription(CoreTypes.AquilaRuntimeNamespace, "NotNullAttribute",
                new[] { s_signature_HasThis_Void });

        public static readonly AttributeDescription AquilaHiddenAttribute =
            new AttributeDescription(CoreTypes.AquilaRuntimeNamespace, "AquilaHiddenAttribute",
                new[] { s_signature_HasThis_Void });

        public static readonly AttributeDescription CastToFalse =
            new AttributeDescription(CoreTypes.AquilaRuntimeNamespace, "CastToFalse",
                new[] { s_signature_HasThis_Void });

        public static bool HasCastToFalse(EntityHandle token, PEModuleSymbol containingModule)
        {
            return containingModule != null && PEModule
                .FindTargetAttribute(containingModule.Module.MetadataReader, token, CastToFalse).HasValue;
        }

        public static bool HasAquilaHiddenAttribute(EntityHandle token, PEModuleSymbol containingModule)
        {
            return containingModule != null && PEModule
                .FindTargetAttribute(containingModule.Module.MetadataReader, token, AquilaHiddenAttribute).HasValue;
        }

        static bool ReadCustomAttributeValue(CustomAttributeHandle handle, PEModule module, out int value)
        {
            // PEModule.cs

            var valueBlob = module.GetCustomAttributeValueOrThrow(handle);
            if (!valueBlob.IsNil)
            {
                // TODO: error checking offset in range
                var reader = module.MetadataReader.GetBlobReader(valueBlob);

                if (reader.Length > 4)
                {
                    // check prolog
                    if (reader.ReadByte() == 1 && reader.ReadByte() == 0)
                    {
                        // read Int32
                        if (reader.RemainingBytes >= 4)
                        {
                            value = reader.ReadInt32();
                            return true;
                        }
                    }
                }
            }

            value = default;
            return false;
        }

        public static ImportValueAttributeData HasImportValueAttribute(EntityHandle token,
            PEModuleSymbol containingModule)
        {
            var metadataReader = containingModule.Module.MetadataReader;
            foreach (var attr in metadataReader.GetCustomAttributes(token))
            {
                if (containingModule.Module.IsTargetAttribute(attr, CoreTypes.AquilaRuntimeNamespace,
                    "ImportValueAttribute", out _))
                {
                    // [ImportValue(Int32)]
                    if (ReadCustomAttributeValue(attr, containingModule.Module, out var valuespec))
                    {
                        Debug.Assert(valuespec != 0);
                        return new ImportValueAttributeData { Value = (ImportValueAttributeData.ValueSpec)valuespec };
                    }
                }
            }

            //
            return default;
        }

        /// <summary>
        /// Looks for <c>Aquila.Runtime</c>'s <c>DefaultValueAttribute</c>.
        /// </summary>
        public static bool HasDefaultValueAttributeData(EntityHandle token, PEModuleSymbol containingModule)
        {
            try
            {
                var metadataReader = containingModule.Module.MetadataReader;
                foreach (var attr in metadataReader.GetCustomAttributes(token))
                {
                    if (containingModule.Module.IsTargetAttribute(attr, CoreTypes.AquilaRuntimeNamespace,
                        "DefaultValueAttribute", out _))
                    {
                        return true;
                    }
                }
            }
            catch (BadImageFormatException)
            {
            }

            //
            return false;
        }

        public static bool HasNotNullAttribute(EntityHandle token, PEModuleSymbol containingModule)
        {
            // TODO: C# 8.0 NotNull

            return containingModule != null && PEModule
                .FindTargetAttribute(containingModule.Module.MetadataReader, token, NotNullAttribute).HasValue;
        }
    }

    struct ImportValueAttributeData
    {
        /// <summary>
        /// Value to be imported.
        /// </summary>
        public enum ValueSpec
        {
            Default = 0,

            /// <summary>
            /// Current class context.
            /// </summary>
            CallerClass,

            /// <summary>
            /// Current late static bound class (<c>static</c>).
            /// </summary>
            CallerStaticClass,

            /// <summary>
            /// Calue of <c>$this</c> variable or <c>null</c> if variable is not defined.
            /// </summary>
            This,

            /// <summary>
            /// Provides a reference to the array of local variables.
            /// </summary>
            Locals,

            /// <summary>
            /// Provides callers parameters.
            /// </summary>
            CallerArgs,

            /// <summary>
            /// Provides reference to the current script container.
            /// The parameter must be of type <see cref="RuntimeTypeHandle"/>.
            /// </summary>
            CallerScript,
        }

        public bool IsDefault => Value == ValueSpec.Default;

        public bool IsValid => Value != ValueSpec.Default && Value != (ValueSpec)(-1);

        public static ImportValueAttributeData Invalid => new ImportValueAttributeData { Value = (ValueSpec)(-1) };

        public ValueSpec Value;
    }
}