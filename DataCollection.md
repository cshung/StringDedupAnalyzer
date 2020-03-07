# Call for data
In order to proceed with the [StringDedup feature](https://github.com/dotnet/runtime/blob/master/docs/design/features/StringDeduplication.md), we needed your help!

# How can you help?
We wanted to understand how many strings (in terms of bytes and counts) are duplicated in the managed heap for a .NET Core process. 

To help, you can create a memory dump of your process using [dotnet-dump](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-dump) or otherwise, then you run the tool against the dump.

Here is an example session on how the tool is ran:

```c#
using System;

namespace Strings
{
    class Holder
    {
        public string s1;
        public string s2;
        public string s3;
        public string s4;
        public string s5;
    }
    class Program
    {
        static void Main(string[] args)
        {
            // In order for a string to be deduplicated, it has to be referenced by a gen 2 object.
            // Therefore we create a holder object that could be moved to gen 2.
            Holder holder1 = new Holder();
            // Deliberately create two duplicated strings
            holder1.s1 = new string('A', 15);
            holder1.s2 = new string('A', 15);
            // Forcing all objects to gen 2.
            GC.Collect();
            GC.Collect();
            Console.WriteLine("Collect the dump");
            Console.ReadLine();
        }
    }
}
```

We can invoke `dotnet-dump` to create a memory dump
```sh
dotnet-dump collect -p 12345
```

Then we can run the tool to analyze for the result:
```sh
dotnet run dump_20200306_12345.dmp
```

The analyzer should output:
```
Gen 2 size         : 65680
Gen 2 strings      : 12844 bytes out of 66 strings
Gen 2 scan string  : 9866 bytes out of 33 strings
Gen 2 dup string   : 30 bytes out of 1 strings
65680,12844,66,9866,33,30,1
```

To complete the data collection process, update your finding in [data.csv](data.csv) and [make a pull request].

## Caveats

If the dump analysis failed if this exception:
```
Unhandled exception. System.IO.FileNotFoundException: Could not find matching DAC for this runtime.
File name: 'mscordacwks_Amd64_Amd64_4.7.3468.00.dll'
   at Microsoft.Diagnostics.Runtime.ClrInfo.CreateRuntime() in C:\Dev\clrmd\src\Microsoft.Diagnostics.Runtime\src\DataTargets\ClrInfo.cs:line 95
   at DumpStrings.Program.Main(String[] args) in C:\Dev\StringDedupAnalyzer\src\StringDedupAnalyzer\Program.cs:line 21
```

You might need to set the symbol path (on Windows) - the symbol path is only used to obtain the data acess binary (i.e. mscordacwks* for .NET framework or mscordaccore for .NET Core)
```sh
SET _NT_SYMBOL_PATH=https://msdl.microsoft.com/download/symbols
```

If your dump is huge, it can take some good amount of time to process it. You can check the CPU and memory usage of the tool, it should be spinning a single thread hot and the memory should be increasing.

If possible, **keep the memory dump**. Be sure to comply with the necessary laws in your country in case the memory dump could contain customer data. The tool is still a work in progress and it won't surprise me if there are bugs. Keeping the memory dump allow us to rerun the tool as necessary. We will never request for the dump, we only wanted the statistics.

# I can help more
This is great! With a more involved contributor like you, we can experiment more. In particular, here is a prototype branch with a rudimentary string deduplication prototype implemented. It would be really nice to see it in action in real world applications. This part is still a work in progress.

```sh
# TODO - we need an example here
```

[make a pull request]: https://help.github.com/articles/creating-a-pull-request