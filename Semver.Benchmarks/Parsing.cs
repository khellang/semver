﻿extern alias current;
using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SemVersion_Current = current::Semver.SemVersion;
using SemVersion_Previous = Semver.SemVersion;
using SemVersionStyles_Current = current::Semver.SemVersionStyles;

namespace Semver.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    public class Parsing
    {
        private const int Seed = -217274474;
        private const int VersionCount = 1_000;
        private readonly IReadOnlyList<string> versions;

        public Parsing()
        {
            var random = new Random(Seed);
            versions = Enumerables.Generate(VersionCount,
                                      () => random.VersionString())
                                  .ToList().AsReadOnly();
        }

        [Benchmark(OperationsPerInvoke = VersionCount)]
        [Arguments(true)]
        [Arguments(false)]
        public long PreviousRegExParsing(bool strict)
        {
            // The accumulator ensures the versions aren't dead code with minimal overhead
            long accumulator = 0;
            for (int i = 0; i < VersionCount; i++)
            {
                var version = SemVersion_Previous.Parse(this.versions[i], strict);
                accumulator += version.Major;
            }

            return accumulator;
        }

        [Benchmark(OperationsPerInvoke = VersionCount)]
        [Arguments(true)]
        [Arguments(false)]
        public long CurrentRegExParsing(bool strict)
        {
            // The accumulator ensures the versions aren't dead code with minimal overhead
            long accumulator = 0;
            for (int i = 0; i < VersionCount; i++)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var version = SemVersion_Current.Parse(this.versions[i], strict);
#pragma warning restore CS0618 // Type or member is obsolete
                accumulator += version.Major;
            }

            return accumulator;
        }

        [Benchmark(OperationsPerInvoke = VersionCount)]
        [Arguments(SemVersionStyles_Current.Strict)]
        [Arguments(SemVersionStyles_Current.Any)]
        public long CurrentParsing(SemVersionStyles_Current style)
        {
            // The accumulator ensures the versions aren't dead code with minimal overhead
            long accumulator = 0;
            for (int i = 0; i < VersionCount; i++)
            {
                var version = SemVersion_Current.Parse(versions[i], style);
                accumulator += version.Major;
            }

            return accumulator;
        }
    }
}