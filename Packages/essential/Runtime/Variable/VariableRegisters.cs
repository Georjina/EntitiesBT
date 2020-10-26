using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Core.Utilities;
using static EntitiesBT.Variable.Utilities;

namespace EntitiesBT.Variable
{
    public class MethodIdAttribute : Attribute
    {
        internal readonly int ID;
        public MethodIdAttribute(string guid) => ID = GuidHashCode(guid);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReaderMethodAttribute : MethodIdAttribute
    {
        public ReaderMethodAttribute(string guid) : base(guid) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RefReaderMethodAttribute : MethodIdAttribute
    {
        public RefReaderMethodAttribute(string guid) : base(guid) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class WriterMethodAttribute : MethodIdAttribute
    {
        public WriterMethodAttribute(string id) : base(id) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AccessorMethodAttribute : MethodIdAttribute
    {
        public AccessorMethodAttribute(string id) : base(id) {}
    }

    internal static class VariableRegisters
    {
        internal delegate IEnumerable<ComponentType> GetComponentAccessFunc(ref BlobVariable variable);
        internal static readonly IReadOnlyDictionary<int, MethodInfo> READERS;
        internal static readonly IReadOnlyDictionary<int, MethodInfo> REF_READERS;
        internal static readonly IReadOnlyDictionary<int, MethodInfo> WRTIERS;
        internal static readonly IReadOnlyDictionary<int, GetComponentAccessFunc> ACCESSORS;
        public static GetComponentAccessFunc GetComponentAccess(int entryId)
        {
            return ACCESSORS.TryGetValue(entryId, out var func) ? func : GetComponentAccessDefault;
        }

        private static IEnumerable<ComponentType> GetComponentAccessDefault(ref BlobVariable _) => Enumerable.Empty<ComponentType>();

        static VariableRegisters()
        {
            var readers = new Dictionary<int, MethodInfo>();
            var refReaders = new Dictionary<int, MethodInfo>();
            var writers = new Dictionary<int, MethodInfo>();
            var accessors = new Dictionary<int, GetComponentAccessFunc>();
            foreach (var (methodInfo, attribute) in BEHAVIOR_TREE_ASSEMBLY_TYPES.Value
                .Where(type => IsReader(type) || IsWriter(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .SelectMany(methodInfo => methodInfo.GetCustomAttributes<MethodIdAttribute>().Select(attribute => (methodInfo, attribute)))
            )
            {
                switch (attribute)
                {
                case ReaderMethodAttribute reader:
                    readers.Add(reader.ID, methodInfo);
                    break;
                case RefReaderMethodAttribute refReader:
                    refReaders.Add(refReader.ID, methodInfo);
                    break;
                case WriterMethodAttribute writer:
                    writers.Add(writer.ID, methodInfo);
                    break;
                case AccessorMethodAttribute accessor:
                    accessors.Add(accessor.ID, (GetComponentAccessFunc)methodInfo.CreateDelegate(typeof(GetComponentAccessFunc)));
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            READERS = new ReadOnlyDictionary<int, MethodInfo>(readers);
            REF_READERS = new ReadOnlyDictionary<int, MethodInfo>(refReaders);
            WRTIERS = new ReadOnlyDictionary<int, MethodInfo>(writers);
            ACCESSORS = new ReadOnlyDictionary<int, GetComponentAccessFunc>(accessors);
        }
    }

    public static class VariableRegisters<T> where T : struct
    {
        private static IReadOnlyDictionary<int, TDelegate> MakeRegisterDictionary<TDelegate, TNodeBlob, TBlackboard>(
            IReadOnlyDictionary<int, MethodInfo> methodInfoMap
        )
            where TDelegate : System.Delegate
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var map = new Dictionary<int, TDelegate>(methodInfoMap.Count * 3);
            var types = new [] {typeof(TNodeBlob), typeof(TBlackboard)};
            foreach (var keyValue in methodInfoMap)
            {
                var func = (TDelegate)keyValue.Value
                    .MakeGenericMethod(types)
                    .CreateDelegate(typeof(TDelegate))
                ;
                map[keyValue.Key] = func;
            }
            return new ReadOnlyDictionary<int, TDelegate>(map);
        }

        public delegate T ReadFunc<TNodeBlob, TBlackboard>(ref BlobVariable variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public delegate ref T ReadRefFunc<TNodeBlob, TBlackboard>(ref BlobVariable variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public static ReadFunc<TNodeBlob, TBlackboard> GetReader<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => ReaderRegisters<TNodeBlob, TBlackboard>.READERS[entryId];

        public static ReadRefFunc<TNodeBlob, TBlackboard> GetRefReader<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => RefReaderRegisters<TNodeBlob, TBlackboard>.READERS[entryId];

        // optimize: convert method info into delegate to increase performance on calling.
        private static class ReaderRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, ReadFunc<TNodeBlob, TBlackboard>> READERS =
                MakeRegisterDictionary<ReadFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariableRegisters.READERS);
        }

        // optimize: convert method info into delegate to increase performance on calling.
        private static class RefReaderRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, ReadRefFunc<TNodeBlob, TBlackboard>> READERS =
                MakeRegisterDictionary<ReadRefFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariableRegisters.REF_READERS);
        }

        public delegate void WriteFunc<TNodeBlob, TBlackboard>(ref BlobVariable variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public static WriteFunc<TNodeBlob, TBlackboard> GetWriter<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => WriterRegisters<TNodeBlob, TBlackboard>.WRITERS[entryId];

        // optimize: convert writer method info into delegate to increase performance on calling.
        private static class WriterRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, WriteFunc<TNodeBlob, TBlackboard>> WRITERS =
                MakeRegisterDictionary<WriteFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariableRegisters.WRTIERS);
        }
    }
}
