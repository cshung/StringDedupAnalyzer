namespace DumpStrings
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Diagnostics.Runtime;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            bool detailed = false;
            if (args.Length != 1)
            {
                Console.WriteLine("dotnet run dmp");
                return;
            }
            var strings = new Dictionary<string, ClrObject>();
            using (DataTarget dataTarget = DataTarget.LoadCrashDump(args[0]))
            {
                ClrInfo runtimeInfo = dataTarget.ClrVersions[0];
                ClrRuntime runtime = runtimeInfo.CreateRuntime();
                ClrHeap heap = runtime.Heap;
                ulong gen2Size = 0;

                long gen2StringCount = 0;
                long gen2ScanStringCount = 0;
                long gen2DupStringCount = 0;

                long gen2StringSize = 0;
                long gen2ScanStringSize = 0;
                long gen2DupStringSize = 0;
                foreach (ClrSegment segment in heap.Segments)
                {
                    gen2Size += segment.Gen2Length;
                }
                foreach (ClrObject obj in heap.EnumerateObjects())
                {
                    int? objGen = GenerationOf(heap, obj);
                    if (objGen == 2)
                    {
                        if (obj.Type.Name.Equals("System.String"))
                        {
                            // This are all the strings in gen2, they may or may not be live, they may or may not have a gen 2 reference.
                            // This should give an idea why strings are important
                            // My prediction is that this should be a significant percentage (otherwise why bother?)
                            gen2StringCount++;
                            gen2StringSize += obj.AsString().Length * 2;
                        }
                        foreach (ClrObject referencedObject in obj.EnumerateReferences())
                        {
                            int? refGen = GenerationOf(heap, referencedObject);
                            // This happen when we see a string referenced by a gen2 object
                            if (refGen == 2)
                            {
                                if (referencedObject.Type.Name.Equals("System.String"))
                                {
                                    if (detailed)
                                    {
                                        Console.WriteLine("A gen 2 object {0:X} is referencing a gen 2 string {1:X}", obj.Address, referencedObject.Address);
                                    }

                                    // This is a string the string deduplication algorithm would consider to put into the hash table
                                    // This should give an idea how much work we need to do to scan these strings
                                    // My assumption is that the majority of the work is the computing of the hash code, that why
                                    // the cost should be roughly corresponding to the total length of the strings
                                    string referencedObjectAsString = referencedObject.AsString();
                                    gen2ScanStringCount++;
                                    gen2ScanStringSize += referencedObjectAsString.Length * 2;
                                    // This happen when we see a gen2 string referenced by a gen2 object
                                    ClrObject existingObject;
                                    if (strings.TryGetValue(referencedObjectAsString, out existingObject))
                                    {
                                        // This happen when we see a gen2 string referenced by a gen2 object with a seen content
                                        if (referencedObject.Address != existingObject.Address)
                                        {
                                            if (detailed)
                                            {
                                                Console.WriteLine("And it is considered a duplicate with {0:X}", existingObject.Address);
                                            }
                                            // This happen when we see a different gen 2 string referenced by a gen2 object with a seen content
                                            // We do not know if deduplicating it will lead to its death (it depends on whether or not that string has any other references)
                                            // which would be expensive to compute, but this should give an upper bound on how much we could save.
                                            gen2DupStringCount++;
                                            gen2DupStringSize += referencedObjectAsString.Length * 2;
                                        }
                                    }
                                    else
                                    {
                                        // This happen when we see a gen 2 string referenced by a gen2 object that is not seen before
                                        strings.Add(referencedObjectAsString, referencedObject);
                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Gen 2 size         : " + gen2Size);
                Console.WriteLine("Gen 2 strings      : " + gen2StringSize + " bytes out of " + gen2StringCount + " strings");
                Console.WriteLine("Gen 2 scan string  : " + gen2ScanStringSize + " bytes out of " + gen2ScanStringCount + " strings");
                Console.WriteLine("Gen 2 dup string   : " + gen2DupStringSize + " bytes out of " + gen2DupStringCount + " strings");
                Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}",gen2Size,gen2StringSize,gen2StringCount,gen2ScanStringSize,gen2ScanStringCount,gen2DupStringSize,gen2DupStringCount);
            }
        }

        private static int? GenerationOf(ClrHeap heap, ClrObject obj)
        {
            ClrSegment segment = heap.GetSegmentByAddress(obj.Address);
            if (segment == null)
            {
                // This happen if the object is no longer live
                return null;
            }
            else
            {
                return segment.GetGeneration(obj);
            }
        }
    }
}
