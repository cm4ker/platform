using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aquila.Core;
using Aquila.Metadata;

namespace Aquila.Library.Scripting
{
    public sealed class ScriptingProvider : AqContext.IScriptingProvider
    {
        readonly Dictionary<string, List<Script>> _scripts = new(StringComparer.Ordinal);

        readonly ReaderWriterLockSlim _scriptsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        readonly AquilaCompilationFactory _builder = new AquilaCompilationFactory();

        Script TryGetOrCreateScript(string code, MetadataProvider metadata,
            AqContext.ScriptOptions options, ScriptingContext context)
        {
            var script = default(Script);

            _scriptsLock.EnterUpgradeableReadLock();
            try
            {
                if (!_scripts.TryGetValue(code, out var subsmissions))
                {
                    _scriptsLock.EnterWriteLock();
                    try
                    {
                        if (!_scripts.TryGetValue(code, out subsmissions))
                        {
                            _scripts[code] = subsmissions = new List<Script>(1);
                        }
                    }
                    finally
                    {
                        _scriptsLock.ExitWriteLock();
                    }
                }

                if ((script = CacheLookupNoLock(subsmissions, options, code, context)) == null)
                {
                    _scriptsLock.EnterWriteLock();
                    try
                    {
                        if ((script = CacheLookupNoLock(subsmissions, options, code, context)) == null)
                        {
                            subsmissions.Add((script =
                                Script.Create(options, code, _builder, context.Submissions, metadata)));
                        }
                    }
                    finally
                    {
                        _scriptsLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _scriptsLock.ExitUpgradeableReadLock();
            }

            return script;
        }

        Script CacheLookupNoLock(List<Script> candidates, AqContext.ScriptOptions options, string code,
            ScriptingContext context)
        {
            foreach (var c in candidates)
            {
                Debug.Assert(c.DependingSubmissions != null);

                // candidate requires that all its dependencies were loaded into context
                // TODO: resolve the compiled code dependencies - referenced types and declared functions - instead of "DependingSubmissions"
                if (c.DependingSubmissions.All(context.Submissions.Contains))
                {
                    return c;
                }
            }

            return null;
        }

        AqContext.IScript AqContext.IScriptingProvider.CreateScript(AqContext.ScriptOptions options, string code,
            MetadataProvider metadata)
        {
            var context = ScriptingContext.EnsureContext(options.Context);
            var script = TryGetOrCreateScript(code, metadata, options, context);

            Debug.Assert(script != null);

            //
            context.Submissions.Add(script);

            //
            return script;
        }
    }
}