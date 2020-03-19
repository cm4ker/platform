using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BufferedDataReaderDotNet.Extensions
{
    internal static class TypeExtensions
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        private static readonly IDictionary<Type, Func<BinaryReader, object>> ReadFuncs =
            new Dictionary<Type, Func<BinaryReader, object>>
            {
                {typeof(byte[]), br => br.ReadBytes()},
                {typeof(DateTime), br => br.ReadDateTime()},
                {typeof(Guid), br => br.ReadGuid()},
                {typeof(TimeSpan), br => br.ReadTimeSpan()}
            };

        private static readonly IDictionary<Type, Action<object, BinaryWriter>> WriteActions =
            new Dictionary<Type, Action<object, BinaryWriter>>
            {
                {typeof(byte[]), (o, bw) => bw.WriteBytes((byte[]) o)},
                {typeof(DateTime), (o, bw) => bw.WriteDateTime((DateTime) o)},
                {typeof(Guid), (o, bw) => bw.WriteGuid((Guid) o)},
                {typeof(TimeSpan), (o, bw) => bw.WriteTimeSpan((TimeSpan) o)}
            };

        static TypeExtensions()
        {
            AddReadFuncs();
            AddWriteActions();
        }

        private static void AddReadFuncs()
        {
            var methodInfos = typeof(BinaryReader)
                .GetMethods(DefaultBindingFlags)
                .Where(mi => mi.Name.Length > 4 && mi.Name.StartsWith("Read"))
                .Where(mi => mi.GetParameters().Length == 0)
                .Where(mi => mi.ReturnType != typeof(void))
                .GroupBy(mi => mi.ReturnType)
                .Where(g => g.Count() == 1)
                .Select(g => g.First());

            foreach (var methodInfo in methodInfos)
            {
                var returnType = methodInfo.ReturnType;

                if (ReadFuncs.ContainsKey(returnType))
                    continue;

                var binaryReaderParameter = Expression.Parameter(typeof(BinaryReader), "binaryReader");
                var callExpression = Expression.Call(binaryReaderParameter, methodInfo);
                var convertExpression = Expression.Convert(callExpression, typeof(object));

                var lamdbaExpression = Expression.Lambda<Func<BinaryReader, object>>(
                    convertExpression, binaryReaderParameter);

                ReadFuncs.Add(returnType, lamdbaExpression.Compile());
            }
        }

        private static void AddWriteActions()
        {
            var methodInfos = typeof(BinaryWriter)
                .GetMethods(DefaultBindingFlags)
                .Where(mi => mi.Name == nameof(BinaryWriter.Write))
                .Select(mi => new {Parameters = mi.GetParameters(), MethodInfo = mi})
                .Where(x => x.Parameters.Length == 1)
                .ToDictionary(x => x.Parameters[0].ParameterType, x => x.MethodInfo);

            foreach (var keyValuePair in methodInfos)
            {
                var parameterType = keyValuePair.Key;
                var methodInfo = keyValuePair.Value;

                if (WriteActions.ContainsKey(parameterType))
                    continue;

                var valueParameter = Expression.Parameter(typeof(object), "value");
                var convertExpression = Expression.Convert(valueParameter, parameterType);
                var binaryWriterParameter = Expression.Parameter(typeof(BinaryWriter), "binaryWriter");
                var callExpression = Expression.Call(binaryWriterParameter, methodInfo, convertExpression);

                var lambdaExpression = Expression.Lambda<Action<object, BinaryWriter>>(
                    callExpression, valueParameter, binaryWriterParameter);

                WriteActions.Add(parameterType, lambdaExpression.Compile());
            }
        }

        public static Func<BinaryReader, object> GetReadFunc(
            this Type fieldType, BufferedDataOptions bufferedDataOptions)
        {
            Func<BinaryReader, object> readFunc;

            if (!bufferedDataOptions.ReadFuncs.TryGetValue(fieldType, out readFunc))
            {
                if (!ReadFuncs.TryGetValue(fieldType, out readFunc))
                {
                    // TODO: methodInfo is a misnomer
                    var methodInfo = fieldType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(mi => mi.Name == "Parse")
                        .Select(mi => new {Parameters = mi.GetParameters(), MethodInfo = mi})
                        .Where(x => x.Parameters.Length == 1)
                        // If there's any ambiguity we'll prefer Parse(String).
                        .OrderByDescending(x => x.Parameters[0].ParameterType == typeof(string))
                        .First(); // InvalidOperationException if we can't read.

                    var binaryReaderParameter = Expression.Parameter(typeof(BinaryReader), "binaryReader");
                    var readStringMethodInfo = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadString));
                    var callReadExpression = Expression.Call(binaryReaderParameter, readStringMethodInfo);
                    // We're trying to get to methods like SqlGeography.Parse(SqlString)
                    // whose SqlString parameter has an implicit conversion from String.
                    //var parseParameter = Expression.Convert(callReadExpression, methodInfo.Parameters[0].ParameterType);
                    //var callParseExpression = Expression.Call(null, methodInfo.MethodInfo, parseParameter);
                    //var convertExpression = Expression.Convert(callParseExpression, typeof(object));

                    // Just testing...
                    var convertExpression = Expression.Convert(callReadExpression, typeof(object));

                    var lambdaExpression = Expression.Lambda<Func<BinaryReader, object>>(
                        convertExpression, binaryReaderParameter);

                    return lambdaExpression.Compile();
                }
            }

            return readFunc;
        }

        public static Action<object, BinaryWriter> GetWriteAction(
            this Type fieldType, BufferedDataOptions bufferedDataOptions)
        {
            Action<object, BinaryWriter> writeAction;

            if (!bufferedDataOptions.WriteActions.TryGetValue(fieldType, out writeAction))
            {
                if (!WriteActions.TryGetValue(fieldType, out writeAction))
                    writeAction = (o, bw) => bw.Write(o.ToString());
            }

            return writeAction;
        }
    }
}