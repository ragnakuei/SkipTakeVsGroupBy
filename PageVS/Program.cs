using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace PageVS
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestRunner>();
        }
    }

    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class TestRunner
    {
        private readonly TestClass _test = new TestClass();

        public TestRunner()
        {
        }

        [Benchmark]
        public void Method1ForEnum() => _test.SelectWithIndexGroupByForEnum();

        [Benchmark]
        public void Method2ForEnum() => _test.SkipTakeForEnum();

        [Benchmark]
        public void Method1ForArray() => _test.SelectWithIndexGroupByForArray();

        [Benchmark]
        public void Method2ForArray() => _test.SkipTakeForArray();
    }

    public class TestClass
    {
        private readonly int _batchCount = 4;
        private readonly IEnumerable<Test> _sampleDataEnum ;
        private readonly Test[] _sampleDataEnumArray;

        public TestClass()
        {
            _sampleDataEnum = GetData();
            _sampleDataEnumArray = GetData().ToArray();
        }

        private IEnumerable<Test> GetData()
        {
            return Enumerable.Range(0, 50).Select(i => new Test { Id = i, Name = ( (char) ( i + 97 ) ).ToString() });
        }

        public void SelectWithIndexGroupByForEnum()
        {
            var result = _sampleDataEnum.Select((v, i) => new { index = i, value = v })
                                    .GroupBy(g => g.index / _batchCount)
                                    .Select(i=>i.Select(x=>x.value));
            Print(result);
        }

        public void SkipTakeForEnum()
        {
            var totalCount = _sampleDataEnum.Count();
            var pageCount = (totalCount % _batchCount == 0)
                                ? (totalCount / _batchCount)
                                : (totalCount / _batchCount) + 1;

            var result = Enumerable.Range(0, pageCount)
                                   .Select(c => _sampleDataEnum.Skip(c * _batchCount)
                                                           .Take(_batchCount));
            Print(result);
        }

        public void SelectWithIndexGroupByForArray()
        {
            var result = _sampleDataEnumArray.Select((v, i) => new { index = i, value = v })
                                        .GroupBy(g => g.index / _batchCount)
                                        .Select(i => i.Select(x => x.value));
            Print(result);
        }

        public void SkipTakeForArray()
        {
            var totalCount = _sampleDataEnumArray.Length;
            var pageCount = (totalCount % _batchCount == 0)
                                ? (totalCount / _batchCount)
                                : (totalCount / _batchCount) + 1;

            var result = Enumerable.Range(0, pageCount)
                                   .Select(c => _sampleDataEnumArray.Skip(c * _batchCount)
                                                               .Take(_batchCount));
            Print(result);
        }

        private void Print(IEnumerable<IEnumerable<Test>> groupbyItems)
        {
            foreach (var groupbyItem in groupbyItems)
            {
                foreach (var item in groupbyItem)
                {
                    var a = item;
                }

                var b = "===================================";
            }
        }
    }

    public class Test
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
    }
}