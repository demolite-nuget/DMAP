// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using DMAP.Net.Benchmarks.Runners;

var summary = BenchmarkRunner.Run<DMapperBenchmarks>();