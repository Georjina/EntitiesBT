using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;
using static EntitiesBT.Variable.Utilities;

namespace EntitiesBT.Variable
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableComponentDataAttribute : PropertyAttribute {}

    public static class ComponentVariableProperty
    {
        public const string GUID = "8E5CDB60-17DB-498A-B925-2094062769AB";

        public struct DynamicComponentData
        {
            public ulong StableHash;
            public int Offset;
        }

        [Preserve, AccessorMethod(GUID)]
        private static IEnumerable<ComponentType> GetDynamicAccess(ref BlobVariable blobVariable)
        {
            var hash = blobVariable.Value<DynamicComponentData>().StableHash;
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(hash);
            return ComponentType.FromTypeIndex(typeIndex).Yield();
        }

        [Serializable]
        public class Reader<T> : IVariablePropertyReader<T> where T : struct
        {
            [VariableComponentData] public string ComponentValueName;

            public void Allocate(ref BlobBuilder builder, ref BlobVariable blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariable.VariableId = GuidHashCode(GUID);
                var data = GetTypeHashAndFieldOffset(ComponentValueName);
                if (data.Type != typeof(T) || data.Hash == 0)
                {
                    Debug.LogError($"ComponentVariable({ComponentValueName}) is not valid, fallback to ConstantValue", (UnityEngine.Object)self);
                    throw new ArgumentException();
                }
                builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = data.Hash, Offset = data.Offset});
            }

            private static unsafe ref T GetComponentValue(ulong stableHash, int offset, Func<Type, IntPtr> getDataPtr)
            {
                var index = TypeManager.GetTypeIndexFromStableTypeHash(stableHash);
                var componentPtr = getDataPtr(TypeManager.GetType(index));
                // TODO: type safety check
                var dataPtr = componentPtr + offset;
                return ref UnsafeUtility.AsRef<T>(dataPtr.ToPointer());
            }

            [Preserve, ReaderMethod(GUID)]
            private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariable blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariable.Value<DynamicComponentData>();
                return GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRO);
            }

            [Preserve, RefReaderMethod(GUID)]
            private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariable blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                ref var data = ref blobVariable.Value<DynamicComponentData>();
                return ref GetComponentValue(data.StableHash, data.Offset, bb.GetDataPtrRW);
            }
        }
    }
}
